// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoAeternityUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.AeternityExplorer.Responses
{
    /// <summary>
    /// Coingecko Aeternity USD price response.
    /// </summary>
    public class CoingeckoAeternityUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("aeternity")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}