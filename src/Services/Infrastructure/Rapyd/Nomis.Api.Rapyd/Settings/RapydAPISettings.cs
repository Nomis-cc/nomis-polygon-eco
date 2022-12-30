// ------------------------------------------------------------------------------------------------------
// <copyright file="RapydAPISettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Api.Rapyd.Settings
{
    /// <summary>
    /// Rapyd API settings.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal class RapydAPISettings :
        IAPISettings
    {
        /// <inheritdoc/>
        public bool APIEnabled { get; set; }

        /// <inheritdoc/>
        public string APIName => RapydController.RapydTag;

        /// <inheritdoc/>
        public string ControllerName => nameof(RapydController);
    }
}