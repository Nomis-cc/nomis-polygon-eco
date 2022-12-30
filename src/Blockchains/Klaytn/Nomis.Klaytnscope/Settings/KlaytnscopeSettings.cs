// ------------------------------------------------------------------------------------------------------
// <copyright file="KlaytnscopeSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Klaytnscope.Settings
{
    /// <summary>
    /// Klaytnscope settings.
    /// </summary>
    internal class KlaytnscopeSettings :
        ISettings
    {
        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://docs.klaytn.foundation"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}