// ------------------------------------------------------------------------------------------------------
// <copyright file="BobaHelpers.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.Bobascan.Interfaces.Models;

namespace Nomis.Bobascan.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for boba.
    /// </summary>
    public static class BobaHelpers
    {
        /// <summary>
        /// Convert Wei value to Boba.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Boba.</returns>
        public static decimal ToBoba(this string valueInWei)
        {
            if (!decimal.TryParse(valueInWei, out decimal wei))
            {
                return 0;
            }

            return wei.ToBoba();
        }

        /// <summary>
        /// Convert Wei value to Boba.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Boba.</returns>
        public static decimal ToBoba(this ulong valueInWei)
        {
            return valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to Boba.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Boba.</returns>
        public static decimal ToBoba(this in BigInteger valueInWei)
        {
            return (decimal)valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to Boba.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Boba.</returns>
        public static decimal ToBoba(this decimal valueInWei)
        {
            return new BigInteger(valueInWei).ToBoba();
        }

        /// <summary>
        /// Get token UID based on it ContractAddress and Id.
        /// </summary>
        /// <param name="token">Token info.</param>
        /// <returns>Returns token UID.</returns>
        public static string GetTokenUid(this IBobascanAccountNftTokenEvent token)
        {
            return token.ContractAddress + "_" + token.TokenId;
        }
    }
}