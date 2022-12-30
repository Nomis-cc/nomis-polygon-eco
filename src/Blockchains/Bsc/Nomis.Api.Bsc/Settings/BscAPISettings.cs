// ------------------------------------------------------------------------------------------------------
// <copyright file="BscAPISettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Api.Bsc.Settings
{
    /// <summary>
    /// BSC API settings.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal class BscAPISettings :
        IAPISettings
    {
        /// <inheritdoc/>
        public bool APIEnabled { get; set; }

        /// <inheritdoc/>
        public string APIName => BscController.BscTag;

        /// <inheritdoc/>
        public string ControllerName => nameof(BscController);
    }
}