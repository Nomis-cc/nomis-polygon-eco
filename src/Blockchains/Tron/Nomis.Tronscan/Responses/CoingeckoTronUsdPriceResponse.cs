// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoTronUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.Tronscan.Responses
{
    /// <summary>
    /// Coingecko Tron USD price response.
    /// </summary>
    public class CoingeckoTronUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("tron")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}