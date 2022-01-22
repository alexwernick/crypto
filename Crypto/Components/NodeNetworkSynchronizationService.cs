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

        public NodeNetworkSynchronizationService(INodeNetwork nodeNetwork, IBlockchain blockchain)
        {
            _nodeNetwork = nodeNetwork ?? throw new ArgumentNullException(nameof(nodeNetwork));
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await SynchronizeChain();
                await _nodeNetwork.SynchronizeNodes();
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
    }
}
