using System;

namespace Crypto.Components
{
    public class Node
    {
        public Node(Uri address, bool isSeedNode = false)
        {
            Address = address;
            IsSeedNode = isSeedNode;
        }

        public Uri Address { get; }
        public bool IsSeedNode { get; }
    }
}
