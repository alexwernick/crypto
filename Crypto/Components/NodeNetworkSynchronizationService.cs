using Crypto.Threading;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Crypto.Components
{
    public class NodeNetworkSynchronizationService : IHostedService
    {
        private const int PollingWaitTimeSeconds = 5;
        private readonly INodeNetwork _nodeNetwork;
        private readonly IBlockchain _blockchain;
        private readonly IMemPool _memPool;
        private readonly ILogger _logger;

        private bool _nodeRegistered;
        private Timer _timer = null!;
        private readonly ThreadSafeSingleShotGuard _guard;

        public NodeNetworkSynchronizationService(INodeNetwork nodeNetwork, IBlockchain blockchain, IMemPool memPool, ILogger logger)
        {
            _nodeNetwork = nodeNetwork ?? throw new ArgumentNullException(nameof(nodeNetwork));
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _memPool = memPool ?? throw new ArgumentNullException(nameof(memPool));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _guard = new ThreadSafeSingleShotGuard();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(BeginSyncronize, null, TimeSpan.Zero, TimeSpan.FromSeconds(PollingWaitTimeSeconds));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void BeginSyncronize(object? _)
        {
            if(_guard.CheckAndSetFirstCall)
            {
                var task = Syncronize();
            }
        }

        private async Task Syncronize()
        {
            try
            {
                var nodeRegistered = await EnsureNodeRegistered();

                if(nodeRegistered)
                {
                    await _nodeNetwork.SynchronizeNodes();
                    await SynchronizeChain();
                    await SynchronizeMemPool();
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Unexpected error during network sycronization");
            }
            finally
            {
                _guard.Reset();
            }
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
            var transactionsToRemove = new List<Transaction>();

            foreach (var transaction in await _nodeNetwork.GetMemPool())
            {
                _memPool.TryAddTransaction(transaction);
            }

            foreach(var transaction in _memPool.GetTransactions())
            {
                if(_blockchain.IsTransactionInChain(transaction))
                {
                    transactionsToRemove.Add(transaction);
                }
            }

            _memPool.TryRemoveTransactions(transactionsToRemove);
        }

        private async Task<bool> EnsureNodeRegistered()
        {
            if(!_nodeRegistered)
            {
                _nodeRegistered = await _nodeNetwork.TryRegisterNode();
            }

            return _nodeRegistered;
        }
    }
}
