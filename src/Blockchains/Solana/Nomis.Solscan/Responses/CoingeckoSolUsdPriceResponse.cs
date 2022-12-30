// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoSolUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.Solscan.Responses
{
    /// <summary>
    /// Coingecko SOL USD price response.
    /// </summary>
    public class CoingeckoSolUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("solana")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}