// ------------------------------------------------------------------------------------------------------
// <copyright file="CoingeckoHederaUsdPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Coingecko.Interfaces.Models;

namespace Nomis.HederaMirrorNode.Responses
{
    /// <summary>
    /// Coingecko Hedera USD price response.
    /// </summary>
    public class CoingeckoHederaUsdPriceResponse :
        ICoingeckoUsdPriceResponse
    {
        /// <inheritdoc cref="CoingeckoUsdPriceData"/>
        [JsonPropertyName("hedera-hashgraph")]
        public CoingeckoUsdPriceData? Data { get; set; }
    }
}