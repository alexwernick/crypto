using System;
using System.Collections.Generic;

namespace Crypto.Controllers.Contracts.Blockchain
{
    public class Block
    {
        public Block(ulong proof, string previousHash, DateTime createdDate, IEnumerable<Transaction> transactions)
        {
            Proof = proof;
            PreviousHash = previousHash;
            CreatedDate = createdDate;
            Transactions = transactions;
        }

        public ulong Proof { get; }
        public string PreviousHash { get; }
        public DateTime CreatedDate { get; }
        public IEnumerable<Transaction> Transactions { get; }
    }
}
