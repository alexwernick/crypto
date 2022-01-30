using System;

namespace Crypto.Components
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
        public Guid Id { get; set; } // we use and id now for simplicity to ensure we don't have duplicates. This is not how Cryptocurrencies work in practice
    }
}
