// ------------------------------------------------------------------------------------------------------
// <copyright file="AeternityExplorerService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json;

using Nomis.AeternityExplorer.Calculators;
using Nomis.AeternityExplorer.Interfaces;
using Nomis.AeternityExplorer.Interfaces.Extensions;
using Nomis.AeternityExplorer.Interfaces.Models;
using Nomis.AeternityExplorer.Responses;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Wrapper;

namespace Nomis.AeternityExplorer
{
    /// <inheritdoc cref="IAeternityScoringService"/>
    internal sealed class AeternityExplorerService :
        IAeternityScoringService,
        ITransientService
    {
        private readonly IAeternityExplorerClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="AeternityExplorerService"/>.
        /// </summary>
        /// <param name="client"><see cref="IAeternityExplorerClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public AeternityExplorerService(
            IAeternityExplorerClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc/>
        public ulong ChainId => 111112;

        /// <inheritdoc/>
        public bool IsEVMCompatible => false;

        /// <inheritdoc/>
        public async Task<Result<AeternityWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            var balanceWei = (await _client.GetBalanceAsync(address)).Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoAeternityUsdPriceResponse>(balanceWei.ToAeternity(), "aeternity");
            var transactions = (await _client.GetTransactionsAsync<AeternityExplorerAccountNormalTransactions, AeternityExplorerAccountNormalTransaction>(address)).ToList();
            var internalTransactions = (await _client.GetTransactionsAsync<AeternityExplorerAccountInternalTransactions, AeternityExplorerAccountInternalTransaction>(address)).ToList();
            var aex9FromTokens = (await _client.GetTransactionsAsync<AeternityExplorerAccountAEX9TokenEvents, AeternityExplorerAccountAEX9TokenEvent>(address, true)).ToList();
            var aex9ToTokens = (await _client.GetTransactionsAsync<AeternityExplorerAccountAEX9TokenEvents, AeternityExplorerAccountAEX9TokenEvent>(address, false)).ToList();
            var aex9Tokens = aex9FromTokens.Union(aex9ToTokens).ToList();
            var aex141FromTokens = (await _client.GetTransactionsAsync<AeternityExplorerAccountAEX141TokenEvents, AeternityExplorerAccountAEX141TokenEvent>(address, true)).ToList();
            var aex141ToTokens = (await _client.GetTransactionsAsync<AeternityExplorerAccountAEX141TokenEvents, AeternityExplorerAccountAEX141TokenEvent>(address, false)).ToList();
            var aex141Tokens = aex141FromTokens.Union(aex141ToTokens).ToList();

            var walletStats = new AeternityStatCalculator(
                    address,
                    (decimal)balanceWei,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    aex141Tokens,
                    aex9Tokens)
                .GetStats();

            double score = walletStats.GetScore<AeternityWalletStats, AeternityTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<AeternityWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got aeternity wallet score.");
        }
    }
}