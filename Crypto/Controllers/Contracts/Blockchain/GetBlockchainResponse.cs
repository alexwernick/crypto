using System.Collections.Generic;

namespace Crypto.Controllers.Contracts.Blockchain
{
    public partial class GetBlockchainResponse
    {
        public GetBlockchainResponse(IEnumerable<Block> blocks)
        {
            Blocks = blocks;
        }

        public IEnumerable<Block> Blocks { get; set; }
    }
}
