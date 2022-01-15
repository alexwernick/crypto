using System;
using System.Collections.Generic;
using Crypto.Cryptography;
using Newtonsoft.Json;

namespace Crypto.Components
{
    public class Block
    {
        private const string ProofCriteria = "0000";

        private Block(ulong proof, string previousHash, List<Transaction> transactions)
        {
            Proof = proof;
            PreviousHash = previousHash;
            CreatedDate = DateTime.UtcNow;
            Transactions = transactions;
        }

        public ulong Proof { get; }
        public string PreviousHash { get; }
        public DateTime CreatedDate { get; }
        public List<Transaction> Transactions { get; }


        public static Block CreateGenesisBlock()
        {
            var previousHash = "0";
            var transactions = new List<Transaction>();
            var proof = ProofOfWork(previousHash, transactions);
            return new Block(proof, "0", transactions);
        }

        public static Block CreateBlock(ulong proof, string previousHash, List<Transaction> Transactions)
        {
            return new Block(proof, previousHash, Transactions);
        }

        public static ulong ProofOfWork(string previousHash, List<Transaction> transactions)
        {
            ulong newProof = 1;

            while (!CheckProof(newProof, previousHash, transactions))
            {
                ++newProof;
            }

            return newProof;
        }

        private static bool CheckProof(ulong proof, string previousHash, List<Transaction> transactions)
        {
            return CreateHash(proof, previousHash, transactions).StartsWith(ProofCriteria);
        }

        public bool IsBlockValid(string previousHash)
        {
            if (PreviousHash != previousHash)
            {
                return false;
            }

            if(!CheckProof(Proof, previousHash, Transactions))
            {
                return false;
            }

            return true;
        }

        public string Hash()
        {
            return CreateHash(Proof, PreviousHash, Transactions);
        }

        private static string CreateHash(ulong proof, string previousHash, List<Transaction> transactions)
        {
            return Hashing.ComputeSHA256HashAsHexidecimal($"{proof}.{previousHash}.{JsonConvert.SerializeObject(transactions)}");
        }
    }
}
