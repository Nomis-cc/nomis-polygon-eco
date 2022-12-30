// ------------------------------------------------------------------------------------------------------
// <copyright file="EvmosWalletScore.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;

namespace Nomis.EvmosAPI.Interfaces.Models
{
    /// <summary>
    /// Evmos wallet score.
    /// </summary>
    public class EvmosWalletScore :
        IWalletScore<EvmosWalletStats, EvmosTransactionIntervalData>
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
        public EvmosWalletStats? Stats { get; set; }
    }
}