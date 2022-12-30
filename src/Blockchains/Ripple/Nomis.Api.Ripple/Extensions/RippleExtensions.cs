﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="RippleExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Api.Common.Extensions;
using Nomis.Api.Ripple.Settings;
using Nomis.ScoringService.Interfaces.Builder;
using Nomis.Xrpscan.Interfaces;

namespace Nomis.Api.Ripple.Extensions
{
    /// <summary>
    /// Ripple extension methods.
    /// </summary>
    public static class RippleExtensions
    {
        /// <summary>
        /// Add Ripple blockchain.
        /// </summary>
        /// <typeparam name="TServiceRegistrar">The service registrar type.</typeparam>
        /// <param name="optionsBuilder"><see cref="IScoringOptionsBuilder"/>.</param>
        /// <returns>Returns <see cref="IScoringOptionsBuilder"/>.</returns>
        public static IScoringOptionsBuilder WithRippleBlockchain<TServiceRegistrar>(
            this IScoringOptionsBuilder optionsBuilder)
            where TServiceRegistrar : IRippleServiceRegistrar, new()
        {
            return optionsBuilder
                .With<RippleAPISettings, TServiceRegistrar>();
        }
    }
}