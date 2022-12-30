// ------------------------------------------------------------------------------------------------------
// <copyright file="IAeternityScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.AeternityExplorer.Interfaces.Models;
using Nomis.Blockchain.Abstractions;
using Nomis.Utils.Contracts.Services;

namespace Nomis.AeternityExplorer.Interfaces
{
    /// <summary>
    /// Aeternity scoring service.
    /// </summary>
    public interface IAeternityScoringService :
        IBlockchainScoringService<AeternityWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}