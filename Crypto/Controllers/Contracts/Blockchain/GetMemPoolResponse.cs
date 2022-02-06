using System.Collections.Generic;

namespace Crypto.Controllers.Contracts.Blockchain
{
    public partial class GetMemPoolResponse
    {
        public GetMemPoolResponse(IEnumerable<Transaction> transactions)
        {
            Transactions = transactions;
        }

        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
