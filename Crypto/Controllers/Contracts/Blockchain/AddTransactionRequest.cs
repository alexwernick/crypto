namespace Crypto.Controllers.Contracts.Blockchain
{
    public class AddTransactionRequest
    {
        public AddTransactionRequest(string sender, string receiver, float amount)
        {
            Sender = sender;
            Receiver = receiver;
            Amount = amount;
        }

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public float Amount { get; set; }
    }
}
