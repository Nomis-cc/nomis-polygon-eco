// ------------------------------------------------------------------------------------------------------
// <copyright file="KlaytnscopeAccountNftTransfers.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.Klaytnscope.Interfaces.Models
{
    /// <summary>
    /// Klaytnscope account NFT token transfers.
    /// </summary>
    public class KlaytnscopeAccountNftTransfers :
        IKlaytnscopeTransferList<KlaytnscopeAccountNftTransfer>
    {
        /// <summary>
        /// Success.
        /// </summary>
        [JsonPropertyName("success")]
        public bool? Success { get; set; }

        /// <summary>
        /// Code.
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// Account NFT token list.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public List<KlaytnscopeAccountNftTransfer>? Result { get; set; } = new();

        /// <summary>
        /// Page.
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; }

        /// <summary>
        /// Limit.
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Total.
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }

        /// <summary>
        /// Account NFT token dictionary.
        /// </summary>
        [JsonPropertyName("tokens")]
        [DataMember(EmitDefaultValue = true)]
        public Dictionary<string, KlaytnscopeAccountNftTokenData>? Tokens { get; set; } = new();
    }
}