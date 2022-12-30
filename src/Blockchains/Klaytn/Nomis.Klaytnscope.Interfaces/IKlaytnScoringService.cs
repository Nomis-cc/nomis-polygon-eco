// ------------------------------------------------------------------------------------------------------
// <copyright file="IKlaytnScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Klaytnscope.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Klaytnscope.Interfaces
{
    /// <summary>
    /// Klaytn scoring service.
    /// </summary>
    public interface IKlaytnScoringService :
        IBlockchainScoringService<KlaytnWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}