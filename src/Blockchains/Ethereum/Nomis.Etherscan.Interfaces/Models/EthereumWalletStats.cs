// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumWalletStats.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Nomis.Blockchain.Abstractions.Models;
using Nomis.Blockchain.Abstractions.Stats;
using Nomis.HapiExplorer.Interfaces.Responses;

// ReSharper disable InconsistentNaming
namespace Nomis.Etherscan.Interfaces.Models
{
    /// <summary>
    /// Ethereum wallet stats.
    /// </summary>
    public class EthereumWalletStats :
        IWalletCommonStats<EthereumTransactionIntervalData>,
        IWalletBalanceStats,
        IWalletTransactionStats,
        IWalletNftStats,
        IWalletTokenStats,
        IWalletContractStats,
        IWalletSnapshotStats
    {
        /// <inheritdoc/>
        public bool NoData { get; init; }

        /// <inheritdoc/>
        [Display(Description = "Amount of deployed smart-contracts", GroupName = "number")]
        public int DeployedContracts { get; init; }

        /// <inheritdoc/>
        [Display(Description = "Wallet balance", GroupName = "ETH")]
        public decimal Balance { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Wallet balance", GroupName = "USD")]
        public decimal BalanceUSD { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Wallet age", GroupName = "months")]
        public int WalletAge { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Total transactions on wallet", GroupName = "number")]
        public int TotalTransactions { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Total rejected transactions on wallet", GroupName = "number")]
        public int TotalRejectedTransactions { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Average time interval between transactions", GroupName = "hours")]
        public double AverageTransactionTime { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Maximum time interval between transactions", GroupName = "hours")]
        public double MaxTransactionTime { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Minimal time interval between transactions", GroupName = "hours")]
        public double MinTransactionTime { get; set; }

        /// <inheritdoc/>
        [Display(Description = "The movement of funds on the wallet", GroupName = "ETH")]
        public decimal WalletTurnover { get; set; }

        /// <inheritdoc/>
        public IEnumerable<EthereumTransactionIntervalData>? TurnoverIntervals { get; set; }

        /// <inheritdoc/>
        [Display(Description = "The balance change value in the last month", GroupName = "ETH")]
        public decimal BalanceChangeInLastMonth { get; set; }

        /// <inheritdoc/>
        [Display(Description = "The balance change value in the last year", GroupName = "ETH")]
        public decimal BalanceChangeInLastYear { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Total NFTs on wallet", GroupName = "number")]
        public int NftHolding { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Time since last transaction", GroupName = "months")]
        public int TimeFromLastTransaction { get; set; }

        /// <inheritdoc/>
        [Display(Description = "NFT trading activity", GroupName = "ETH")]
        public decimal NftTrading { get; set; }

        /// <inheritdoc/>
        [Display(Description = "NFT worth on wallet", GroupName = "ETH")]
        public decimal NftWorth { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Last month transactions", GroupName = "number")]
        public int LastMonthTransactions { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Last year transactions on wallet", GroupName = "number")]
        public int LastYearTransactions { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Average transaction per months", GroupName = "number")]
        public double TransactionsPerMonth => WalletAge != 0 ? (double)TotalTransactions / WalletAge : 0;

        /// <inheritdoc/>
        [Display(Description = "Value of all holding tokens", GroupName = "number")]
        public int TokensHolding { get; set; }

        /// <inheritdoc/>
        [Display(Description = "The Snapshot protocol votes", GroupName = "collection")]
        public IEnumerable<SnapshotProtocolVoteData>? SnapshotVotes { get; set; } = new List<SnapshotProtocolVoteData>();

        /// <inheritdoc/>
        [Display(Description = "The Snapshot protocol proposals", GroupName = "collection")]
        public IEnumerable<SnapshotProtocolProposalData>? SnapshotProposals { get; set; } = new List<SnapshotProtocolProposalData>();

        /// <summary>
        /// The HAPI protocol risk score.
        /// </summary>
        [Display(Description = "The HAPI protocol risk score data", GroupName = "value")]
        public HapiProxyRiskScoreResponse? HapiRiskScore { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, PropertyData> StatsDescriptions => GetType()
            .GetProperties()
            .Where(prop => !ExcludedStatDescriptions.Contains(prop.Name) && Attribute.IsDefined(prop, typeof(DisplayAttribute)))
            .ToDictionary(p => p.Name, p => new PropertyData(p));

        /// <inheritdoc/>
        [JsonIgnore]
        public IEnumerable<Func<double>> AdditionalScores => new List<Func<double>>
        {
            () => (this as IWalletSnapshotStats).GetScore()
        };

        /// <inheritdoc cref="IWalletCommonStats{ITransactionIntervalData}.ExcludedStatDescriptions"/>
        [JsonIgnore]
        public IEnumerable<string> ExcludedStatDescriptions =>
            IWalletCommonStats<EthereumTransactionIntervalData>.ExcludedStatDescriptions.Union(new List<string>
            {
                nameof(SnapshotProposals),
                nameof(SnapshotVotes),
                nameof(HapiRiskScore)
            });
    }
}