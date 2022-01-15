using System;
using System.Collections.Generic;

namespace Crypto.Components
{
    public class NodeNetwork : INodeNetwork
    {
        private List<Node> _nodes;

        public NodeNetwork()
        {
            _nodes = new List<Node>();
        }

        public void AddNode(Uri uri)
        {
            _nodes.Add(new Node(uri));
        }

        public List<Block> GetLongestChain()
        {
            // loop through all nodes and do http request to get block

            return new List<Block>();
        }
    }
}
