// ------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nomis.Etherscan.Interfaces;
using Nomis.Etherscan.Settings;
using Nomis.HapiExplorer.Interfaces;
using Nomis.Snapshot.Interfaces;
using Nomis.Utils.Extensions;

namespace Nomis.Etherscan.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Etherscan service.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <returns>Returns <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddEtherscanService(
            this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            services.AddSettings<EtherscanSettings>(configuration);
            serviceProvider.GetRequiredService<ISnapshotService>();
            serviceProvider.GetRequiredService<IHapiExplorerService>();
            return services
                .AddTransientInfrastructureService<IEthereumScoringService, EtherscanService>();
        }
    }
}