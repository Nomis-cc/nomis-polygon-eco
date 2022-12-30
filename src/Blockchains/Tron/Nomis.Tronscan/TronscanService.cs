// ------------------------------------------------------------------------------------------------------
// <copyright file="TronscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json;

using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.ScoringService.Interfaces;
using Nomis.Tronscan.Calculators;
using Nomis.Tronscan.Interfaces;
using Nomis.Tronscan.Interfaces.Models;
using Nomis.Tronscan.Responses;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Wrapper;

namespace Nomis.Tronscan
{
    /// <inheritdoc cref="ITronScoringService"/>
    internal sealed class TronscanService :
        ITronScoringService,
        ITransientService
    {
        private readonly ITronscanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="TronscanService"/>.
        /// </summary>
        /// <param name="client"><see cref="ITronscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public TronscanService(
            ITronscanClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 111115;

        /// <inheritdoc/>
        public bool IsEVMCompatible => false;

        /// <inheritdoc/>
        public async Task<Result<TronWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            var accountData = await _client.GetBalanceAsync(address);
            decimal balance = accountData.Tokens?.Sum(x => x.Amount) ?? accountData.Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoTronUsdPriceResponse>(balance, "tron");
            var contractsData = await _client.GetContractsAsync(address);
            var transactions = (await _client.GetTransactionsAsync<TronscanAccountNormalTransactions, TronscanAccountNormalTransaction>(address)).ToList();
            var internalTransactions = (await _client.GetTransactionsAsync<TronscanAccountInternalTransactions, TronscanAccountInternalTransaction>(address)).ToList();
            var transfers = await _client.GetTransactionsAsync<TronscanAccountTransfers, TronscanAccountTransfer>(address);
            var tokens = transfers.Where(x => x.TokenInfo?.TokenType?.Equals("trc721", StringComparison.OrdinalIgnoreCase) == true).ToList();

            var walletStats = new TronStatCalculator(
                    address,
                    accountData,
                    balance,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    tokens,
                    accountData.Trc20Balances ?? new(),
                    contractsData)
                .GetStats();

            double score = walletStats.GetScore<TronWalletStats, TronTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<TronWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got tron wallet score.");
        }
    }
}