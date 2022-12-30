// ------------------------------------------------------------------------------------------------------
// <copyright file="XrpscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Xrpscan.Settings
{
    /// <summary>
    /// Xrpscan settings.
    /// </summary>
    internal class XrpscanSettings :
        ISettings
    {
        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://docs.xrpscan.com/api-doc.html"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}