// ------------------------------------------------------------------------------------------------------
// <copyright file="CeloHelpers.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.Celoscan.Interfaces.Models;

namespace Nomis.Celoscan.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for celo.
    /// </summary>
    public static class CeloHelpers
    {
        /// <summary>
        /// Convert Wei value to Celo.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Celo.</returns>
        public static decimal ToCelo(this string valueInWei)
        {
            if (!decimal.TryParse(valueInWei, out decimal wei))
            {
                return 0;
            }

            return wei.ToCelo();
        }

        /// <summary>
        /// Convert Wei value to Celo.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Celo.</returns>
        public static decimal ToCelo(this ulong valueInWei)
        {
            return valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to Celo.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Celo.</returns>
        public static decimal ToCelo(this in BigInteger valueInWei)
        {
            return (decimal)valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to Celo.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Celo.</returns>
        public static decimal ToCelo(this decimal valueInWei)
        {
            return new BigInteger(valueInWei).ToCelo();
        }

        /// <summary>
        /// Get token UID based on it ContractAddress and Id.
        /// </summary>
        /// <param name="token">Token info.</param>
        /// <returns>Returns token UID.</returns>
        public static string GetTokenUid(this ICeloscanAccountNftTokenEvent token)
        {
            return token.ContractAddress + "_" + token.TokenId;
        }
    }
}