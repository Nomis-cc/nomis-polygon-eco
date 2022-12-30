// ------------------------------------------------------------------------------------------------------
// <copyright file="IEvmosAPITransferList.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.EvmosAPI.Interfaces.Models
{
    /// <summary>
    /// EvmosAPI transfer list.
    /// </summary>
    /// <typeparam name="TListItem">EvmosAPI transfer.</typeparam>
    public interface IEvmosAPITransferList<TListItem>
        where TListItem : IEvmosAPITransfer
    {
        /// <summary>
        /// List of transfers.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public List<TListItem> Data { get; set; }
    }
}