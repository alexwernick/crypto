using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crypto.Components
{
    public interface INodeNetwork
    {
        void AddNode(Uri uri);
        List<Node> GetNodes();
        Task<List<Block>> GetLongestChain();
        Task SynchronizeNodes();
    }
}
