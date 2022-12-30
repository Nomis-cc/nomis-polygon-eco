// ------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nomis.Coingecko.Interfaces;
using Nomis.HederaMirrorNode.Interfaces;
using Nomis.HederaMirrorNode.Settings;
using Nomis.Utils.Extensions;

namespace Nomis.HederaMirrorNode.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add HederaMirrorNode service.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <returns>Returns <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHederaMirrorNodeService(
            this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            serviceProvider.GetRequiredService<ICoingeckoService>();
            services.AddSettings<HederaMirrorNodeSettings>(configuration);
            return services
                .AddTransient<IHederaMirrorNodeClient, HederaMirrorNodeClient>()
                .AddTransientInfrastructureService<IHederaScoringService, HederaMirrorNodeService>();
        }
    }
}