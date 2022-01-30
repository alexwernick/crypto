using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crypto.Components
{
    public interface INodeNetwork
    {
        Task<bool> TryAddNode(Uri uri);
        List<Node> GetNodes();
        Task<List<Block>> GetLongestChain();
        Task SynchronizeNodes();
        Task<List<Transaction>> GetMemPool();
    }
}
