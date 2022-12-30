// ------------------------------------------------------------------------------------------------------
// <copyright file="CeloscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net;
using System.Text.Json;

using Nethereum.Util;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Celoscan.Calculators;
using Nomis.Celoscan.Interfaces;
using Nomis.Celoscan.Interfaces.Extensions;
using Nomis.Celoscan.Interfaces.Models;
using Nomis.Celoscan.Responses;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.Celoscan
{
    /// <inheritdoc cref="ICeloScoringService"/>
    internal sealed class CeloscanService :
        ICeloScoringService,
        ITransientService
    {
        private readonly ICeloscanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="CeloscanService"/>.
        /// </summary>
        /// <param name="client"><see cref="ICeloscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public CeloscanService(
            ICeloscanClient client,
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
        public async Task<Result<CeloWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            string? balanceWei = (await _client.GetBalanceAsync(address)).Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoCeloUsdPriceResponse>(balanceWei?.ToCelo() ?? 0, "celo");
            var transactions = (await _client.GetTransactionsAsync<CeloscanAccountNormalTransactions, CeloscanAccountNormalTransaction>(address)).ToList();
            var internalTransactions = (await _client.GetTransactionsAsync<CeloscanAccountInternalTransactions, CeloscanAccountInternalTransaction>(address)).ToList();
            var erc20Tokens = (await _client.GetTransactionsAsync<CeloscanAccountERC20TokenEvents, CeloscanAccountERC20TokenEvent>(address)).ToList();
            var tokens = (await _client.GetTransactionsAsync<CeloscanAccountERC721TokenEvents, CeloscanAccountERC721TokenEvent>(address)).ToList();

            var walletStats = new CeloStatCalculator(
                    address,
                    decimal.TryParse(balanceWei, out decimal wei) ? wei : 0,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    tokens,
                    erc20Tokens)
                .GetStats();

            double score = walletStats.GetScore<CeloWalletStats, CeloTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<CeloWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got celo wallet score.");
        }
    }
}