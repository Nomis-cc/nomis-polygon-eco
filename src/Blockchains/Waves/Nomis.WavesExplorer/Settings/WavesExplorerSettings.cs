// ------------------------------------------------------------------------------------------------------
// <copyright file="WavesExplorerSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.WavesExplorer.Settings
{
    /// <summary>
    /// Waves Explorer settings.
    /// </summary>
    internal class WavesExplorerSettings :
        ISettings
    {
        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://docs.waves.tech/en/waves-node/node-api/#api-of-pool-of-public-nodes"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}