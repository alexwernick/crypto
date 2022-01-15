using System;
using System.Collections.Generic;
using System.Linq;

namespace Crypto.Components
{
    public class Blockchain : IBlockchain
    {
        private readonly INodeNetwork _nodeNetwork;

        public Blockchain(INodeNetwork nodeNetwork)
        {
            _nodeNetwork = nodeNetwork ?? throw new ArgumentNullException(nameof(nodeNetwork));
            Chain = new List<Block>() { Block.CreateGenesisBlock() };
        }

        public List<Block> Chain { get; private set; }

        public void AddBlock(ulong proof, List<Transaction> transactions)
        {
            var previousHash = GetPreviousBlock().Hash();
            var block = Block.CreateBlock(proof, previousHash, transactions);
            Chain.Add(block);
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < Chain.Count; ++i)
            {
                var block = Chain[i];
                var previousBlock = Chain[i - 1];

                if (!block.IsBlockValid(previousBlock.Hash()))
                {
                    return false;
                }
            }

            return true;
        }

        public Block GetPreviousBlock()
        {
            return Chain.Last();
        }

        public bool ReplaceChain()
        {
            var longestChainInNetwork = _nodeNetwork.GetLongestChain();

            if (longestChainInNetwork.Count > Chain.Count)
            {
                Chain = longestChainInNetwork;
                return true;
            }

            return false;
        }
    }
}
