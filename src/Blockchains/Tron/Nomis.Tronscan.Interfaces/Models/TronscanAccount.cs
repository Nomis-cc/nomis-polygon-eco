// ------------------------------------------------------------------------------------------------------
// <copyright file="TronscanAccount.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.Tronscan.Interfaces.Models
{
    /// <summary>
    /// Tronscan account.
    /// </summary>
    public class TronscanAccount
    {
        /// <summary>
        /// The account TRC20 balances.
        /// </summary>
        [JsonPropertyName("trc20token_balances")]
        public List<TronscanAccountTrc20TokenBalance>? Trc20Balances { get; set; } = new();

        /// <summary>
        /// The account out transactions.
        /// </summary>
        [JsonPropertyName("transactions_out")]
        public long TransactionsOut { get; set; }

        /// <summary>
        /// The account in transactions.
        /// </summary>
        [JsonPropertyName("transactions_in")]
        public long TransactionsIn { get; set; }

        /// <summary>
        /// The account total transactions.
        /// </summary>
        [JsonPropertyName("totalTransactionCount")]
        public long TotalTransactionCount { get; set; }

        /// <summary>
        /// The account transactions.
        /// </summary>
        [JsonPropertyName("transactions")]
        public long Transactions { get; set; }

        /// <summary>
        /// The account reward num.
        /// </summary>
        [JsonPropertyName("rewardNum")]
        public long RewardNum { get; set; }

        /// <summary>
        /// The account reward.
        /// </summary>
        [JsonPropertyName("reward")]
        public long Reward { get; set; }

        /// <summary>
        /// The account token balances.
        /// </summary>
        [JsonPropertyName("tokenBalances")]
        public List<TronscanAccountTokenBalance>? TokenBalances { get; set; } = new();

        /// <summary>
        /// The account balances.
        /// </summary>
        [JsonPropertyName("balances")]
        public List<TronscanAccountBalance>? Balances { get; set; } = new();

        /// <summary>
        /// The account TRC721 balances.
        /// </summary>
        [JsonPropertyName("trc721token_balances")]
        public List<TronscanAccountTrc721TokenBalance>? Trc721Balances { get; set; } = new();

        /// <summary>
        /// The account balance.
        /// </summary>
        [JsonPropertyName("balance")]
        public long Balance { get; set; }

        /// <summary>
        /// The account vote total count.
        /// </summary>
        [JsonPropertyName("voteTotal")]
        public long VoteTotal { get; set; }

        /// <summary>
        /// The account total frozen.
        /// </summary>
        [JsonPropertyName("totalFrozen")]
        public long TotalFrozen { get; set; }

        /// <summary>
        /// The account tokens.
        /// </summary>
        [JsonPropertyName("tokens")]
        public List<TronscanAccountToken>? Tokens { get; set; } = new();

        /// <summary>
        /// The account address.
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        /// <summary>
        /// The account created date.
        /// </summary>
        [JsonPropertyName("date_created")]
        public long DateCreated { get; set; }

        /// <summary>
        /// The account type.
        /// </summary>
        [JsonPropertyName("accountType")]
        public long AccountType { get; set; }

        /// <summary>
        /// The account name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}