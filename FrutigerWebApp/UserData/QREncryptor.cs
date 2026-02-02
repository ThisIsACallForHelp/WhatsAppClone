using Microsoft.AspNetCore.DataProtection;
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
        public static string Sign(string secret, string data)
        {
            byte[] key = Convert.FromBase64String(secret);
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            using (var hmac = new HMACSHA256(key))
            {
                byte[] hash = hmac.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool Verify(string key, string data, byte[] hmac)
        {
            byte[] ByteKey = Convert.FromBase64String(key);
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] computedHmac = ComputeHMAC(bytes, ByteKey);
            return computedHmac.SequenceEqual(hmac);
        }
    }
}
