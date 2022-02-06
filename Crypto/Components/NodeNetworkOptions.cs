using System.Collections.Generic;

namespace Crypto.Components
{
    public class NodeNetworkOptions
    {
        public List<string>? SeedNodes { get; set; }
        public bool TryRegisterNode { get; set; } = true;
        public string? NodeAddress { get; set; }
    }
}
