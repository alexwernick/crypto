using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crypto.Components
{
    public interface INodeNetwork
    {
        Task<bool> TryAddNode(Uri uri, bool isSeedNode = false);
        List<Node> GetNodes();
        Task<List<Block>> GetLongestChain();
        Task SynchronizeNodes();
        Task<List<Transaction>> GetMemPool();
        Task<bool> TryRegisterNode();
    }
}
