using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Crypto.Components
{
    public class NodeNetwork : INodeNetwork
    {
        private readonly ConcurrentDictionary<string, Node> _nodes;
        private readonly HttpClient _httpClient;
        private readonly Ping _ping;
        private readonly NodeNetworkOptions _options;
        private readonly ILogger _logger;

        public NodeNetwork(NodeNetworkOptions options, ILogger logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _nodes = new ConcurrentDictionary<string, Node>();
            _httpClient = new HttpClient();
            _ping = new Ping();
            AddSeedNodes().Wait();
        }

        public async Task<bool> TryAddNode(Uri uri)
        {
            return await CanConnectToNode(uri) && _nodes.TryAdd(uri.Host, new Node(uri));
        }

        public List<Node> GetNodes()
        {
            return _nodes.Values.ToList();
        }

        public async Task<List<Block>> GetLongestChain()
        {
            var longestChain = new List<Block>();

            foreach (var node in _nodes.Values)
            {
                List<Block>? chain = await GetChainFromNode(node);
                
                if(chain?.Count > longestChain.Count && Blockchain.ValidateChain(chain))
                {
                    longestChain = chain;
                }
            }

            return longestChain;
        }

        public async Task SynchronizeNodes()
        {
            foreach (var node in _nodes.Values)
            {
                List<Node>? nodesFromOtherNode = await GetNodesFromNode(node);

                if (nodesFromOtherNode is not null)
                {
                    foreach(var nodeFromOtherNode in nodesFromOtherNode)
                    {
                        await TryAddNode(nodeFromOtherNode.Address);
                    }
                }
            }
        }

        public async Task<List<Transaction>> GetMemPool()
        {
            var memPool = new List<Transaction>();

            foreach (var node in _nodes.Values)
            {
                List<Transaction>? nodeMemPool = await GetMemPoolFromNode(node);
                if(nodeMemPool is not null)
                {
                    memPool.AddRange(nodeMemPool.Where(x => !memPool.Any(y => y.Id == x.Id)));
                }
            }

            return memPool;
        }

        private async Task<List<Block>?> GetChainFromNode(Node node)
        {
            return await GetFromNode<List<Block>>(node, Controllers.ApiRoutes.Blockchain.Get);
        }

        private async Task<List<Node>?> GetNodesFromNode(Node node)
        {
            return await GetFromNode<List<Node>>(node, Controllers.ApiRoutes.Blockchain.GetNodes);
        }

        private async Task<List<Transaction>?> GetMemPoolFromNode(Node node)
        {
            return await GetFromNode<List<Transaction>>(node, Controllers.ApiRoutes.Blockchain.GetMemPool);
        }

        private async Task<T?> GetFromNode<T>(Node node, string root) where T : class
        {
            var response = await _httpClient.GetAsync(new Uri(node.Address, root));

            if (!response.IsSuccessStatusCode)
            {
                HandleNodeConnectionFailure(node, response.StatusCode);
                return null;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }


        private void HandleNodeConnectionFailure(Node node, HttpStatusCode statusCode)
        {
            _logger.Information(
                "Failed to connect to node {NodeAddress} with status code {StatusCode}",
                node.Address.AbsoluteUri,
                statusCode.ToString());

            if (!node.IsSeedNode)
            {
                if(_nodes.TryRemove(node.Address.Host, out _))
                {
                    _logger.Information("Node {NodeAddress} has been removed", node.Address.AbsoluteUri);
                }
            }
        }

        private async Task<bool> CanConnectToNode(Uri uri)
        {
            var result = await _ping.SendPingAsync(uri.Host);

            if(result.Status == IPStatus.Success)
            {
                return true;
            }

            _logger.Information("Failed to ping node {NodeAddress} with status {IPStatus}", uri.AbsoluteUri, result.Status.ToString());
            return false;
        }

        private async Task AddSeedNodes()
        {
            if (_options?.SeedNodes is not null)
            {
                foreach (var seedNode in _options.SeedNodes)
                {
                    var uri = new UriBuilder(seedNode).Uri;
                    if(await TryAddNode(uri))
                    {
                        _logger.Information("Added seed node {NodeAddress}", uri.AbsoluteUri);
                    }
                    else
                    {
                        _logger.Information("Failed to add seed node {NodeAddress}", uri.AbsoluteUri);
                    }
                }
            }
        }
    }
}
