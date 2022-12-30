// ------------------------------------------------------------------------------------------------------
// <copyright file="CubescanService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net;
using System.Text.Json;

using Nethereum.Util;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Cubescan.Calculators;
using Nomis.Cubescan.Interfaces;
using Nomis.Cubescan.Interfaces.Extensions;
using Nomis.Cubescan.Interfaces.Models;
using Nomis.Cubescan.Responses;
using Nomis.Domain.Scoring.Entities;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.Cubescan
{
    /// <inheritdoc cref="ICubeScoringService"/>
    internal sealed class CubescanService :
        ICubeScoringService,
        ITransientService
    {
        private readonly ICubescanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="CubescanService"/>.
        /// </summary>
        /// <param name="client"><see cref="ICubescanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public CubescanService(
            ICubescanClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 1818;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<Result<CubeWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            string? balanceWei = (await _client.GetBalanceAsync(address)).Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoCubeUsdPriceResponse>(balanceWei?.ToCube() ?? 0, "cube-network");
            var transactions = (await _client.GetTransactionsAsync<CubescanAccountNormalTransactions, CubescanAccountNormalTransaction>(address)).ToList();
            var internalTransactions = (await _client.GetTransactionsAsync<CubescanAccountInternalTransactions, CubescanAccountInternalTransaction>(address)).ToList();
            var erc20Tokens = new List<CubescanAccountERC20TokenEvent>(); // (await Client.GetTransactionsAsync<CubescanAccountERC20TokenEvents, CubescanAccountERC20TokenEvent>(address)).ToList(); // TODO - 504 http status from API
            var tokens = (await _client.GetTransactionsAsync<CubescanAccountERC721TokenEvents, CubescanAccountERC721TokenEvent>(address)).ToList();

            var walletStats = new CubeStatCalculator(
                    address,
                    decimal.TryParse(balanceWei, out decimal wei) ? wei : 0,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    tokens,
                    erc20Tokens)
                .GetStats();

            double score = walletStats.GetScore<CubeWalletStats, CubeTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<CubeWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got cube wallet score.");
        }
    }
}