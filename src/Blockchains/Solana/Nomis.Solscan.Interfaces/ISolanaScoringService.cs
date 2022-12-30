// ------------------------------------------------------------------------------------------------------
// <copyright file="ISolanaScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Solscan.Interfaces.Models;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Wrapper;

namespace Nomis.Solscan.Interfaces
{
    /// <summary>
    /// Solana scoring service.
    /// </summary>
    public interface ISolanaScoringService :
        IBlockchainScoringService<SolanaWalletScore>,
        IBlockchainDescriptor,
        IInfrastructureService
    {
        /// <summary>
        /// Get solana wallets stats by addresses.
        /// </summary>
        /// <param name="addresses">List of solana wallet addresses.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>Returns a list of <see cref="SolanaWalletScore"/> result.</returns>
        public Task<Result<List<SolanaWalletScore>>> GetWalletsStatsAsync(List<string> addresses, CancellationToken cancellationToken = default);
    }
}