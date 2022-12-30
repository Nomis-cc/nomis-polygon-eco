// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonWalletEcoScore.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;

namespace Nomis.Polygonscan.Interfaces.Models
{
    /// <summary>
    /// Polygon wallet eco score.
    /// </summary>
    public class PolygonWalletEcoScore :
        IWalletScore<PolygonWalletEcoStats, PolygonTransactionIntervalData>
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
        /// Additional stat data used in eco score calculations.
        /// </summary>
        public PolygonWalletEcoStats? Stats { get; set; }
    }
}