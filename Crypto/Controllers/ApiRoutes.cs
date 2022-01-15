namespace Crypto.Controllers
{
    public class ApiRoutes
    {
        public static class Blockchain
        {
            private const string _base = "/blockchain";
            public const string Get = _base;
            public const string MineBlock = _base + "/mineblock";
            public const string IsChainValid = _base + "/ischainvalid";
            public const string AddTransaction = _base + "/addtransaction";
        }
    }
}
