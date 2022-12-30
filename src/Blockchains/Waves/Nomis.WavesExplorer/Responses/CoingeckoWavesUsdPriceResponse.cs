// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoWavesUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.WavesExplorer.Responses
{
    /// <summary>
    /// Coingecko Waves USD price response.
    /// </summary>
    public class CoingeckoWavesUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("waves")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}