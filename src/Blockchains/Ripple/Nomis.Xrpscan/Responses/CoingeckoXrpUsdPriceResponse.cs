// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoXrpUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.Xrpscan.Responses
{
    /// <summary>
    /// Coingecko XRP USD price response.
    /// </summary>
    public class CoingeckoXrpUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("ripple")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}