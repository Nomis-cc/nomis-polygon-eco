// ------------------------------------------------------------------------------------------------------
// <copyright file="EvmosEVMSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.EvmosAPI.Settings
{
    /// <summary>
    /// EvmosEVM settings.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal class EvmosEVMSettings :
        ISettings
    {
        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://evm.evmos.org/api-docs"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}