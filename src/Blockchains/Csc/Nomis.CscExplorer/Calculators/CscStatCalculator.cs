// ------------------------------------------------------------------------------------------------------
// <copyright file="CscStatCalculator.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.Blockchain.Abstractions.Calculators;
using Nomis.Blockchain.Abstractions.Models;
using Nomis.CscExplorer.Interfaces.Extensions;
using Nomis.CscExplorer.Interfaces.Models;
using Nomis.Utils.Extensions;

namespace Nomis.CscExplorer.Calculators
{
    /// <summary>
    /// Csc wallet stats calculator.
    /// </summary>
    internal sealed class CscStatCalculator :
        IStatCalculator<CscWalletStats, CscTransactionIntervalData>
    {
        private readonly string _address;
        private readonly decimal _balance;
        private readonly decimal _usdBalance;
        private readonly IEnumerable<CscExplorerAccountTransactionRecord> _transactions;
        private readonly IEnumerable<CscExplorerAccountCetTransferRecord> _cetTransfers;
        private readonly IEnumerable<CscExplorerAccountCrc20TransferRecord> _crc20Transfers;
        private readonly IEnumerable<CscExplorerAccountCrc721TransferRecord> _crc721Transfers;

        public CscStatCalculator(
            string address,
            decimal balance,
            decimal usdBalance,
            IEnumerable<CscExplorerAccountTransactionRecord> transactions,
            IEnumerable<CscExplorerAccountCetTransferRecord> cetTransfers,
            IEnumerable<CscExplorerAccountCrc20TransferRecord> crc20Transfers,
            IEnumerable<CscExplorerAccountCrc721TransferRecord> crc721Transfers)
        {
            _address = address;
            _balance = balance;
            _usdBalance = usdBalance;
            _transactions = transactions;
            _cetTransfers = cetTransfers;
            _crc20Transfers = crc20Transfers;
            _crc721Transfers = crc721Transfers;
        }

        public CscWalletStats GetStats()
        {
            if (!_transactions.Any())
            {
                return new()
                {
                    NoData = true
                };
            }

            var intervals = IStatCalculator
                .GetTransactionsIntervals(_transactions.Select(x => x.TimeStamp.ToString().ToDateTime())).ToList();
            if (intervals.Count == 0)
            {
                return new()
                {
                    NoData = true
                };
            }

            var monthAgo = DateTime.Now.AddMonths(-1);
            var yearAgo = DateTime.Now.AddYears(-1);

            var soldTokens = _crc721Transfers.Where(x => x.From?.Address?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true).ToList();
            var soldSum = IStatCalculator
                .GetTokensSum(soldTokens.Select(x => x.TxHash!), _transactions.Select(x => (x.TxHash!, BigInteger.TryParse(x.Amount, out var amount) ? amount : 0)));

            var soldTokensIds = soldTokens.Select(x => x.GetTokenUid());
            var buyTokens = _crc721Transfers.Where(x => x.To?.Address?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true && soldTokensIds.Contains(x.GetTokenUid()));
            var buySum = IStatCalculator
                .GetTokensSum(buyTokens.Select(x => x.TxHash!), _transactions.Select(x => (x.TxHash!, BigInteger.TryParse(x.Amount, out var amount) ? amount : 0)));

            var buyNotSoldTokens = _crc721Transfers.Where(x => x.To?.Address?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true && !soldTokensIds.Contains(x.GetTokenUid()));
            var buyNotSoldSum = IStatCalculator
                .GetTokensSum(buyNotSoldTokens.Select(x => x.TxHash!), _transactions.Select(x => (x.TxHash!, BigInteger.TryParse(x.Amount, out var amount) ? amount : 0)));

            int holdingTokens = _crc721Transfers.Count() - soldTokens.Count;
            decimal nftWorth = buySum == 0 ? 0 : (decimal)soldSum / (decimal)buySum * (decimal)buyNotSoldSum;
            int contractsCreated = _transactions.Count(x => x.Method?.Equals("Create Contract") == true && x.Status == 1);
            var totalTokens = _crc20Transfers.Select(x => x.TokenInfo?.Symbol).Distinct();

            var turnoverIntervalsDataList =
                _transactions.Select(x => new TurnoverIntervalsData(
                    x.TimeStamp.ToString().ToDateTime(),
                    BigInteger.TryParse(x.Amount, out var value) ? value : 0,
                    x.From?.Address?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true));
            var turnoverIntervals = IStatCalculator<CscTransactionIntervalData>
                .GetTurnoverIntervals(turnoverIntervalsDataList, _transactions.Min(x => x.TimeStamp.ToString().ToDateTime())).ToList();

            return new()
            {
                Balance = _balance,
                BalanceUSD = _usdBalance,
                WalletAge = IStatCalculator
                    .GetWalletAge(_transactions.Select(x => x.TimeStamp.ToString().ToDateTime())),
                TotalTransactions = _transactions.Count(),
                TotalRejectedTransactions = _transactions.Count(t => t.Status == 0),
                MinTransactionTime = intervals.Min(),
                MaxTransactionTime = intervals.Max(),
                AverageTransactionTime = intervals.Average(),
                WalletTurnover = _transactions.Sum(x => decimal.TryParse(x.Amount, out decimal value) ? value : 0),
                TurnoverIntervals = turnoverIntervals,
                BalanceChangeInLastMonth = IStatCalculator<CscTransactionIntervalData>.GetBalanceChangeInLastMonth(turnoverIntervals),
                BalanceChangeInLastYear = IStatCalculator<CscTransactionIntervalData>.GetBalanceChangeInLastYear(turnoverIntervals),
                LastMonthTransactions = _transactions.Count(x => x.TimeStamp.ToString().ToDateTime() > monthAgo),
                LastYearTransactions = _transactions.Count(x => x.TimeStamp.ToString().ToDateTime() > yearAgo),
                TimeFromLastTransaction = (int)((DateTime.UtcNow - _transactions.OrderBy(x => x.TimeStamp).Last().TimeStamp.ToString().ToDateTime()).TotalDays / 30),
                NftHolding = holdingTokens,
                NftTrading = (decimal)(soldSum - buySum),
                NftWorth = nftWorth,
                DeployedContracts = contractsCreated,
                TokensHolding = totalTokens.Count()
            };
        }
    }
}