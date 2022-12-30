// ------------------------------------------------------------------------------------------------------
// <copyright file="CscExplorerSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.CscExplorer.Settings
{
    /// <summary>
    /// CscExplorer settings.
    /// </summary>
    internal class CscExplorerSettings :
        ISettings
    {
        /// <summary>
        /// API key for CSC Explorer.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// CscExplorer API base URL.
        /// </summary>
        public string? ApiBaseUrl { get; set; }
    }
}