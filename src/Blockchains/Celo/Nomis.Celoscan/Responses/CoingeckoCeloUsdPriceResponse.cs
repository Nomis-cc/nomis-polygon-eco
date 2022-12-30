// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoCeloUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.Celoscan.Responses
{
    /// <summary>
    /// Coingecko Celo USD price response.
    /// </summary>
    public class CoingeckoCeloUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("celo")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}