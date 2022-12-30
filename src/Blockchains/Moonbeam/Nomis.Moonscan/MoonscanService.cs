// ------------------------------------------------------------------------------------------------------
// <copyright file="MoonscanService.cs" company="Nomis">
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
using Nomis.Moonscan.Calculators;
using Nomis.Moonscan.Interfaces;
using Nomis.Moonscan.Interfaces.Extensions;
using Nomis.Moonscan.Interfaces.Models;
using Nomis.Moonscan.Responses;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.Moonscan
{
    /// <inheritdoc cref="IMoonbeamScoringService"/>
    internal sealed class MoonscanService :
        IMoonbeamScoringService,
        ITransientService
    {
        private readonly IMoonscanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="MoonscanService"/>.
        /// </summary>
        /// <param name="client"><see cref="IMoonscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public MoonscanService(
            IMoonscanClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 1284;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<Result<MoonbeamWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            string? balanceWei = (await _client.GetBalanceAsync(address)).Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoGlmrUsdPriceResponse>(balanceWei?.ToGlmr() ?? 0, "moonbeam");
            var transactions = (await _client.GetTransactionsAsync<MoonscanAccountNormalTransactions, MoonscanAccountNormalTransaction>(address)).ToList();
            var internalTransactions = (await _client.GetTransactionsAsync<MoonscanAccountInternalTransactions, MoonscanAccountInternalTransaction>(address)).ToList();
            var erc20Tokens = (await _client.GetTransactionsAsync<MoonscanAccountERC20TokenEvents, MoonscanAccountERC20TokenEvent>(address)).ToList();
            var tokens = (await _client.GetTransactionsAsync<MoonscanAccountERC721TokenEvents, MoonscanAccountERC721TokenEvent>(address)).ToList();

            var walletStats = new MoonbeamStatCalculator(
                    address,
                    decimal.TryParse(balanceWei, out decimal wei) ? wei : 0,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    tokens,
                    erc20Tokens)
                .GetStats();

            double score = walletStats.GetScore<MoonbeamWalletStats, MoonbeamTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<MoonbeamWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got moonbeam wallet score.");
        }
    }
}