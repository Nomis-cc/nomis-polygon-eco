// ------------------------------------------------------------------------------------------------------
// <copyright file="SolHelpers.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

namespace Nomis.Solscan.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for solana.
    /// </summary>
    public static class SolHelpers
    {
        /// <summary>
        /// Convert Lamports value to Sol.
        /// </summary>
        /// <param name="valueInLamports">Lamports.</param>
        /// <returns>Returns total Sol.</returns>
        public static decimal ToSol(this long valueInLamports)
        {
            return valueInLamports * 0.000000001M;
        }

        /// <summary>
        /// Convert Lamports value to Sol.
        /// </summary>
        /// <param name="valueInLamports">Lamports.</param>
        /// <returns>Returns total Sol.</returns>
        public static decimal ToSol(this ulong valueInLamports)
        {
            return valueInLamports * 0.000000001M;
        }

        /// <summary>
        /// Convert Lamports value to Sol.
        /// </summary>
        /// <param name="valueInLamports">Lamports.</param>
        /// <returns>Returns total Sol.</returns>
        public static decimal ToSol(this in BigInteger valueInLamports)
        {
            return (decimal)valueInLamports * 0.000000001M;
        }
    }
}