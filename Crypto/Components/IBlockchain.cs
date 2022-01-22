using System.Collections.Generic;

namespace Crypto.Components
{
    public interface IBlockchain
    {
        List<Block> Chain { get; }
        void AddBlock(ulong proof, List<Transaction> transactions);
        bool IsChainValid();
        Block GetPreviousBlock();
        void ReplaceChain(List<Block> chain);
    }
}
