// ------------------------------------------------------------------------------------------------------
// <copyright file="IHederaScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.HederaMirrorNode.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.HederaMirrorNode.Interfaces
{
    /// <summary>
    /// Hedera scoring service.
    /// </summary>
    public interface IHederaScoringService :
        IBlockchainScoringService<HederaWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}