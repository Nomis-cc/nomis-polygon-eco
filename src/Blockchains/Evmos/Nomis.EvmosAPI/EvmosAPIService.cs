// ------------------------------------------------------------------------------------------------------
// <copyright file="EvmosAPIService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net;
using System.Text.Json;

using Nethereum.Util;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.EvmosAPI.Calculators;
using Nomis.EvmosAPI.Interfaces;
using Nomis.EvmosAPI.Interfaces.Extensions;
using Nomis.EvmosAPI.Interfaces.Models;
using Nomis.EvmosAPI.Responses;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

// ReSharper disable InconsistentNaming

namespace Nomis.EvmosAPI
{
    /// <inheritdoc cref="IEvmosScoringService"/>
    internal sealed class EvmosAPIService :
        IEvmosScoringService,
        ITransientService
    {
        private readonly IEvmosAPIClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="EvmosAPIService"/>.
        /// </summary>
        /// <param name="client"><see cref="IEvmosAPIClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public EvmosAPIService(
            IEvmosAPIClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 9001;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<Result<EvmosWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            string? balanceWei = (await _client.GetBalanceAsync(address)).Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoEvmosUsdPriceResponse>(balanceWei?.ToEvmos() ?? 0, "evmos");
            var transactions = (await _client.GetTransactionsAsync<EvmosAPIAccountNormalTransactions, EvmosAPIAccountNormalTransaction>(address)).ToList();
            var erc20Tokens = (await _client.GetTransactionsAsync<EvmosAPIAccountERC20TokenEvents, EvmosAPIAccountERC20TokenEvent>(address)).ToList();
            var tokenList = (await _client.GetOwnedTokensAsync(address)).ToList();

            var walletStats = new EvmosStatCalculator(
                    address,
                    decimal.TryParse(balanceWei, out decimal wei) ? wei : 0,
                    usdBalance,
                    transactions,
                    tokenList,
                    erc20Tokens)
                .GetStats();

            double score = walletStats.GetScore<EvmosWalletStats, EvmosTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<EvmosWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got evmos wallet score.");
        }
    }
}