using System.Security.Cryptography;

namespace Web_Service
{
    public class KeyStorage
    {
        public static void SaveEncryptedKey(string filePath, byte[] keyBytes)
        {
            // Protect key bytes for current user on this machine
            byte[] protectedBytes = ProtectedData.Protect(keyBytes, null, DataProtectionScope.CurrentUser);
            System.IO.File.WriteAllBytes(filePath, protectedBytes);
        }

        // Load and decrypt key bytes from file
        public static byte[] LoadEncryptedKey(string filePath)
        {
            byte[] protectedBytes = System.IO.File.ReadAllBytes(filePath);
            return ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
        }
    }
}
