// ------------------------------------------------------------------------------------------------------
// <copyright file="ICubeScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Cubescan.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Cubescan.Interfaces
{
    /// <summary>
    /// Cube scoring service.
    /// </summary>
    public interface ICubeScoringService :
        IBlockchainScoringService<CubeWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}