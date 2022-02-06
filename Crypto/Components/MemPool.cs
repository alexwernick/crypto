using System;
using System.Collections.Generic;
using System.Linq;

namespace Crypto.Components
{
    public class MemPool : IMemPool
    {
        private List<Transaction> _transactions;

        public MemPool()
        {
            _transactions = new List<Transaction>();
        }

        public List<Transaction> TakeTransactions()
        {
            // this function needs locking....
            int amountToTake = 10; // for now we just take the top ten
            var transactionsToTake = _transactions.Take(amountToTake).ToList(); 
            var remainingTransactions = _transactions.Skip(amountToTake).ToList();
            _transactions = remainingTransactions;
            return transactionsToTake;
        }

        public bool TryAddTransaction(string sender, string receiver, float amount, Guid id)
        {
            return TryAddTransaction(new Transaction(sender, receiver, amount, id));
        }

        public bool TryAddTransaction(Transaction transaction)
        {
            // locking needed here
            if (_transactions.Any(x => x.Id == transaction.Id))
            {
                return false;
            }

            _transactions.Add(transaction);
            return true;
        }

        public bool TryRemoveTransactions(List<Transaction> transactions)
        {
            // locking needed here
            return _transactions.RemoveAll(x => transactions.Any(y => y.Id == x.Id)) > 0;
        }

        public List<Transaction> GetTransactions()
        {
            return _transactions;
        }
    }
}
