using Newtonsoft.Json;
using System;
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
        private List<Node> _nodes;
        private HttpClient _httpClient;
        private Ping _ping;

        public NodeNetwork()
        {
            _nodes = new List<Node>();
            _httpClient = new HttpClient();
            _ping = new Ping();
        }

        public async Task AddNode(Uri uri)
        {
            // locking needed
            if(!_nodes.Any(y => y.Address.AbsoluteUri == uri.AbsoluteUri) && await CanConnectToNode(uri))
            {
                _nodes.Add(new Node(uri));
            }
        }

        public List<Node> GetNodes()
        {
            return _nodes;
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
            foreach (var node in _nodes)
            {
                List<Node>? nodesFromOtherNode = await GetNodesFromNode(node);

                if (nodesFromOtherNode is not null)
                {
                    foreach(var nodeFromOtherNode in nodesFromOtherNode)
                    {
                        AddNode(nodeFromOtherNode.Address);
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
                if(_nodes.Remove(node))
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
