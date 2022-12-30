// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoKlaytnUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.Klaytnscope.Responses
{
    /// <summary>
    /// Coingecko Klaytn USD price response.
    /// </summary>
    public class CoingeckoKlaytnUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("klay-token")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}