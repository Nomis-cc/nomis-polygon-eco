// ------------------------------------------------------------------------------------------------------
// <copyright file="BscscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Bscscan.Settings
{
    /// <summary>
    /// Bscscan settings.
    /// </summary>
    internal class BscscanSettings :
        ISettings
    {
        /// <summary>
        /// API key for Bscscan.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Bscscan API base URL.
        /// </summary>
        public string? ApiBaseUrl { get; set; }
    }
}