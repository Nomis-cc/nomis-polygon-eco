// ------------------------------------------------------------------------------------------------------
// <copyright file="INearScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Nearblocks.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Nearblocks.Interfaces
{
    /// <summary>
    /// Near scoring service.
    /// </summary>
    public interface INearScoringService :
        IBlockchainScoringService<NearWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}