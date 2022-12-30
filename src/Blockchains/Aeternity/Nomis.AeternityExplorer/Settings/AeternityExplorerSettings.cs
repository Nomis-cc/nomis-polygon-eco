// ------------------------------------------------------------------------------------------------------
// <copyright file="AeternityExplorerSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.AeternityExplorer.Settings
{
    /// <summary>
    /// AeternityExplorer settings.
    /// </summary>
    internal class AeternityExplorerSettings :
        ISettings
    {
        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/aeternity/ae_mdw#hosted-infrastructure"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}