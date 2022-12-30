// ------------------------------------------------------------------------------------------------------
// <copyright file="ITronScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Tronscan.Interfaces.Models;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Tronscan.Interfaces
{
    /// <summary>
    /// Tron scoring service.
    /// </summary>
    public interface ITronScoringService :
        IBlockchainScoringService<TronWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
    }
}