// ------------------------------------------------------------------------------------------------------
// <copyright file="IRippleScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Utils.Contracts.Services;
using Nomis.Xrpscan.Interfaces.Models;

namespace Nomis.Xrpscan.Interfaces
{
    /// <summary>
    /// Ripple scoring service.
    /// </summary>
    public interface IRippleScoringService :
        IBlockchainScoringService<RippleWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}