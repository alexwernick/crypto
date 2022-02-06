using System;
using System.Collections.Generic;

namespace Crypto.Components
{
    public interface IMemPool
    {
        List<Transaction> TakeTransactions();
        bool TryAddTransaction(string sender, string receiver, float amount, Guid id);
        bool TryAddTransaction(Transaction transaction);
        bool TryRemoveTransactions(List<Transaction> transactions);
        List<Transaction> GetTransactions();
    }
}
