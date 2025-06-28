using System;
using System.Security.Cryptography;
using System.Text;
namespace Web_Service
{
    public class Encryptor : IEncryptor
    {
        public byte[] GetSalt()
        {
            byte[] salt = new byte[32];
            for (int i = 0; i < salt.Length; i++)
            {
                using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
                {
                    generator.GetBytes(salt);
                }
            }
            return salt;
        }

        public string GetHash(byte[] Salt, string Pass)
        {
            byte[] hash = new byte[32];
            byte[] pass = Encoding.UTF8.GetBytes(Pass);
            Salt = Salt.Concat(pass).ToArray();
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                hash = algorithm.ComputeHash(Salt);
            }
            return Convert.ToBase64String(hash);
        }
    }
}
