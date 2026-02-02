using System.Security.Cryptography;
using System.Text;

namespace FrutigerWebApp
{
    public class QREncryptor
    {
        public string? HMAC { get; set; } = string.Empty;
        public readonly string? _secret;

        public static string GenerateHMAC(int bytes = 64)
        {
            byte[] key = RandomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(key);
        }

        public static byte[] ComputeHMAC(byte[] data, byte[] key)
        {
            using (var hmacsha = new HMACSHA256(key))
            {
                return hmacsha.ComputeHash(data);
            }
        }
        public static string Sign(string secretBase64, string data)
        {
            byte[] key = Convert.FromBase64String(secretBase64);
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA256(key);
            byte[] hash = hmac.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}
