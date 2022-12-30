// ------------------------------------------------------------------------------------------------------
// <copyright file="ITrustscanTransferList.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.Trustscan.Interfaces.Models
{
    /// <summary>
    /// Trustscan transfer list.
    /// </summary>
    /// <typeparam name="TListItem">Trustscan transfer.</typeparam>
    public interface ITrustscanTransferList<TListItem>
        where TListItem : ITrustscanTransfer
    {
        /// <summary>
        /// List of transfers.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public List<TListItem> Data { get; set; }
    }
}