namespace Crypto.Controllers.Contracts.Blockchain
{
    public class AddNodeRequest
    {
        public AddNodeRequest(string nodeAddress)
        {
            NodeAddress = nodeAddress;
        }

        public string NodeAddress { get; set; }
    }
}
