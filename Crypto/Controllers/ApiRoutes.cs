namespace Crypto.Controllers
{
    public class ApiRoutes
    {
        public static class Blockchain
        {
            private const string _base = "/blockchain";
            public const string Get = _base;
            public const string MineBlock = _base + "/mine-block";
            public const string IsChainValid = _base + "/is-chain-valid";
            public const string AddTransaction = _base + "/add-transaction";
            public const string AddNode = _base + "/add-node";
            public const string GetNodes = _base + "/get-nodes";
        }
    }
}
