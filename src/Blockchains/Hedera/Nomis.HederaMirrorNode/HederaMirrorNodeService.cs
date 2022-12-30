// ------------------------------------------------------------------------------------------------------
// <copyright file="HederaMirrorNodeService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json;

using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.HederaMirrorNode.Calculators;
using Nomis.HederaMirrorNode.Interfaces;
using Nomis.HederaMirrorNode.Interfaces.Extensions;
using Nomis.HederaMirrorNode.Interfaces.Models;
using Nomis.HederaMirrorNode.Responses;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Wrapper;

namespace Nomis.HederaMirrorNode
{
    /// <inheritdoc cref="IHederaScoringService"/>
    internal sealed class HederaMirrorNodeService :
        IHederaScoringService,
        ITransientService
    {
        private readonly IHederaMirrorNodeClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="HederaMirrorNodeService"/>.
        /// </summary>
        /// <param name="client"><see cref="IHederaMirrorNodeClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public HederaMirrorNodeService(
            IHederaMirrorNodeClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 111113;

        /// <inheritdoc/>
        public bool IsEVMCompatible => false;

        /// <inheritdoc/>
        public async Task<Result<HederaWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            var accountData = await _client.GetBalanceAsync(address);
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoHederaUsdPriceResponse>((accountData.Balance?.Balance ?? 0).ToHbars(), "hedera-hashgraph");
            var transactions = accountData.Transactions;
            var internalTransactions = accountData.Transactions.SelectMany(x => x.Transfers);
            var nfts = await _client.GetNftsAsync(address);
            var nftTransactions = new List<HederaMirrorNodeNftTransactionData>();
            foreach (var nft in nfts.Nfts)
            {
                nftTransactions.AddRange((await _client.GetNftTransactionsAsync(nft.TokenId!, nft.SerialNumber)).Transactions);
            }

            var walletStats = new HederaStatCalculator(
                    address,
                    accountData,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    nfts.Nfts,
                    nftTransactions)
                .GetStats();

            double score = walletStats.GetScore<HederaWalletStats, HederaTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<HederaWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got hedera wallet score.");
        }
    }
}