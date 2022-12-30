// ------------------------------------------------------------------------------------------------------
// <copyright file="EvmosHelpers.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.EvmosAPI.Interfaces.Models;

namespace Nomis.EvmosAPI.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for evmos.
    /// </summary>
    public static class EvmosHelpers
    {
        /// <summary>
        /// Convert Wei value to Evmos.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Evmos.</returns>
        public static decimal ToEvmos(this string valueInWei)
        {
            if (!decimal.TryParse(valueInWei, out decimal wei))
            {
                return 0;
            }

            return wei.ToEvmos();
        }

        /// <summary>
        /// Convert Wei value to Evmos.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Evmos.</returns>
        public static decimal ToEvmos(this ulong valueInWei)
        {
            return valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to Evmos.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Evmos.</returns>
        public static decimal ToEvmos(this in BigInteger valueInWei)
        {
            return (decimal)valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to Evmos.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Evmos.</returns>
        public static decimal ToEvmos(this decimal valueInWei)
        {
            return new BigInteger(valueInWei).ToEvmos();
        }

        /// <summary>
        /// Get token UID based on it ContractAddress and Id.
        /// </summary>
        /// <param name="token">Token info.</param>
        /// <returns>Returns token UID.</returns>
        public static string GetTokenUid(this IEvmosAPIAccountNftTokenEvent token)
        {
            return token.ContractAddress + "_" + token.TokenId;
        }
    }
}