﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="ITrustscanTransfer.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.Trustscan.Interfaces.Models
{
    /// <summary>
    /// Trustscan transfer.
    /// </summary>
    public interface ITrustscanTransfer
    {
        /// <summary>
        /// Block number.
        /// </summary>
        [JsonPropertyName("blockNumber")]
        public string? BlockNumber { get; set; }
    }
}