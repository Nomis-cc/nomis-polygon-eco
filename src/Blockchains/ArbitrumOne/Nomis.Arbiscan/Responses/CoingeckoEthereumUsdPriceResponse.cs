// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoEthereumUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.Arbiscan.Responses
{
    /// <summary>
    /// Coingecko Ethereum USD price response.
    /// </summary>
    public class CoingeckoEthereumUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("ethereum")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}