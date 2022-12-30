// ------------------------------------------------------------------------------------------------------
// <copyright file="KlaytnHelpers.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.Klaytnscope.Interfaces.Models;

namespace Nomis.Klaytnscope.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for klaytn.
    /// </summary>
    public static class KlaytnHelpers
    {
        /// <summary>
        /// Convert Wei value to KLAY.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total KLAY.</returns>
        public static decimal ToKlay(this string valueInWei)
        {
            if (!decimal.TryParse(valueInWei, out decimal wei))
            {
                return 0;
            }

            return wei.ToKlay();
        }

        /// <summary>
        /// Convert Wei value to KLAY.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total KLAY.</returns>
        public static decimal ToKlay(this long valueInWei)
        {
            return valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to KLAY.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total KLAY.</returns>
        public static decimal ToKlay(this in BigInteger valueInWei)
        {
            return (decimal)valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to KLAY.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total KLAY.</returns>
        public static decimal ToKlay(this decimal valueInWei)
        {
            return new BigInteger(valueInWei).ToKlay();
        }

        /// <summary>
        /// Get token UID based on it ParentHash and Id.
        /// </summary>
        /// <param name="token">Token info.</param>
        /// <returns>Returns token UID.</returns>
        public static string GetTokenUid(this KlaytnscopeAccountNftTransfer token)
        {
            return token.TokenAddress + "_" + token.TokenId;
        }
    }
}