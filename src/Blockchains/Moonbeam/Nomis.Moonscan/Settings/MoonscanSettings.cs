// ------------------------------------------------------------------------------------------------------
// <copyright file="MoonscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Moonscan.Settings
{
    /// <summary>
    /// Moonscan settings.
    /// </summary>
    internal class MoonscanSettings :
        ISettings
    {
        /// <summary>
        /// API key for moonscan.
        /// </summary>
        /// <remarks>
        /// <see href="https://moonbeam.moonscan.io/apis"/>
        /// </remarks>
        public string? ApiKey { get; set; }

        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://moonbeam.moonscan.io/apis#accounts"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }

        /// <summary>
        /// Moonbeam API is enabled.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool APIEnabled { get; set; }
    }
}