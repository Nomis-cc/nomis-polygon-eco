// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Polygonscan.Settings
{
    /// <summary>
    /// Polygonscan settings.
    /// </summary>
    internal class PolygonscanSettings :
        ISettings
    {
        /// <summary>
        /// API key for polygonscan.
        /// </summary>
        /// <remarks>
        /// <see href="https://docs.polygonscan.com/"/>
        /// </remarks>
        public string? ApiKey { get; set; }

        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://docs.polygonscan.com/getting-started/endpoint-urls"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}