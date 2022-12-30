// ------------------------------------------------------------------------------------------------------
// <copyright file="AeternityExplorer.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nomis.AeternityExplorer.Extensions;
using Nomis.AeternityExplorer.Interfaces;
using Nomis.Coingecko.Interfaces;

namespace Nomis.AeternityExplorer
{
    /// <summary>
    /// AeternityExplorer service registrar.
    /// </summary>
    public sealed class AeternityExplorer :
        IAeternityServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            serviceProvider.GetRequiredService<ICoingeckoService>();
            return services
                .AddAeternityExplorerService(configuration);
        }
    }
}