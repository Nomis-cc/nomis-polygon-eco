// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoEvmosUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.EvmosAPI.Responses
{
    /// <summary>
    /// Coingecko Evmos USD price response.
    /// </summary>
    public class CoingeckoEvmosUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("evmos")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}