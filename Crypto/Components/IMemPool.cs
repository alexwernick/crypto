using System.Collections.Generic;

namespace Crypto.Components
{
    public interface IMemPool
    {
        List<Transaction> TakeTransactions();
        void AddTransaction(string sender, string receiver, float amount);
    }
}
