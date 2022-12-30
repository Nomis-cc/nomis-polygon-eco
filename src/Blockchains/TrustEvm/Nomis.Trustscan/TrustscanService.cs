// ------------------------------------------------------------------------------------------------------
// <copyright file="TrustscanService.cs" company="Nomis">
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
using Nomis.ScoringService.Interfaces;
using Nomis.Trustscan.Calculators;
using Nomis.Trustscan.Interfaces;
using Nomis.Trustscan.Interfaces.Extensions;
using Nomis.Trustscan.Interfaces.Models;
using Nomis.Trustscan.Responses;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.Trustscan
{
    /// <inheritdoc cref="ITrustEvmScoringService"/>
    internal sealed class TrustscanService :
        ITrustEvmScoringService,
        ITransientService
    {
        private readonly ITrustscanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="TrustscanService"/>.
        /// </summary>
        /// <param name="client"><see cref="ITrustscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public TrustscanService(
            ITrustscanClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 15555;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<Result<TrustEvmWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            string? balanceWei = (await _client.GetBalanceAsync(address)).Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoEthereumUsdPriceResponse>(balanceWei?.ToEth() ?? 0, "ethereum");
            var transactions = (await _client.GetTransactionsAsync<TrustscanAccountNormalTransactions, TrustscanAccountNormalTransaction>(address)).ToList();
            var erc20Tokens = (await _client.GetTransactionsAsync<TrustscanAccountERC20TokenEvents, TrustscanAccountERC20TokenEvent>(address)).ToList();

            var walletStats = new TrustEvmStatCalculator(
                    address,
                    decimal.TryParse(balanceWei, out decimal wei) ? wei : 0,
                    usdBalance,
                    transactions,
                    erc20Tokens)
                .GetStats();

            double score = walletStats.GetScore<TrustEvmWalletStats, TrustEvmTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<TrustEvmWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got Trust EVM wallet score.");
        }
    }
}