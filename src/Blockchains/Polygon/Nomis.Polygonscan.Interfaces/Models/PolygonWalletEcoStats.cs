﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonWalletEcoStats.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

using Nomis.Blockchain.Abstractions.Models;
using Nomis.Blockchain.Abstractions.Stats;

namespace Nomis.Polygonscan.Interfaces.Models
{
    /// <summary>
    /// Polygon wallet eco stats.
    /// </summary>
    public class PolygonWalletEcoStats :
        IWalletCommonStats<PolygonTransactionIntervalData>,
        IWalletBalanceStats,
        IWalletTransactionStats,
        IWalletNftStats,
        IWalletTokenStats
    {
        /// <inheritdoc/>
        public bool NoData { get; init; }

        /// <summary>
        /// Eco token.
        /// </summary>
        [Display(Description = "Eco token used for calculating", GroupName = "value")]
        public string? EcoToken { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Wallet balance", GroupName = "Eco token")]
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
        [Display(Description = "The movement of funds on the wallet", GroupName = "Eco token")]
        public decimal WalletTurnover { get; set; }

        /// <inheritdoc/>
        public IEnumerable<PolygonTransactionIntervalData>? TurnoverIntervals { get; set; }

        /// <inheritdoc/>
        [Display(Description = "The balance change value in the last month", GroupName = "Eco token")]
        public decimal BalanceChangeInLastMonth { get; set; }

        /// <inheritdoc/>
        [Display(Description = "The balance change value in the last year", GroupName = "Eco token")]
        public decimal BalanceChangeInLastYear { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Total NFTs on wallet", GroupName = "number")]
        public int NftHolding { get; set; }

        /// <inheritdoc/>
        [Display(Description = "Time since last transaction", GroupName = "months")]
        public int TimeFromLastTransaction { get; set; }

        /// <inheritdoc/>
        [Display(Description = "NFT trading activity", GroupName = "Eco token")]
        public decimal NftTrading { get; set; }

        /// <inheritdoc/>
        [Display(Description = "NFT worth on wallet", GroupName = "Eco token")]
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
        public Dictionary<string, PropertyData> StatsDescriptions => GetType()
            .GetProperties()
            .Where(prop => !IWalletCommonStats<PolygonTransactionIntervalData>.ExcludedStatDescriptions.Contains(prop.Name) && Attribute.IsDefined(prop, typeof(DisplayAttribute)))
            .ToDictionary(p => p.Name, p => new PropertyData(p));
    }
}