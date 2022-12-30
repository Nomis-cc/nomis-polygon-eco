// ------------------------------------------------------------------------------------------------------
// <copyright file="IPolygonScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Polygonscan.Interfaces.Enums;
using Nomis.Polygonscan.Interfaces.Models;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Wrapper;

namespace Nomis.Polygonscan.Interfaces
{
    /// <summary>
    /// Polygon scoring service.
    /// </summary>
    public interface IPolygonScoringService :
        IBlockchainScoringService<PolygonWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
        /// <summary>
        /// Get blockchain wallet eco stats by address.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        /// <param name="ecoToken">Eco token.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>Returns the wallet eco score result.</returns>
        Task<Result<PolygonWalletEcoScore>> GetWalletEcoStatsAsync(string address, PolygonEcoToken ecoToken, CancellationToken cancellationToken = default);
    }
}