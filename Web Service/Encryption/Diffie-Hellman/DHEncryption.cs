using System.Security.Cryptography;
using System.Text;
namespace Web_Service
{
    public class DHEncryption
    {
        public static ECDiffieHellmanCng Dh { get; private set; }
        public byte[] PublicKey { get; private set; }

        // ECDSA for signing data
        private ECDsaCng dsa;
        public byte[] PublicSigningKey { get; private set; }

        public DHEncryption()
        {
            // Setup ECDH for shared key
            Dh = new ECDiffieHellmanCng
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                //derive the key 
                HashAlgorithm = CngAlgorithm.Sha256
                //using the SHA256 Algorithm
            };
            PublicKey = Dh.PublicKey.ToByteArray();
            //convert it to byte array
            // Setup ECDSA for signing/verifying
            dsa = new ECDsaCng(CngKey.Create(CngAlgorithm.ECDsaP256));
            //create the dsa 
            dsa.HashAlgorithm = CngAlgorithm.Sha256;
            //SHA 256 algorithm
            PublicSigningKey = dsa.Key.Export(CngKeyBlobFormat.EccPublicBlob);
            //export the key
        }

        // derive shared AES key from other's public key
        public static byte[] DeriveSharedKey(byte[] otherPartyPublicKey)
        {
            var otherPubKey = ECDiffieHellmanCngPublicKey.FromByteArray(otherPartyPublicKey, CngKeyBlobFormat.EccPublicBlob);
            return Dh.DeriveKeyMaterial(otherPubKey);
        }

        // Sign data with private ECDSA key
        //change ts pls 
        public byte[] SignData(byte[] data)
        {
            return dsa.SignData(data);
        }

        // Static method to verify signature using sender's public signing key
        public bool VerifySignature(byte[] data, byte[] signature, byte[] senderPublicSigningKey)
        {
            using (var ecdsa = new ECDsaCng(CngKey.Import(senderPublicSigningKey, CngKeyBlobFormat.EccPublicBlob)))
            {
                return ecdsa.VerifyData(data, signature, HashAlgorithmName.SHA256);
            }
        }


        // Encrypt message using AES-CBC + HMAC-SHA256 for authentication
        public static byte[] EncryptMessage(string message, byte[] key, out byte[] iv, out byte[] hmac)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();
                iv = aes.IV;

                byte[] ciphertext;
                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs, Encoding.UTF8))
                    {
                        sw.Write(message);
                    }
                    ciphertext = ms.ToArray();
                }

                // Compute HMAC for authentication
                hmac = ComputeHMAC(ciphertext, key);
                return ciphertext;
            }
        }

        // Decrypt message after verifying HMAC
        public static string DecryptMessage(byte[] ciphertext, byte[] key, byte[] iv, byte[] hmac)
        {
            byte[] computedHmac = ComputeHMAC(ciphertext, key);
            if (!computedHmac.SequenceEqual(hmac))
                throw new CryptographicException("Message authentication failed (HMAC mismatch).");

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(ciphertext))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private static byte[] ComputeHMAC(byte[] data, byte[] key)
        {
            using (var hmacsha = new HMACSHA256(key))
            {
                return hmacsha.ComputeHash(data);
            }
        }

    }
}
