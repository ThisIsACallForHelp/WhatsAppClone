using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Sandbox.Program;
namespace Sandbox
{
    internal class Program
    {
        public class DHEncryption
        {
            public ECDiffieHellmanCng Dh { get; private set; }
            public byte[] PublicKey { get; private set; }

            public DHEncryption()
            {
                Dh = new ECDiffieHellmanCng
                {
                    KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                    HashAlgorithm = CngAlgorithm.Sha256
                };
                PublicKey = Dh.PublicKey.ToByteArray();
            }
            //it works ig 
            public byte[] DeriveSharedKey(byte[] otherPartyPublicKey)
            {
                var otherPubKey = ECDiffieHellmanCngPublicKey.FromByteArray(otherPartyPublicKey, CngKeyBlobFormat.EccPublicBlob);
                return Dh.DeriveKeyMaterial(otherPubKey); // shared AES key
            }
            public static byte[] EncryptAES(string message, byte[] key, out byte[] iv)
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.GenerateIV();
                    iv = aes.IV;

                    using (var encryptor = aes.CreateEncryptor())
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(message);
                        }
                        return ms.ToArray();
                    }
                }
            }

            public static string DecryptAES(byte[] encrypted, byte[] key, byte[] iv)
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var ms = new MemoryStream(encrypted))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }

        }
        public static void Main(string[] args)
        {

            // Alice and Bob each generate keys
            var alice = new DHEncryption();
            var bob = new DHEncryption();

            // They exchange public keys and derive the same AES key
            byte[] aliceSharedKey = alice.DeriveSharedKey(bob.PublicKey);
            byte[] bobSharedKey = bob.DeriveSharedKey(alice.PublicKey);

            // Alice encrypts message with AES using shared key
            byte[] iv;
            byte[] encryptedMessage = DHEncryption.EncryptAES("Hello from Alice!", aliceSharedKey, out iv);

            // Bob decrypts it
            string decrypted = DHEncryption.DecryptAES(encryptedMessage, bobSharedKey, iv);
            Console.WriteLine(decrypted); // Output: Hello from Alice!



        }
    }
}
