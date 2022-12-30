// ------------------------------------------------------------------------------------------------------
// <copyright file="KlaytnscopeService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Numerics;
using System.Text.Json;

using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.Klaytnscope.Calculators;
using Nomis.Klaytnscope.Interfaces;
using Nomis.Klaytnscope.Interfaces.Extensions;
using Nomis.Klaytnscope.Interfaces.Models;
using Nomis.Klaytnscope.Responses;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Wrapper;

namespace Nomis.Klaytnscope
{
    /// <inheritdoc cref="IKlaytnScoringService"/>
    internal sealed class KlaytnscopeService :
        IKlaytnScoringService,
        ITransientService
    {
        private readonly IKlaytnscopeClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="KlaytnscopeService"/>.
        /// </summary>
        /// <param name="client"><see cref="IKlaytnscopeClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public KlaytnscopeService(
            IKlaytnscopeClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 8217;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<Result<KlaytnWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            string? hexBalance = (await _client.GetBalanceAsync(address)).Result?.Balance?.Replace("0x", string.Empty);
            BigInteger.TryParse(hexBalance, NumberStyles.HexNumber, new NumberFormatInfo { CurrencyDecimalSeparator = "." }, out var balanceWei);
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoKlaytnUsdPriceResponse>(balanceWei.ToKlay(), "klay-token");
            var kip17Tokens = (await _client.GetTransactionsAsync<KlaytnscopeAccountKIP17TokenEvents, KlaytnscopeAccountKIP17TokenEvent>(address)).ToList();
            var nftTransfers = (await _client.GetTransactionsAsync<KlaytnscopeAccountNftTransfers, KlaytnscopeAccountNftTransfer>(address)).ToList();
            var transactions = (await _client.GetTransactionsAsync<KlaytnscopeAccountNormalTransactions, KlaytnscopeAccountNormalTransaction>(address)).ToList();
            var internalTransactions = (await _client.GetTransactionsAsync<KlaytnscopeAccountInternalTransactions, KlaytnscopeAccountInternalTransaction>(address)).ToList();

            // var kip37Tokens = (await Client.GetTransactionsAsync<KlaytnscopeAccountKIP37TokenEvents, KlaytnscopeAccountKIP37TokenEvent>(address)).ToList();

            var walletStats = new KlaytnStatCalculator(
                    address,
                    (decimal)balanceWei,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    nftTransfers,
                    kip17Tokens)
                .GetStats();

            double score = walletStats.GetScore<KlaytnWalletStats, KlaytnTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<KlaytnWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got klaytn wallet score.");
        }
    }
}