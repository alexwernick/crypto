using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Crypto.Components
{
    public class NodeNetwork : INodeNetwork
    {
        private List<Node> _nodes;
        private HttpClient _httpClient;

        public NodeNetwork()
        {
            _nodes = new List<Node>();
            _httpClient = new HttpClient();
        }

        public void AddNode(Uri uri)
        {
            // locking needed
            if(!_nodes.Any(y => y.Address.AbsoluteUri == uri.AbsoluteUri))
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
                // log here
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
                // log here
                return null;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Node>>(jsonResponse);
        }
    }
}
