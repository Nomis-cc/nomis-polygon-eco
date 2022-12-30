// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoCubeUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.Cubescan.Responses
{
    /// <summary>
    /// Coingecko Cube USD price response.
    /// </summary>
    public class CoingeckoCubeUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("cube-network")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}