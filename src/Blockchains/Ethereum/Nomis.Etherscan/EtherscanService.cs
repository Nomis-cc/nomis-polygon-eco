// ------------------------------------------------------------------------------------------------------
// <copyright file="EtherscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using EthScanNet.Lib;
using EthScanNet.Lib.Models.EScan;
using Microsoft.Extensions.Options;
using Nethereum.ENS;
using Nethereum.Util;
using Nethereum.Web3;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Domain.Scoring.Entities;
using Nomis.Etherscan.Calculators;
using Nomis.Etherscan.Interfaces;
using Nomis.Etherscan.Interfaces.Extensions;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Etherscan.Interfaces.Requests;
using Nomis.Etherscan.Interfaces.Responses;
using Nomis.Etherscan.Settings;
using Nomis.HapiExplorer.Interfaces;
using Nomis.HapiExplorer.Interfaces.Responses;
using Nomis.ScoringService.Interfaces;
using Nomis.Snapshot.Interfaces;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.Etherscan
{
    /// <inheritdoc cref="IEthereumScoringService"/>
    internal sealed class EtherscanService :
        IEthereumScoringService,
        ITransientService
    {
        private readonly IScoringService _scoringService;
        private readonly ISnapshotService _snapshotService;
        private readonly IHapiExplorerService _hapiExplorerService;
        private readonly EtherscanSettings _settings;
        private readonly Web3 _nethereumClient;
        private readonly HttpClient _coinbaseClient;
        private readonly EScanClient _client;

        /// <summary>
        /// Initialize <see cref="EtherscanService"/>.
        /// </summary>
        /// <param name="settings"><see cref="EtherscanSettings"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        /// <param name="snapshotService"><see cref="ISnapshotService"/>.</param>
        /// <param name="hapiExplorerService"><see cref="IHapiExplorerService"/>.</param>
        public EtherscanService(
            IOptions<EtherscanSettings> settings,
            IScoringService scoringService,
            ISnapshotService snapshotService,
            IHapiExplorerService hapiExplorerService)
        {
            _scoringService = scoringService;
            _snapshotService = snapshotService;
            _hapiExplorerService = hapiExplorerService;
            _settings = settings.Value;
            _client = new(EScanNetwork.MainNet, settings.Value.ApiKey);

            _nethereumClient = new(settings.Value.BlockchainProviderUrl)
            {
                TransactionManager =
                {
                    DefaultGasPrice = new(0x4c4b40), // TODO - remove to settings
                    DefaultGas = new(0x4c4b40)
                }
            };

            _coinbaseClient = new()
            {
                BaseAddress = new("https://api.coinbase.com/")
            };
        }

        /// <inheritdoc />
        public ulong ChainId => 1;

        /// <inheritdoc/>
        public bool IsEVMCompatible => true;

        /// <inheritdoc/>
        public async Task<T?> CallReadFunctionAsync<T>(EthereumCallReadFunctionRequest request)
        {
            if (!new AddressUtil().IsValidAddressLength(request.ContractAddress))
            {
                throw new CustomException("Invalid contract address", statusCode: HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.Abi))
            {
                throw new CustomException("ABI must be set", statusCode: HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.FunctionName))
            {
                throw new CustomException("Function name must be set", statusCode: HttpStatusCode.BadRequest);
            }

            var contract = _nethereumClient.Eth.GetContract(request.Abi, request.ContractAddress);
            var function = contract.GetFunction(request.FunctionName);

            var result = await function.CallAsync<T>(request.Parameters);
            return result ?? default;
        }

        /// <inheritdoc/>
        public async Task<Result<EthereumWalletScore>> GetWalletStatsAsync(string address, CancellationToken cancellationToken = default)
        {
            if (address.EndsWith(".eth", StringComparison.CurrentCultureIgnoreCase))
            {
                var web3 = new Web3(_settings.BlockchainProviderUrl);
                address = await new ENSService(web3).ResolveAddressAsync(address);
            }

            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            var ethAddress = new EScanAddress(address);
            var balanceWei = (await _client.Accounts.GetBalanceAsync(ethAddress)).Balance;
            decimal balanceUsd = await GetUsdBalanceAsync(balanceWei.ToEth());
            var transactions = (await _client.Accounts.GetNormalTransactionsAsync(ethAddress)).Transactions;
            var internalTransactions = (await _client.Accounts.GetInternalTransactionsAsync(ethAddress)).Transactions;
            var tokens = (await _client.Accounts.GetTokenEvents(ethAddress)).TokenTransferEvents;
            var erc20Tokens = (await _client.Accounts.GetERC20TokenEvents(ethAddress)).ERC20TokenTransferEvents;

            // Snapshot protocol
            var snapshotVotes = (await _snapshotService.GetSnapshotVotesAsync(new()
            {
                Voter = address
            })).Data;
            var snapshotProposals = (await _snapshotService.GetSnapshotProposalsAsync(new()
            {
                Author = address
            })).Data;

            // HAPI protocol
            HapiProxyRiskScoreResponse? hapiRiskScore = null;
            try
            {
                hapiRiskScore = (await _hapiExplorerService.GetWalletRiskScoreAsync("ethereum", address)).Data;
            }
            catch (NoDataException)
            {
                // ignored
            }

            var walletStats = new EthereumStatCalculator(
                    address,
                    balanceWei,
                    balanceUsd,
                    transactions,
                    internalTransactions,
                    tokens,
                    erc20Tokens,
                    snapshotVotes,
                    snapshotProposals,
                    hapiRiskScore)
                .GetStats();

            double score = AdjustScore(walletStats.GetScore<EthereumWalletStats, EthereumTransactionIntervalData>(), hapiRiskScore);
            var scoringData = new ScoringData(address, address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken);

            return await Result<EthereumWalletScore>.SuccessAsync(
                new()
                {
                    Address = address,
                    Stats = walletStats,
                    Score = score
                }, "Got ethereum wallet score.");
        }

        /// <summary>
        /// Adjust Nomis score.
        /// </summary>
        /// <remarks>
        /// <see href="https://hapi-one.gitbook.io/hapi-protocol/hapi-core-of-decentralized-cybersecurity/risk-assessment"/>
        /// </remarks>
        /// <param name="score">Calculated Nomis score.</param>
        /// <param name="hapiRiskScore"><see cref="HapiProxyRiskScoreResponse"/>.</param>
        /// <returns>Returns adjusted Nomis score value.</returns>
        private double AdjustScore(
            double score,
            HapiProxyRiskScoreResponse? hapiRiskScore)
        {
            if (hapiRiskScore?.Address == null)
            {
                return score;
            }

            return hapiRiskScore.Address.Risk switch
            {
                >= 10 => 0,
                >= 5 => score * 0.6,
                >= 1 => score * 0.8,
                _ => score
            };
        }

        private async Task<decimal> GetUsdBalanceAsync(decimal balance)
        {
            var response = await _coinbaseClient.GetAsync("/v2/prices/ETH-USD/spot");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<CoinbaseSpotPriceResponse>() ?? throw new CustomException("Can't get USD balance.");

            if (decimal.TryParse(data.Data?.Amount, NumberStyles.AllowDecimalPoint, new NumberFormatInfo() { CurrencyDecimalSeparator = "." }, out decimal decimalAmount))
            {
                return balance * decimalAmount;
            }

            return 0;
        }
    }
}