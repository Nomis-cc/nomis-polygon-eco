// ------------------------------------------------------------------------------------------------------
// <copyright file="SolscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json;

using Microsoft.Extensions.Logging;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.MagicEden.Interfaces;
using Nomis.ScoringService.Interfaces;
using Nomis.Solscan.Calculators;
using Nomis.Solscan.Interfaces;
using Nomis.Solscan.Interfaces.Models;
using Nomis.Solscan.Responses;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;
using Solnet.Wallet;

namespace Nomis.Solscan
{
    /// <inheritdoc cref="ISolanaScoringService"/>
    internal sealed class SolscanService :
        ISolanaScoringService,
        ITransientService
    {
        private readonly IMagicEdenClient _magicEdenClient;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;
        private readonly ILogger<SolscanService> _logger;
        private readonly ISolscanClient _client;

        /// <summary>
        /// Initialize <see cref="SolscanService"/>.
        /// </summary>
        /// <param name="magicEdenClient"><see cref="IMagicEdenClient"/>.</param>
        /// <param name="solscanClient"><see cref="ISolscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public SolscanService(
            IMagicEdenClient magicEdenClient,
            ISolscanClient solscanClient,
            ICoingeckoService coingeckoService,
            IScoringService scoringService,
            ILogger<SolscanService> logger)
        {
            _magicEdenClient = magicEdenClient;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
            _logger = logger;
            _client = solscanClient;
        }

        /// <inheritdoc />
        public ulong ChainId => 111111;

        /// <inheritdoc/>
        public bool IsEVMCompatible => false;

        /// <inheritdoc/>
        public async Task<Result<SolanaWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            try
            {
                var publicKey = new PublicKey(address);
                if (!publicKey.IsValid())
                {
                    throw new InvalidAddressException("Invalid address");
                }
            }
            catch (ArgumentException e)
            {
                throw new InvalidAddressException(e.Message);
            }

            decimal balance = await _client.GetBalanceAsync(address);
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoSolUsdPriceResponse>(balance, "solana");
            var magicEdenWalletActivities = (await _magicEdenClient.GetWalletActivitiesData(address)).ToList();
            var splTransfers = await _client.GetTransfersDataAsync<SolscanSplTransferList, SolscanSplTransfer>(address);
            var solTransfers = await _client.GetTransfersDataAsync<SolscanSolTransferList, SolscanSolTransfer>(address);
            var accountTokens = (await _client.GetTokensAsync(address)).ToList();

            var walletStats = new SolanaStatCalculator(
                    address,
                    balance,
                    usdBalance,
                    magicEdenWalletActivities,
                    splTransfers,
                    solTransfers,
                    accountTokens)
                .GetStats();

            double score = walletStats.GetScore<SolanaWalletStats, SolanaTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<SolanaWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got solana wallet score.");
        }

        /// <inheritdoc/>
        public async Task<Result<List<SolanaWalletScore>>> GetWalletsStatsAsync(List<string> addresses, CancellationToken cancellationToken = default)
        {
            int counter = 0;

            // use throttler bc of solscan API limitations
            var throttler = new SemaphoreSlim(5);

            var result = new List<SolanaWalletScore>();
            var tasks = addresses
                .Select(async address =>
                {
                    await throttler.WaitAsync(cancellationToken);

                    var task = GetWalletStatsAsync(address, cancellationToken);
                    _ = task.ContinueWith(
                        async _ =>
                        {
                            Interlocked.Increment(ref counter);
                            _logger.LogWarning("{Counter} - Stats for {Wallet} calculated!", counter, address);

                            await Task.Delay(10, cancellationToken);
                            throttler.Release();
                        }, cancellationToken);

                    try
                    {
                        return await task;
                    }
                    catch (HttpRequestException)
                    {
                        return await task;
                    }
                });
            var statsResults = await Task.WhenAll(tasks);
            result.AddRange(statsResults.Where(r => r.Succeeded).Select(r => r.Data));

            return await Result<List<SolanaWalletScore>>.SuccessAsync(result, "Got solana wallets score.");
        }
    }
}