// ------------------------------------------------------------------------------------------------------
// <copyright file="CeloscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Celoscan.Settings
{
    /// <summary>
    /// Celoscan settings.
    /// </summary>
    internal class CeloscanSettings :
        ISettings
    {
        /// <summary>
        /// API key for celoscan.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://celoscan.io/apis"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}