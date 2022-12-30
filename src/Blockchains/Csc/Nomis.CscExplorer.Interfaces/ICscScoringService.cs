// ------------------------------------------------------------------------------------------------------
// <copyright file="ICscScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.CscExplorer.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.CscExplorer.Interfaces
{
    /// <summary>
    /// CSC scoring service.
    /// </summary>
    public interface ICscScoringService :
        IBlockchainScoringService<CscWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}