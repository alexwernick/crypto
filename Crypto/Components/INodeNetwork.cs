using System;
using System.Collections.Generic;

namespace Crypto.Components
{
    public interface INodeNetwork
    {
        void AddNode(Uri uri);
        List<Block> GetLongestChain();
    }
}
