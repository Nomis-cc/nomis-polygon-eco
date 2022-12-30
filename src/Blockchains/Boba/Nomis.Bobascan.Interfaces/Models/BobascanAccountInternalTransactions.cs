﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="BobascanAccountInternalTransactions.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.Bobascan.Interfaces.Models
{
    /// <summary>
    /// Bobascan account internal transactions.
    /// </summary>
    public class BobascanAccountInternalTransactions :
        IBobascanTransferList<BobascanAccountInternalTransaction>
    {
        /// <summary>
        /// Status.
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }

        /// <summary>
        /// Message.
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Account internal transaction list.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public List<BobascanAccountInternalTransaction> Data { get; set; } = new();
    }
}