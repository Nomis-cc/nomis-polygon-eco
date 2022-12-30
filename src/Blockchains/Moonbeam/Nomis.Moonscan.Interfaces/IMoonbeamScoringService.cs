// ------------------------------------------------------------------------------------------------------
// <copyright file="IMoonbeamScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Moonscan.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Moonscan.Interfaces
{
    /// <summary>
    /// Moonbeam scoring service.
    /// </summary>
    public interface IMoonbeamScoringService :
        IBlockchainScoringService<MoonbeamWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}