// ------------------------------------------------------------------------------------------------------
// <copyright file="ArbitrumOneHelpers.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.Arbiscan.Interfaces.Models;

namespace Nomis.Arbiscan.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for Arbitrum One.
    /// </summary>
    public static class ArbitrumOneHelpers
    {
        /// <summary>
        /// Convert Wei value to ETH.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total ETH.</returns>
        public static decimal ToEth(this string valueInWei)
        {
            if (!decimal.TryParse(valueInWei, out decimal wei))
            {
                return 0;
            }

            return wei.ToEth();
        }

        /// <summary>
        /// Convert Wei value to ETH.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total ETH.</returns>
        public static decimal ToEth(this in BigInteger valueInWei)
        {
            return (decimal)valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to ETH.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total ETH.</returns>
        public static decimal ToEth(this decimal valueInWei)
        {
            return new BigInteger(valueInWei).ToEth();
        }

        /// <summary>
        /// Get token UID based on it ContractAddress and Id.
        /// </summary>
        /// <param name="token">Token info.</param>
        /// <returns>Returns token UID.</returns>
        public static string GetTokenUid(this IArbiscanAccountNftTokenEvent token)
        {
            return token.ContractAddress + "_" + token.TokenId;
        }
    }
}