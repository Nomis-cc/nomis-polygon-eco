// ------------------------------------------------------------------------------------------------------
// <copyright file="XrpscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Wrapper;
using Nomis.Xrpscan.Calculators;
using Nomis.Xrpscan.Interfaces;
using Nomis.Xrpscan.Interfaces.Models;
using Nomis.Xrpscan.Responses;

namespace Nomis.Xrpscan
{
    /// <inheritdoc cref="XrpscanService"/>
    internal sealed class XrpscanService :
        IRippleScoringService,
        ITransientService
    {
        private readonly IXrpscanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;
        private readonly ILogger<XrpscanService> _logger;

        /// <summary>
        /// Initialize <see cref="XrpscanService"/>.
        /// </summary>
        /// <param name="client"><see cref="IXrpscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public XrpscanService(
            IXrpscanClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService,
            ILogger<XrpscanService> logger)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
            _logger = logger;
        }

        /// <inheritdoc />
        public ulong ChainId => 111114;

        /// <inheritdoc/>
        public bool IsEVMCompatible => false;

        /// <inheritdoc/>
        public async Task<Result<RippleWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            var account = await _client.GetAccountDataAsync(address);
            decimal.TryParse(account.XrpBalance, NumberStyles.AllowDecimalPoint, new NumberFormatInfo { CurrencyDecimalSeparator = "." }, out decimal balance);

            var kycStatus = await _client.GetKycDataAsync(address);
            var assets = await _client.GetAssetsDataAsync(address);
            var orders = await _client.GetOrdersDataAsync(address);
            var obligations = await _client.GetObligationsDataAsync(address);
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoXrpUsdPriceResponse>(balance, "ripple");
            var transactions = await _client.GetTransactionsDataAsync(address);
            var nfts = await _client.GetNftsDataAsync(address);

            var walletStats = new RippleStatCalculator(
                    account,
                    balance,
                    usdBalance,
                    kycStatus,
                    assets,
                    orders,
                    obligations,
                    transactions,
                    nfts)
                .GetStats();

            double score = walletStats.GetScore<RippleWalletStats, RippleTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<RippleWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got ripple wallet score.");
        }
    }
}