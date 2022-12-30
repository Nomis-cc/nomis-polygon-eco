﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="IMoonscanTransfer.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.Moonscan.Interfaces.Models
{
    /// <summary>
    /// Moonscan transfer.
    /// </summary>
    public interface IMoonscanTransfer
    {
        /// <summary>
        /// Block number.
        /// </summary>
        [JsonPropertyName("blockNumber")]
        public string? BlockNumber { get; set; }
    }
}