﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="GlmrHelpers.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.Moonscan.Interfaces.Models;

namespace Nomis.Moonscan.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for moonbeam.
    /// </summary>
    public static class GlmrHelpers
    {
        /// <summary>
        /// Convert Wei value to Glmr.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Glmr.</returns>
        public static decimal ToGlmr(this string valueInWei)
        {
            if (!decimal.TryParse(valueInWei, out decimal wei))
            {
                return 0;
            }

            return wei.ToGlmr();
        }

        /// <summary>
        /// Convert Wei value to Glmr.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Glmr.</returns>
        public static decimal ToGlmr(this ulong valueInWei)
        {
            return valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to Glmr.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Glmr.</returns>
        public static decimal ToGlmr(this in BigInteger valueInWei)
        {
            return (decimal)valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to Glmr.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Glmr.</returns>
        public static decimal ToGlmr(this decimal valueInWei)
        {
            return new BigInteger(valueInWei).ToGlmr();
        }

        /// <summary>
        /// Get token UID based on it ContractAddress and Id.
        /// </summary>
        /// <param name="token">Token info.</param>
        /// <returns>Returns token UID.</returns>
        public static string GetTokenUid(this IMoonscanAccountNftTokenEvent token)
        {
            return token.ContractAddress + "_" + token.TokenId;
        }
    }
}