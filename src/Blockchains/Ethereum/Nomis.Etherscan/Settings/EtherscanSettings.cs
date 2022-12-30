// ------------------------------------------------------------------------------------------------------
// <copyright file="EtherscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Etherscan.Settings
{
    /// <summary>
    /// Etherscan settings.
    /// </summary>
    internal class EtherscanSettings :
        ISettings
    {
        /// <summary>
        /// API key for etherscan.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Blockchain provider URL.
        /// </summary>
        public string? BlockchainProviderUrl { get; set; }

        /// <summary>
        /// Ethereum API is enabled.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool APIEnabled { get; set; }
    }
}