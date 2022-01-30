using Crypto.Components;
using Crypto.Controllers.Contracts.Blockchain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Crypto.Controllers
{
    [ApiController]
    public class BlockchainController : ControllerBase
    {
        private readonly IBlockchain _blockchain;
        private readonly IMemPool _memPool;
        private readonly INodeNetwork _nodeNetwork;

        public BlockchainController(IBlockchain blockchain, IMemPool memPool, INodeNetwork nodeNetwork)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _memPool = memPool ?? throw new ArgumentNullException(nameof(memPool));
            _nodeNetwork = nodeNetwork ?? throw new ArgumentNullException(nameof(nodeNetwork));
        }

        [HttpGet(ApiRoutes.Blockchain.Get)]
        public IActionResult Get()
        { 
            return Ok(_blockchain.Chain);
        }

        [HttpGet(ApiRoutes.Blockchain.MineBlock)]
        public IActionResult MineBlock()
        {
            var previousBlock = _blockchain.GetPreviousBlock();
            var transactions = _memPool.TakeTransactions();
            transactions.Add(new Transaction("coinbase", "miner", 1, Guid.NewGuid())); // add coinbase transaction
            var proof = Block.ProofOfWork(previousBlock.Hash(), transactions);
            _blockchain.AddBlock(proof, transactions);
            return Ok(_blockchain.GetPreviousBlock());
        }

        [HttpGet(ApiRoutes.Blockchain.IsChainValid)]
        public IActionResult IsChainValid()
        {
            return Ok(new { IsChainValid = _blockchain.IsChainValid() });
        }

        [HttpPost(ApiRoutes.Blockchain.AddTransaction)]
        public IActionResult AddTransaction([FromBody]AddTransactionRequest request)
        {
            // add validation
            _memPool.TryAddTransaction(request.Sender, request.Receiver, request.Amount, Guid.NewGuid());
            return Accepted();
        }

        [HttpPost(ApiRoutes.Blockchain.AddNode)]
        public async Task<IActionResult> AddNode([FromBody] AddNodeRequest request)
        {
            // add validation
            await _nodeNetwork.TryAddNode(new Uri(request.NodeAddress));
            return Accepted();
        }

        [HttpGet(ApiRoutes.Blockchain.GetNodes)]
        public IActionResult GetNodes()
        {
            return Ok(_nodeNetwork.GetNodes());
        }

        [HttpGet(ApiRoutes.Blockchain.GetMemPool)]
        public IActionResult GetMemPool()
        {
            return Ok(_memPool.GetTransactions());
        }
    }
}