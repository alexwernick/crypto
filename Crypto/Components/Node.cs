using System;

namespace Crypto.Components
{
    public class Node
    {
        public Node(Uri address)
        {
            Address = address;
            Id = Guid.NewGuid();
        }

        public Uri Address { get; }
        public Guid Id { get; }
    }
}
