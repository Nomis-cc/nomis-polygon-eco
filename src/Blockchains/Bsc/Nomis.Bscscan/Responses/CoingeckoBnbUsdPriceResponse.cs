// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoBnbUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.Bscscan.Responses
{
    /// <summary>
    /// Coingecko BNB USD price response.
    /// </summary>
    public class CoingeckoBnbUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("binancecoin")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}