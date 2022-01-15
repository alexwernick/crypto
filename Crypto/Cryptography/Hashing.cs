using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Crypto.Cryptography
{
    public static class Hashing
    {
        public static byte[] ComputeSHA256Hash(string input)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                return mySHA256.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }

        public static string ComputeSHA256HashAsHexidecimal(string input)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in ComputeSHA256Hash(input))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
