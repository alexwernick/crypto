using System.Collections.Generic;
using System.Linq;

namespace Crypto.Components
{
    public class Blockchain : IBlockchain
    {
        public Blockchain()
        {
            Chain = new List<Block>() { Block.CreateGenesisBlock() };
        }

        public List<Block> Chain { get; private set; }

        public static bool ValidateChain(List<Block> chain)
        {
            for (int i = 1; i < chain.Count; ++i)
            {
                var block = chain[i];
                var previousBlock = chain[i - 1];

                if (!block.IsBlockValid(previousBlock.Hash()))
                {
                    return false;
                }
            }

            return true;
        }

        public void AddBlock(ulong proof, List<Transaction> transactions)
        {
            var previousHash = GetPreviousBlock().Hash();
            var block = Block.CreateBlock(proof, previousHash, transactions);
            Chain.Add(block);
        }

        public bool IsChainValid()
        {
            return ValidateChain(Chain);
        }

        public Block GetPreviousBlock()
        {
            return Chain.Last();
        }

        public void ReplaceChain(List<Block> chain)
        {
            Chain = chain;
        }

        public bool IsTransactionInChain(Transaction transactions)
        {
            foreach(var block in Chain)
            {
                if(block.Transactions.Any(x => x.Id == transactions.Id))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
