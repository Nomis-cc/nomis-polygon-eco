// ------------------------------------------------------------------------------------------------------
// <copyright file="BobascanService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net;
using System.Text.Json;

using Nethereum.Util;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Bobascan.Calculators;
using Nomis.Bobascan.Interfaces;
using Nomis.Bobascan.Interfaces.Extensions;
using Nomis.Bobascan.Interfaces.Models;
using Nomis.Bobascan.Responses;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.Bobascan
{
    /// <inheritdoc cref="IBobaScoringService"/>
    internal sealed class BobascanService :
        IBobaScoringService,
        ITransientService
    {
        private readonly IBobascanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="BobascanService"/>.
        /// </summary>
        /// <param name="client"><see cref="IBobascanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public BobascanService(
            IBobascanClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 288;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<Result<BobaWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            string? balanceWei = (await _client.GetBalanceAsync(address)).Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoEthereumUsdPriceResponse>(balanceWei?.ToBoba() ?? 0, "ethereum");
            var transactions = (await _client.GetTransactionsAsync<BobascanAccountNormalTransactions, BobascanAccountNormalTransaction>(address)).ToList();
            var internalTransactions = (await _client.GetTransactionsAsync<BobascanAccountInternalTransactions, BobascanAccountInternalTransaction>(address)).ToList();
            var erc20Tokens = (await _client.GetTransactionsAsync<BobascanAccountERC20TokenEvents, BobascanAccountERC20TokenEvent>(address)).ToList();
            var tokens = (await _client.GetTransactionsAsync<BobascanAccountERC721TokenEvents, BobascanAccountERC721TokenEvent>(address)).ToList();

            var walletStats = new BobaStatCalculator(
                    address,
                    decimal.TryParse(balanceWei, out decimal wei) ? wei : 0,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    tokens,
                    erc20Tokens)
                .GetStats();

            double score = walletStats.GetScore<BobaWalletStats, BobaTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<BobaWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got boba wallet score.");
        }
    }
}