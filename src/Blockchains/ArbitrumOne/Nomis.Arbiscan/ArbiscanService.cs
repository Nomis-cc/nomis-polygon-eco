// ------------------------------------------------------------------------------------------------------
// <copyright file="ArbiscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net;
using System.Text.Json;

using Nethereum.Util;
using Nomis.Arbiscan.Calculators;
using Nomis.Arbiscan.Interfaces;
using Nomis.Arbiscan.Interfaces.Extensions;
using Nomis.Arbiscan.Interfaces.Models;
using Nomis.Arbiscan.Responses;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.Arbiscan
{
    /// <inheritdoc cref="IArbitrumOneScoringService"/>
    internal sealed class ArbiscanService :
        IArbitrumOneScoringService,
        ITransientService
    {
        private readonly IArbiscanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="ArbiscanService"/>.
        /// </summary>
        /// <param name="client"><see cref="IArbiscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public ArbiscanService(
            IArbiscanClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 42220;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<Result<ArbitrumOneWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            string? balanceWei = (await _client.GetBalanceAsync(address)).Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoEthereumUsdPriceResponse>(balanceWei?.ToEth() ?? 0, "ethereum");
            var transactions = (await _client.GetTransactionsAsync<ArbiscanAccountNormalTransactions, ArbiscanAccountNormalTransaction>(address)).ToList();
            var internalTransactions = (await _client.GetTransactionsAsync<ArbiscanAccountInternalTransactions, ArbiscanAccountInternalTransaction>(address)).ToList();
            var erc20Tokens = (await _client.GetTransactionsAsync<ArbiscanAccountERC20TokenEvents, ArbiscanAccountERC20TokenEvent>(address)).ToList();
            var tokens = (await _client.GetTransactionsAsync<ArbiscanAccountERC721TokenEvents, ArbiscanAccountERC721TokenEvent>(address)).ToList();

            var walletStats = new ArbitrumOneStatCalculator(
                    address,
                    decimal.TryParse(balanceWei, out decimal wei) ? wei : 0,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    tokens,
                    erc20Tokens)
                .GetStats();

            double score = walletStats.GetScore<ArbitrumOneWalletStats, ArbitrumOneTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<ArbitrumOneWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got Arbitrum One wallet score.");
        }
    }
}