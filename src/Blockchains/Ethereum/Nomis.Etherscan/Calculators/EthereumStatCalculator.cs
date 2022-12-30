// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumStatCalculator.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using EthScanNet.Lib.Models.ApiResponses.Accounts.Models;
using Nomis.Blockchain.Abstractions.Calculators;
using Nomis.Blockchain.Abstractions.Models;
using Nomis.Etherscan.Interfaces.Extensions;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.HapiExplorer.Interfaces.Responses;
using Nomis.Snapshot.Interfaces.Models;
using Nomis.Utils.Extensions;

namespace Nomis.Etherscan.Calculators
{
    /// <summary>
    /// Ethereum wallet stats calculator.
    /// </summary>
    internal sealed class EthereumStatCalculator :
        IStatCalculator<EthereumWalletStats, EthereumTransactionIntervalData>
    {
        private readonly string _address;
        private readonly BigInteger _balance;
        private readonly decimal _balanceUsd;
        private readonly IEnumerable<EScanTransaction> _transactions;
        private readonly IEnumerable<EScanTransaction> _internalTransactions;
        private readonly IEnumerable<EScanTokenTransferEvent> _tokenTransfers;
        private readonly IEnumerable<EScanTokenTransferEvent> _erc20TokenTransfers;
        private readonly IEnumerable<SnapshotVote> _snapshotVotes;
        private readonly IEnumerable<SnapshotProposal> _snapshotProposals;
        private readonly HapiProxyRiskScoreResponse? _hapiRiskScore;

        public EthereumStatCalculator(
            string address,
            BigInteger balance,
            decimal balanceUsd,
            IEnumerable<EScanTransaction> transactions,
            IEnumerable<EScanTransaction> internalTransactions,
            IEnumerable<EScanTokenTransferEvent> tokenTransfers,
            IEnumerable<EScanTokenTransferEvent> erc20TokenTransfers,
            IEnumerable<SnapshotVote> snapshotVotes,
            IEnumerable<SnapshotProposal> snapshotProposals,
            HapiProxyRiskScoreResponse? hapiRiskScore)
        {
            _address = address;
            _balance = balance;
            _balanceUsd = balanceUsd;
            _transactions = transactions;
            _internalTransactions = internalTransactions;
            _tokenTransfers = tokenTransfers;
            _erc20TokenTransfers = erc20TokenTransfers;
            _snapshotVotes = snapshotVotes;
            _snapshotProposals = snapshotProposals;
            _hapiRiskScore = hapiRiskScore;
        }

        public EthereumWalletStats GetStats()
        {
            if (!_transactions.Any())
            {
                return new()
                {
                    NoData = true
                };
            }

            var intervals = IStatCalculator
                .GetTransactionsIntervals(_transactions.Select(x => x.TimeStamp!.ToDateTime())).ToList();
            if (intervals.Count == 0)
            {
                return new()
                {
                    NoData = true
                };
            }

            var monthAgo = DateTime.Now.AddMonths(-1);
            var yearAgo = DateTime.Now.AddYears(-1);

            var soldTokens = _tokenTransfers.Where(x => x.From?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true).ToList();
            var soldSum = IStatCalculator
                .GetTokensSum(soldTokens.Select(x => x.Hash), _internalTransactions.Select(x => (x.Hash, x.Value)));

            var soldTokensIds = soldTokens.Select(x => x.GetTokenUid());
            var buyTokens = _tokenTransfers.Where(x => x.To?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true && soldTokensIds.Contains(x.GetTokenUid()));
            var buySum = IStatCalculator
                .GetTokensSum(buyTokens.Select(x => x.Hash), _internalTransactions.Select(x => (x.Hash, x.Value)));

            var buyNotSoldTokens = _tokenTransfers.Where(x => x.To?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true && !soldTokensIds.Contains(x.GetTokenUid()));
            var buyNotSoldSum = IStatCalculator
                .GetTokensSum(buyNotSoldTokens.Select(x => x.Hash), _internalTransactions.Select(x => (x.Hash, x.Value)));

            int holdingTokens = _tokenTransfers.Count() - soldTokens.Count;
            decimal nftWorth = buySum == 0 ? 0 : (decimal)soldSum / (decimal)buySum * (decimal)buyNotSoldSum;
            int contractsCreated = _transactions.Count(x => !string.IsNullOrWhiteSpace(x.ContractAddress));
            var totalTokens = _erc20TokenTransfers.Select(x => x.TokenSymbol).Distinct();

            var turnoverIntervalsDataList =
                _transactions.Select(x => new TurnoverIntervalsData(
                    x.TimeStamp.ToDateTime(),
                    x.Value,
                    x.From?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true));
            var turnoverIntervals = IStatCalculator<EthereumTransactionIntervalData>
                .GetTurnoverIntervals(turnoverIntervalsDataList, _transactions.Min(x => x.TimeStamp.ToDateTime())).ToList();

            var snapshotVotesData = _snapshotVotes
                .GroupBy(x => x.Space?.Id)
                .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                .Select(x => new SnapshotProtocolVoteData(
                    x.Key!,
                    x.FirstOrDefault()?.Space?.Name ?? string.Empty,
                    x.FirstOrDefault()?.Space?.Avatar,
                    x.FirstOrDefault()?.Space?.About,
                    x.Sum(y => y.Vp ?? 0),
                    x.Count()));

            var snapshotProposalsData = _snapshotProposals
                .GroupBy(x => x.Space?.Id)
                .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                .Select(x => new SnapshotProtocolProposalData(
                    x.Key!,
                    x.FirstOrDefault()?.Space?.Name ?? string.Empty,
                    x.FirstOrDefault()?.Space?.Avatar,
                    x.FirstOrDefault()?.Space?.About,
                    x.Sum(y => y.Votes)));

            return new()
            {
                Balance = _balance.ToEth(),
                BalanceUSD = _balanceUsd,
                WalletAge = IStatCalculator
                    .GetWalletAge(_transactions.Select(x => x.TimeStamp!.ToDateTime())),
                TotalTransactions = _transactions.Count(),
                TotalRejectedTransactions = _transactions.Count(t => t.TxreceiptStatus == "0" || t.IsError == "1"),
                MinTransactionTime = intervals.Min(),
                MaxTransactionTime = intervals.Max(),
                AverageTransactionTime = intervals.Average(),
                WalletTurnover = _transactions.Sum(x => (decimal)x.Value).ToEth(),
                TurnoverIntervals = turnoverIntervals,
                BalanceChangeInLastMonth = IStatCalculator<EthereumTransactionIntervalData>.GetBalanceChangeInLastMonth(turnoverIntervals),
                BalanceChangeInLastYear = IStatCalculator<EthereumTransactionIntervalData>.GetBalanceChangeInLastYear(turnoverIntervals),
                LastMonthTransactions = _transactions.Count(x => x.TimeStamp.ToDateTime() > monthAgo),
                LastYearTransactions = _transactions.Count(x => x.TimeStamp.ToDateTime() > yearAgo),
                TimeFromLastTransaction = (int)((DateTime.UtcNow - _transactions.OrderBy(x => x.TimeStamp).Last().TimeStamp.ToDateTime()).TotalDays / 30),
                NftHolding = holdingTokens,
                NftTrading = (soldSum - buySum).ToEth(),
                NftWorth = nftWorth.ToEth(),
                DeployedContracts = contractsCreated,
                TokensHolding = totalTokens.Count(),
                SnapshotVotes = snapshotVotesData,
                SnapshotProposals = snapshotProposalsData,
                HapiRiskScore = _hapiRiskScore
            };
        }
    }
}