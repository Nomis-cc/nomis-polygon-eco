// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoMaticUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.Polygonscan.Responses
{
    /// <summary>
    /// Coingecko Matic USD price response.
    /// </summary>
    public class CoingeckoMaticUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("matic-network")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}