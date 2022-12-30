// ------------------------------------------------------------------------------------------------------
// <copyright file="CscExplorerService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Nethereum.Util;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.CscExplorer.Calculators;
using Nomis.CscExplorer.Interfaces;
using Nomis.CscExplorer.Interfaces.Models;
using Nomis.CscExplorer.Responses;
using Nomis.Domain.Scoring.Entities;
using Nomis.ScoringService.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.CscExplorer
{
    /// <inheritdoc cref="ICscScoringService"/>
    internal sealed class CscExplorerService :
        ICscScoringService,
        ITransientService
    {
        private readonly ICscExplorerClient _client;
        private readonly IScoringService _scoringService;
        private readonly HttpClient _coinexClient;

        /// <summary>
        /// Initialize <see cref="CscExplorerService"/>.
        /// </summary>
        /// <param name="client"><see cref="ICscExplorerClient"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        public CscExplorerService(
            ICscExplorerClient client,
            IScoringService scoringService)
        {
            _client = client;
            _scoringService = scoringService;
            _coinexClient = new()
            {
                BaseAddress = new("https://www.coinex.net/")
            };
        }

        /// <inheritdoc />
        public ulong ChainId => 52;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<Result<CscWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            string? balance = (await _client.GetBalanceAsync(address)).Data?.Balance;
            decimal.TryParse(balance, NumberStyles.AllowDecimalPoint, new NumberFormatInfo { CurrencyDecimalSeparator = "." }, out decimal balanceValue);
            decimal usdBalance = await GetUsdBalanceAsync(balanceValue);
            var transactions = (await _client.GetTransactionsAsync<CscExplorerAccountTransactions, CscExplorerAccountTransactionData, CscExplorerAccountTransactionRecord>(address)).ToList();
            var cetTransfers = (await _client.GetTransactionsAsync<CscExplorerAccountCetTransfers, CscExplorerAccountCetTransferData, CscExplorerAccountCetTransferRecord>(address)).ToList();
            var crc20Transfers = (await _client.GetTransactionsAsync<CscExplorerAccountCrc20Transfers, CscExplorerAccountCrc20TransferData, CscExplorerAccountCrc20TransferRecord>(address)).ToList();
            var crc721Transfers = (await _client.GetTransactionsAsync<CscExplorerAccountCrc721Transfers, CscExplorerAccountCrc721TransferData, CscExplorerAccountCrc721TransferRecord>(address)).ToList();

            var walletStats = new CscStatCalculator(
                    address,
                    balanceValue,
                    usdBalance,
                    transactions,
                    cetTransfers,
                    crc20Transfers,
                    crc721Transfers)
                .GetStats();

            double score = walletStats.GetScore<CscWalletStats, CscTransactionIntervalData>();
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<CscWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got CSC wallet score.");
        }

        private async Task<decimal> GetUsdBalanceAsync(decimal balance)
        {
            var response = await _coinexClient.GetAsync("/res/exchange/usd");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<CoinexCscUsdPriceResponse>();
            if (data?.Data?.Any() == true
                && data.Data.ContainsKey("0x0000000000000000000000000000000000000000")
                && decimal.TryParse(data.Data["0x0000000000000000000000000000000000000000"], NumberStyles.AllowDecimalPoint, new NumberFormatInfo { NumberDecimalSeparator = "." }, out decimal usdBalance))
            {
                return balance * usdBalance;
            }

            return 0;
        }
    }
}