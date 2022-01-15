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

        public void AddTransaction(string sender, string receiver, float amount)
        {
            _transactions.Add(new Transaction(sender, receiver, amount));
        }
    }
}
