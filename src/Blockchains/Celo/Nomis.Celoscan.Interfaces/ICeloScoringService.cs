// ------------------------------------------------------------------------------------------------------
// <copyright file="ICeloScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Celoscan.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Celoscan.Interfaces
{
    /// <summary>
    /// Celo scoring service.
    /// </summary>
    public interface ICeloScoringService :
        IBlockchainScoringService<CeloWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}