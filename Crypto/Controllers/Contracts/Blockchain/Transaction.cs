using System;

namespace Crypto.Controllers.Contracts.Blockchain
{
    public class Transaction
    {
        public Transaction(string sender, string receiver, float amount, Guid id)
        {
            Sender = sender;
            Receiver = receiver;
            Amount = amount;
            Id = id;
        }

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public float Amount { get; set; }
        public Guid Id { get; set; }
    }
}
