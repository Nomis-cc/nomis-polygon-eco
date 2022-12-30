// ------------------------------------------------------------------------------------------------------
// <copyright file="ITrustEvmScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Trustscan.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Trustscan.Interfaces
{
    /// <summary>
    /// Trust EVM scoring service.
    /// </summary>
    public interface ITrustEvmScoringService :
        IBlockchainScoringService<TrustEvmWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}