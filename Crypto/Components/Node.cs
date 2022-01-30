using System;

namespace Crypto.Components
{
    public class Node
    {
        public Node(Uri address, bool isSeedNode = false)
        {
            Address = address;
            Id = Guid.NewGuid();
            IsSeedNode = isSeedNode;
        }

        public Uri Address { get; }
        public Guid Id { get; }
        public bool IsSeedNode { get; }
    }
}
