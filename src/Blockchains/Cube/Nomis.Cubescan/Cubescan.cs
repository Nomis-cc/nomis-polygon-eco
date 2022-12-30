﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="Cubescan.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Nomis.Cubescan.Extensions;
using Nomis.Cubescan.Interfaces;

namespace Nomis.Cubescan
{
    /// <summary>
    /// Cubescan service registrar.
    /// </summary>
    public sealed class Cubescan :
        ICubeServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            return services
                .AddCubescanService();
        }
    }
}