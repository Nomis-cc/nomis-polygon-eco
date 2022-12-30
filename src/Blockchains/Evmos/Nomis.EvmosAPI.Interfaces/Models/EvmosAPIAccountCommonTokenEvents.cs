﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="EvmosAPIAccountCommonTokenEvents.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.EvmosAPI.Interfaces.Models
{
    /// <summary>
    /// EvmosAPI account common token transfer events.
    /// </summary>
    public class EvmosAPIAccountCommonTokenEvents
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
        /// Account common token event list.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public List<EvmosAPIAccountCommonTokenEvent> Data { get; set; } = new();
    }
}