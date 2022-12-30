// ------------------------------------------------------------------------------------------------------
// <copyright file="IBscScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Bscscan.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Bscscan.Interfaces
{
    /// <summary>
    /// BSC scoring service.
    /// </summary>
    public interface IBscScoringService :
        IBlockchainScoringService<BscWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}