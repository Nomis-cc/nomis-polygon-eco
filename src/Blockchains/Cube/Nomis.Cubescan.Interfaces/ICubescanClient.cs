﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="ICubescanClient.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Cubescan.Interfaces.Models;

namespace Nomis.Cubescan.Interfaces
{
    /// <summary>
    /// Cubescan client.
    /// </summary>
    public interface ICubescanClient
    {
        /// <summary>
        /// Get the account balance in Wei.
        /// </summary>
        /// <param name="address">Account address.</param>
        /// <returns>Returns <see cref="CubescanAccount"/>.</returns>
        Task<CubescanAccount> GetBalanceAsync(string address);

        /// <summary>
        /// Get list of specific transactions/transfers of the given account.
        /// </summary>
        /// <typeparam name="TResult">The type of returned response.</typeparam>
        /// <typeparam name="TResultItem">The type of returned response data items.</typeparam>
        /// <param name="address">Account address.</param>
        /// <returns>Returns list of specific transactions/transfers of the given account.</returns>
        Task<IEnumerable<TResultItem>> GetTransactionsAsync<TResult, TResultItem>(string address)
            where TResult : ICubescanTransferList<TResultItem>
            where TResultItem : ICubescanTransfer;
    }
}