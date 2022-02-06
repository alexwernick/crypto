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
using System.Text;
using Serilog.Events;
using Crypto.Controllers.Contracts.Blockchain;

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

        public async Task<bool> TryAddNode(Uri uri, bool isSeedNode = false)
        {
            return await CanConnectToNode(uri) && _nodes.TryAdd(uri.Host, new Node(uri, isSeedNode: isSeedNode));
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

        public async Task<bool> TryRegisterNode()
        {            
            if(!_options.TryRegisterNode)
            {
                _logger.Information("Registration will be skipped as node is configured to skip registration to the network");
                return true;
            }

            if(string.IsNullOrWhiteSpace(_options.NodeAddress))
            {
                _logger.Error("Registration failed as NodeAddress is not configured as is null or empty");
                return false;
            }

            if (!_nodes.Values.Any())
            {
                _logger.Error("Registration failed as the node is not aware of any other nodes to register with");
                return false;
            }

            var addNodeRequest = new Controllers.Contracts.Blockchain.AddNodeRequest(_options.NodeAddress);
            var content = new StringContent(JsonConvert.SerializeObject(addNodeRequest), Encoding.UTF8, "application/json");

            foreach (var node in _nodes.Values)
            {
                try 
                {
                    var response = await _httpClient.PostAsync(new Uri(node.Address, Controllers.ApiRoutes.Blockchain.AddNode), content);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.Information(
                             "Registration succeeded with node {NodeAddress} with status code {StatusCode}",
                             node.Address.AbsoluteUri,
                             response.StatusCode.ToString());

                        return true;
                    }

                    _logger.Warning(
                        "Registration failed with node {NodeAddress} with status code {StatusCode}",
                        node.Address.AbsoluteUri,
                        response.StatusCode.ToString());
                }
                catch(Exception ex)
                {
                    _logger.Error(
                        ex,
                        "Registration failed with node {NodeAddress}",
                        node.Address.AbsoluteUri);
                }
            }

            return false;
        }

        private async Task<List<Block>?> GetChainFromNode(Node node)
        {
            var getBlockChainResponse = await GetFromNode<GetBlockchainResponse>(node, Controllers.ApiRoutes.Blockchain.Get);
            return getBlockChainResponse?.Blocks.Select(x => new Block(
                x.Proof,
                x.PreviousHash,
                x.CreatedDate,
                x.Transactions.Select(y => new Transaction(
                    y.Sender,
                    y.Receiver,
                    y.Amount,
                    y.Id)).ToList())).ToList();
        }

        private async Task<List<Node>?> GetNodesFromNode(Node node)
        {
            var getNodesResponse = await GetFromNode<GetNodesResponse>(node, Controllers.ApiRoutes.Blockchain.GetNodes);
            return getNodesResponse?.NodeAddresses.Select(x => new Node (new UriBuilder(x).Uri)).ToList();
        }

        private async Task<List<Transaction>?> GetMemPoolFromNode(Node node)
        {
            var getMemPoolResponse = await GetFromNode<GetMemPoolResponse>(node, Controllers.ApiRoutes.Blockchain.GetMemPool);
            return getMemPoolResponse?.Transactions.Select(x => new Transaction(x.Sender, x.Receiver, x.Amount, x.Id)).ToList();
        }

        private async Task<T?> GetFromNode<T>(Node node, string root) where T : class
        {
            try 
            {
                var response = await _httpClient.GetAsync(new Uri(node.Address, root));

                if (!response.IsSuccessStatusCode)
                {
                    HandleNodeConnectionFailure(node, statusCode: response.StatusCode);
                    return null;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonResponse);
            }
            catch(Exception ex)
            {
                HandleNodeConnectionFailure(node, ex: ex);
                return null;
            }
        }


        private void HandleNodeConnectionFailure(Node node, HttpStatusCode? statusCode = null, Exception? ex = null)
        {
            _logger.Write(
                ex is null ? LogEventLevel.Error : LogEventLevel.Warning,
                ex,
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
                    if(await TryAddNode(uri, isSeedNode: true))
                    {
                        _logger.Information("Added seed node {NodeAddress}", uri.AbsoluteUri);
                    }
                    else
                    {
                        _logger.Warning("Failed to add seed node {NodeAddress}", uri.AbsoluteUri);
                    }
                }
            }
        }
    }
}
