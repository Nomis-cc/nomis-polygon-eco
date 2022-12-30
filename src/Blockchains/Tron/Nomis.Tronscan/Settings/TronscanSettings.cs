// ------------------------------------------------------------------------------------------------------
// <copyright file="TronscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Tronscan.Settings
{
    /// <summary>
    /// Tronscan settings.
    /// </summary>
    internal class TronscanSettings :
        ISettings
    {
        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/tronscan/tronscan-frontend/blob/dev2019/document/api.md"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}