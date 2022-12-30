// ------------------------------------------------------------------------------------------------------
// <copyright file="BscscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net;
using System.Text.Json;

using EthScanNet.Lib;
using EthScanNet.Lib.Models.EScan;
using Microsoft.Extensions.Options;
using Nethereum.Util;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Bscscan.Calculators;
using Nomis.Bscscan.Interfaces;
using Nomis.Bscscan.Interfaces.Extensions;
using Nomis.Bscscan.Interfaces.Models;
using Nomis.Bscscan.Responses;
using Nomis.Bscscan.Settings;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.Bscscan
{
    /// <inheritdoc cref="IBscScoringService"/>
    internal sealed class BscscanService :
        IBscScoringService,
        ITransientService
    {
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;
        private readonly EScanClient _client;

        /// <summary>
        /// Initialize <see cref="BscscanService"/>.
        /// </summary>
        /// <param name="settings"><see cref="BscscanSettings"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public BscscanService(
            IOptions<BscscanSettings> settings,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
            _client = new(EScanNetwork.BscMainNet, settings.Value.ApiKey);
        }

        /// <inheritdoc />
        public ulong ChainId => 56;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<Result<BscWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            var bscAddress = new EScanAddress(address);
            var balanceWei = (await _client.Accounts.GetBalanceAsync(bscAddress)).Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoBnbUsdPriceResponse>(balanceWei.ToBnb(), "binancecoin");
            var transactions = (await _client.Accounts.GetNormalTransactionsAsync(bscAddress)).Transactions;
            var internalTransactions = (await _client.Accounts.GetInternalTransactionsAsync(bscAddress)).Transactions; // await _client.GetInternalTransactionsAsync(address);
            var tokens = (await _client.Accounts.GetTokenEvents(bscAddress)).TokenTransferEvents;
            var erc20Tokens = (await _client.Accounts.GetERC20TokenEvents(bscAddress)).ERC20TokenTransferEvents;

            var walletStats = new BscStatCalculator(
                    address,
                    balanceWei,
                    usdBalance,
                    transactions.Select(x => new WrappedEScanTransaction(x)),
                    internalTransactions.Select(x => new WrappedEScanTransaction(x)),
                    tokens,
                    erc20Tokens)
                .GetStats();

            double score = walletStats.GetScore<BscWalletStats, BscTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<BscWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got BSC wallet score.");
        }
    }
}