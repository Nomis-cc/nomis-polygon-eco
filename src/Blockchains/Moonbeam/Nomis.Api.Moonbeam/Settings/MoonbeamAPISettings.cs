// ------------------------------------------------------------------------------------------------------
// <copyright file="MoonbeamAPISettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Api.Moonbeam.Settings
{
    /// <summary>
    /// Moonbeam API settings.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal class MoonbeamAPISettings :
        IAPISettings
    {
        /// <inheritdoc/>
        public bool APIEnabled { get; set; }

        /// <inheritdoc/>
        public string APIName => MoonbeamController.MoonbeamTag;

        /// <inheritdoc/>
        public string ControllerName => nameof(MoonbeamController);
    }
}