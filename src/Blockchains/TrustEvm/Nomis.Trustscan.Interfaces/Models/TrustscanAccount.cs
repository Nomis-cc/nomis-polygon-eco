﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="TrustscanAccount.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.Trustscan.Interfaces.Models
{
    /// <summary>
    /// Trustscan account.
    /// </summary>
    public class TrustscanAccount
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
        /// Balance.
        /// </summary>
        [JsonPropertyName("result")]
        public string? Balance { get; set; }
    }
}