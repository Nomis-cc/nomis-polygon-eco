// ------------------------------------------------------------------------------------------------------
// <copyright file="WavesExplorerService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json;

using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Wrapper;
using Nomis.WavesExplorer.Calculators;
using Nomis.WavesExplorer.Interfaces;
using Nomis.WavesExplorer.Interfaces.Extensions;
using Nomis.WavesExplorer.Interfaces.Models;
using Nomis.WavesExplorer.Responses;

namespace Nomis.WavesExplorer
{
    /// <inheritdoc cref="IWavesScoringService"/>
    internal sealed class WavesExplorerService :
        IWavesScoringService,
        ITransientService
    {
        private readonly IWavesExplorerClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="WavesExplorerService"/>.
        /// </summary>
        /// <param name="client"><see cref="IWavesExplorerClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public WavesExplorerService(
            IWavesExplorerClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 111118;

        /// <inheritdoc/>
        public bool IsEVMCompatible => false;

        /// <inheritdoc/>
        public async Task<Result<WavesWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            var balanceWei = (await _client.GetBalanceAsync(address)).RegularBalance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoWavesUsdPriceResponse>(balanceWei.ToWaves(), "waves");
            var transactions = (await _client.GetTransactionsAsync(address)).ToList();
            var assets = (await _client.GetAssetsAsync(address)).Balances;
            var nftAssets = (await _client.GetNftAssetsAsync(address)).ToList();

            var walletStats = new WavesStatCalculator(
                    address,
                    (decimal)balanceWei,
                    usdBalance,
                    transactions,
                    assets,
                    nftAssets)
                .GetStats();

            double score = walletStats.GetScore<WavesWalletStats, WavesTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<WavesWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got Waves wallet score.");
        }
    }
}