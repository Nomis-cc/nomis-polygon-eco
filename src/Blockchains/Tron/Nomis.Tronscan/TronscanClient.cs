// ------------------------------------------------------------------------------------------------------
// <copyright file="TronscanClient.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net.Http.Json;

using Microsoft.Extensions.Options;
using Nomis.Tronscan.Interfaces;
using Nomis.Tronscan.Interfaces.Models;
using Nomis.Tronscan.Settings;
using Nomis.Utils.Exceptions;

namespace Nomis.Tronscan
{
    /// <inheritdoc cref="ITronscanClient"/>
    internal sealed class TronscanClient :
        ITronscanClient
    {
        // private const int ItemsFetchLimit = 10000;
        private readonly TronscanSettings _tronscanSettings;

        private readonly HttpClient _client;

        /// <summary>
        /// Initialize <see cref="TronscanClient"/>.
        /// </summary>
        /// <param name="tronscanSettings"><see cref="TronscanSettings"/>.</param>
        public TronscanClient(
            IOptions<TronscanSettings> tronscanSettings)
        {
            _tronscanSettings = tronscanSettings.Value;
            _client = new()
            {
                BaseAddress = new(tronscanSettings.Value.ApiBaseUrl ??
                                  throw new ArgumentNullException(nameof(tronscanSettings.Value.ApiBaseUrl)))
            };
        }

        /// <inheritdoc/>
        public async Task<TronscanAccount> GetBalanceAsync(string address)
        {
            string request =
                $"/api/account?address={address}";

            var response = await _client.GetAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TronscanAccount>() ?? throw new CustomException("Can't get account balance.");
        }

        /// <inheritdoc/>
        public async Task<TronscanAccountContracts> GetContractsAsync(string address)
        {
            string request =
                $"/api/contracts?count=true&limit=20&owner={address}&start=0&sort=-trxCount";

            var response = await _client.GetAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TronscanAccountContracts>() ?? throw new CustomException("Can't get account contracts.");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TResultItem>> GetTransactionsAsync<TResult, TResultItem>(string address)
            where TResult : ITronscanTransferList<TResultItem>
            where TResultItem : ITronscanTransfer
        {
            var result = new List<TResultItem>();
            long start = 0;
            var transactionsData = await GetTransactionListAsync<TResult>(address, start);
            result.AddRange(transactionsData.Data ?? new List<TResultItem>());
            while ((transactionsData?.Data?.Count > 0 && result.Count < /* ItemsFetchLimit)*/ transactionsData.RangeTotal) || transactionsData?.RangeTotal == 0) // TODO - too long processing
            {
                if (transactionsData.RangeTotal != 0)
                {
                    Thread.Sleep(100);
                    start++;
                }

                transactionsData = await GetTransactionListAsync<TResult>(address, start * 50);
                result.AddRange(transactionsData?.Data ?? new List<TResultItem>());
            }

            return result;
        }

        private async Task<TResult> GetTransactionListAsync<TResult>(
            string address,
            long start = 0)
        {
            string request = "/api";

            if (typeof(TResult) == typeof(TronscanAccountNormalTransactions))
            {
                request = $"{request}/transaction";
            }
            else if (typeof(TResult) == typeof(TronscanAccountInternalTransactions))
            {
                request = $"{request}/internal-transaction";
            }
            else if (typeof(TResult) == typeof(TronscanAccountTransfers))
            {
                request = $"{request}/transfer";
            }
            else
            {
                return default!;
            }

            request = $"{request}?address={address}&sort=-timestamp&count=true&limit=50&start={start}";

            var response = await _client.GetAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>() ?? throw new CustomException("Can't get account transactions.");
        }
    }
}