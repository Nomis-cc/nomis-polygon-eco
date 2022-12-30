// ------------------------------------------------------------------------------------------------------
// <copyright file="HapiAPISettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Api.Hapi.Settings
{
    /// <summary>
    /// HAPI API settings.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal class HapiAPISettings :
        IAPISettings
    {
        /// <inheritdoc/>
        public bool APIEnabled { get; set; }

        /// <inheritdoc/>
        public string APIName => HapiController.HapiTag;

        /// <inheritdoc/>
        public string ControllerName => nameof(HapiController);
    }
}