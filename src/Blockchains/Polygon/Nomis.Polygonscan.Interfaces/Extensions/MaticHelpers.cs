// ------------------------------------------------------------------------------------------------------
// <copyright file="MaticHelpers.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.Polygonscan.Interfaces.Enums;
using Nomis.Polygonscan.Interfaces.Models;

namespace Nomis.Polygonscan.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for polygon.
    /// </summary>
    public static class MaticHelpers
    {
        /// <summary>
        /// Convert Wei value to Matic.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Matic.</returns>
        public static decimal ToMatic(this string valueInWei)
        {
            if (!ulong.TryParse(valueInWei, out ulong wei))
            {
                return 0;
            }

            return new BigInteger(wei).ToMatic();
        }

        /// <summary>
        /// Convert Wei value to Matic.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Matic.</returns>
        public static decimal ToMatic(this ulong valueInWei)
        {
            return new BigInteger(valueInWei).ToMatic();
        }

        /// <summary>
        /// Convert Wei value to Matic.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Matic.</returns>
        public static decimal ToMatic(this in BigInteger valueInWei)
        {
            return (decimal)valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to Matic.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Matic.</returns>
        public static decimal ToMatic(this decimal valueInWei)
        {
            return new BigInteger(valueInWei).ToMatic();
        }

        /// <summary>
        /// Convert Wei value to Eco token.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <param name="ecoToken"><see cref="PolygonEcoToken"/>.</param>
        /// <returns>Returns total Eco token.</returns>
        public static decimal ToEcoToken(this decimal valueInWei, PolygonEcoToken ecoToken)
        {
            return ecoToken switch
            {
                PolygonEcoToken.Klima => valueInWei * 0.000_000_001M,
                PolygonEcoToken.Zro => valueInWei * 0.000_000_000_000_000_001M,
                PolygonEcoToken.Bct => valueInWei * 0.000_000_000_000_000_001M,
                _ => throw new InvalidOperationException()
            };
        }

        /// <summary>
        /// Get token UID based on it ContractAddress and Id.
        /// </summary>
        /// <param name="token">Token info.</param>
        /// <returns>Returns token UID.</returns>
        public static string GetTokenUid(this IPolygonscanAccountNftTokenEvent token)
        {
            return token.ContractAddress + "_" + token.TokenId;
        }
    }
}