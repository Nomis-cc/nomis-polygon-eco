// ------------------------------------------------------------------------------------------------------
// <copyright file="SolscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Solscan.Settings
{
    /// <summary>
    /// Solscan settings.
    /// </summary>
    internal class SolscanSettings :
        ISettings
    {
        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://public-api.solscan.io/"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}