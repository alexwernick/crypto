using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<string, Node> _nodes;
        private HttpClient _httpClient;
        private Ping _ping;

        public NodeNetwork()
        {
            _nodes = new ConcurrentDictionary<string, Node>();
            _httpClient = new HttpClient();
            _ping = new Ping();
        }

        public async Task<bool> TryAddNode(Uri uri)
        {
            // locking needed
            if(!await CanConnectToNode(uri))
            {
                return false;
            }

            return _nodes.TryAdd(uri.Host, new Node(uri));
        }

        public List<Node> GetNodes()
        {
            return _nodes.Values.ToList();
        }

        public async Task<List<Block>> GetLongestChain()
        {
            var longestChain = new List<Block>();

            foreach (var node in _nodes)
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

        private async Task<List<Block>?> GetChainFromNode(Node node)
        {
            var response = await _httpClient.GetAsync(new Uri(node.Address, Controllers.ApiRoutes.Blockchain.Get));
            
            if (!response.IsSuccessStatusCode)
            {
                HandleNodeConnectionFailure(node, response.StatusCode);
                return null;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Block>>(jsonResponse);
        }

        private async Task<List<Node>?> GetNodesFromNode(Node node)
        {
            var response = await _httpClient.GetAsync(new Uri(node.Address, Controllers.ApiRoutes.Blockchain.GetNodes));

            if (!response.IsSuccessStatusCode)
            {
                HandleNodeConnectionFailure(node, response.StatusCode);
                return null;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Node>>(jsonResponse);
        }

        private void HandleNodeConnectionFailure(Node node, HttpStatusCode statusCode)
        {
            // log failure and http status code

            if(!node.IsSeedNode)
            {
                if(_nodes.(node))
                {
                    // log node is removed
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

            // log ping failure
            return false;
        }
    }
}
