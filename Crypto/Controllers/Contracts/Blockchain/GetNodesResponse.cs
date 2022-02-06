using System.Collections.Generic;

namespace Crypto.Controllers.Contracts.Blockchain
{
    public class GetNodesResponse
    {
        public GetNodesResponse(IEnumerable<string> nodeAddresses)
        {
            NodeAddresses = nodeAddresses;
        }

        public IEnumerable<string> NodeAddresses { get; set; }
    }
}
