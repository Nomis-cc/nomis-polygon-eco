// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Net;
using System.Text.Json;

using Nethereum.Util;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.Domain.Scoring.Entities;
using Nomis.Polygonscan.Calculators;
using Nomis.Polygonscan.Interfaces;
using Nomis.Polygonscan.Interfaces.Enums;
using Nomis.Polygonscan.Interfaces.Extensions;
using Nomis.Polygonscan.Interfaces.Models;
using Nomis.Polygonscan.Responses;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Extensions;
using Nomis.Utils.Wrapper;

namespace Nomis.Polygonscan
{
    /// <inheritdoc cref="IPolygonScoringService"/>
    internal sealed class PolygonscanService :
        IPolygonScoringService,
        ITransientService
    {
        private const string KlimaTokenContractAddress = "0x4e78011Ce80ee02d2c3e649Fb657E45898257815";
        private const string ZroTokenContractAddress = "0x6AcdA5E7EB1117733DC7Cb6158fc67f226b32022";
        private const string BctTokenContractAddress = "0x2f800db0fdb5223b3c3f354886d907a671414a7f";

        private readonly IPolygonscanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="PolygonscanService"/>.
        /// </summary>
        /// <param name="client"><see cref="IPolygonscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public PolygonscanService(
            IPolygonscanClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
        }

        /// <inheritdoc />
        public ulong ChainId => 137;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<Result<PolygonWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            string? balanceWei = (await _client.GetBalanceAsync(address)).Balance;
            decimal usdBalance = await _coingeckoService.GetUsdBalanceAsync<CoingeckoMaticUsdPriceResponse>(balanceWei?.ToMatic() ?? 0, "matic-network");
            await Task.Delay(100, cancellationToken);
            var transactions = (await _client.GetTransactionsAsync<PolygonscanAccountNormalTransactions, PolygonscanAccountNormalTransaction>(address)).ToList();
            await Task.Delay(100, cancellationToken);
            var internalTransactions = (await _client.GetTransactionsAsync<PolygonscanAccountInternalTransactions, PolygonscanAccountInternalTransaction>(address)).ToList();
            await Task.Delay(100, cancellationToken);
            var erc20Tokens = (await _client.GetTransactionsAsync<PolygonscanAccountERC20TokenEvents, PolygonscanAccountERC20TokenEvent>(address)).ToList();
            await Task.Delay(100, cancellationToken);
            var erc721Tokens = (await _client.GetTransactionsAsync<PolygonscanAccountERC721TokenEvents, PolygonscanAccountERC721TokenEvent>(address)).ToList();
            await Task.Delay(100, cancellationToken);
            var erc1155Tokens = (await _client.GetTransactionsAsync<PolygonscanAccountERC1155TokenEvents, PolygonscanAccountERC1155TokenEvent>(address)).ToList();

            var tokens = new List<IPolygonscanAccountNftTokenEvent>();
            tokens.AddRange(erc721Tokens);
            tokens.AddRange(erc1155Tokens);

            var walletStats = new PolygonStatCalculator(
                    address,
                    decimal.TryParse(balanceWei, out decimal wei) ? wei : 0,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    tokens,
                    erc20Tokens)
                .GetStats();

            double score = walletStats.GetScore<PolygonWalletStats, PolygonTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<PolygonWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got polygon wallet score.");
        }

        /// <inheritdoc/>
        public async Task<Result<PolygonWalletEcoScore>> GetWalletEcoStatsAsync(string address, PolygonEcoToken ecoToken, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            string contractAddress = ecoToken switch
            {
                PolygonEcoToken.Klima => KlimaTokenContractAddress,
                PolygonEcoToken.Zro => ZroTokenContractAddress,
                PolygonEcoToken.Bct => BctTokenContractAddress,
                _ => throw new NotImplementedException()
            };

            string coingeckoTokenId = ecoToken.ToDescriptionString();

            string? balanceWei = (await _client.GetTokenBalanceAsync(address, contractAddress)).Balance;
            decimal.TryParse(balanceWei, NumberStyles.AllowDecimalPoint, new NumberFormatInfo { CurrencyDecimalSeparator = "." }, out decimal balance);
            decimal usdBalance = ecoToken switch
            {
                PolygonEcoToken.Klima => await _coingeckoService.GetUsdBalanceAsync<CoingeckoKlimaUsdPriceResponse>(balance.ToEcoToken(ecoToken), coingeckoTokenId),
                PolygonEcoToken.Zro => await _coingeckoService.GetUsdBalanceAsync<CoingeckoZroUsdPriceResponse>(balance.ToEcoToken(ecoToken), coingeckoTokenId),
                PolygonEcoToken.Bct => await _coingeckoService.GetUsdBalanceAsync<CoingeckoBctUsdPriceResponse>(balance.ToEcoToken(ecoToken), coingeckoTokenId),
                _ => throw new NotImplementedException()
            };

            await Task.Delay(100, cancellationToken);
            var erc20Tokens = (await _client.GetTransactionsAsync<PolygonscanAccountERC20TokenEvents, PolygonscanAccountERC20TokenEvent>(address)).ToList();
            erc20Tokens = erc20Tokens.Where(t =>
                t.ContractAddress?.Equals(contractAddress, StringComparison.InvariantCultureIgnoreCase) == true).ToList();

            var walletStats = new PolygonStatCalculator(
                    address,
                    decimal.TryParse(balanceWei, out decimal wei) ? wei : 0,
                    usdBalance,
                    new List<PolygonscanAccountNormalTransaction>(),
                    new List<PolygonscanAccountInternalTransaction>(),
                    new List<IPolygonscanAccountNftTokenEvent>(),
                    erc20Tokens)
                .GetEcoStats(ecoToken);

            double score = walletStats.GetScore<PolygonWalletEcoStats, PolygonTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<PolygonWalletEcoScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got polygon wallet eco score.");
        }
    }
}