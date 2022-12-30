// ------------------------------------------------------------------------------------------------------
// <copyright file="IEvmosScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.EvmosAPI.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.EvmosAPI.Interfaces
{
    /// <summary>
    /// Evmos scoring service.
    /// </summary>
    public interface IEvmosScoringService :
        IBlockchainScoringService<EvmosWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}