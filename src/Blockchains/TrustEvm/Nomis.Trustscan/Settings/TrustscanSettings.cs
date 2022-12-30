// ------------------------------------------------------------------------------------------------------
// <copyright file="TrustscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Trustscan.Settings
{
    /// <summary>
    /// Trustscan settings.
    /// </summary>
    internal class TrustscanSettings :
        ISettings
    {
        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://trustscan.one/api-docs"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}