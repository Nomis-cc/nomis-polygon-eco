// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoGlmrUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.Moonscan.Responses
{
    /// <summary>
    /// Coingecko GLMR USD price response.
    /// </summary>
    public class CoingeckoGlmrUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("moonbeam")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}