// ------------------------------------------------------------------------------------------------------
// <copyright file="TronWalletScore.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;

namespace Nomis.Tronscan.Interfaces.Models
{
    /// <summary>
    /// Tron wallet score.
    /// </summary>
    public class TronWalletScore :
        IWalletScore<TronWalletStats, TronTransactionIntervalData>
    {
        /// <summary>
        /// Wallet address.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Nomis Score in range of [0; 1].
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Additional stat data used in score calculations.
        /// </summary>
        public TronWalletStats? Stats { get; set; }
    }
}