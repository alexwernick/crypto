using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Crypto.Components
{
    public class NodeNetworkSynchronizationService : IHostedService
    {
        private const int PollingWaitTimeSeconds = 10;
        private readonly INodeNetwork _nodeNetwork;
        private readonly IBlockchain _blockchain;
        private readonly IMemPool _memPool;

        public NodeNetworkSynchronizationService(INodeNetwork nodeNetwork, IBlockchain blockchain, IMemPool memPool)
        {
            _nodeNetwork = nodeNetwork ?? throw new ArgumentNullException(nameof(nodeNetwork));
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _memPool = memPool ?? throw new ArgumentNullException(nameof(memPool));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _nodeNetwork.SynchronizeNodes();
                await SynchronizeChain();
                await SynchronizeMemPool();
                Thread.Sleep(PollingWaitTimeSeconds * 1000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task SynchronizeChain()
        {
            var longestChainInNetwork = await _nodeNetwork.GetLongestChain();

            if (!_blockchain.IsChainValid() || longestChainInNetwork.Count > _blockchain.Chain.Count)
            {
                _blockchain.ReplaceChain(longestChainInNetwork);
            }
        }

        private async Task SynchronizeMemPool()
        {
            var memPoolFromNodes = await _nodeNetwork.GetMemPool();
            foreach(var transaction in memPoolFromNodes)
            {
                _memPool.TryAddTransaction(transaction);
            }

            foreach(var transaction in _memPool.GetTransactions())
            {
                if(_blockchain.IsTransactionInChain(transaction))
                {
                    _memPool.TryRemoveTransaction(transaction);
                }
            }
        }
    }
}
