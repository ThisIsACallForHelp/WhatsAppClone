using Data;
using Ganss.Xss;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Web_Service;
using static System.Net.Mime.MediaTypeNames;
namespace FrutigerWebApp
{
    public static class ComplexHelper<T>
    {
        public static string ChatID { get; set; }

        //finish ts
        public static Message SendMSG(string SenderID, string Text, string PublicKey, string SigningKey, out byte[] IV ,
                                      out byte[] HMAC, out byte[] CipherTxt, out byte[] BPublic)
        {
            
            DHEncryption dh = new DHEncryption();
            ECDsaCng dsa = new ECDsaCng(CngKey.Create(CngAlgorithm.ECDsaP256));
            dsa.HashAlgorithm = CngAlgorithm.Sha256;
            byte[] SingningPublicKey = dsa.Key.Export(CngKeyBlobFormat.EccPublicBlob);
            byte[] BytePublicKey = DHEncryption.DeriveSharedKey(Convert.FromBase64String(PublicKey));
            byte[] ByteSigningPublicKey = Convert.FromBase64String(PublicKey);
            byte[] hmac, iv;
            //encrypted text
            byte[] CipherText = DHEncryption.EncryptMessage(Text, BytePublicKey, out iv, out hmac);
            //Sign the data 
            byte[] Signature = dsa.SignData(CipherText);
            using (ECDsaCng verifier = new ECDsaCng(CngKey.Import(SingningPublicKey, CngKeyBlobFormat.EccPublicBlob)))
            {
                verifier.HashAlgorithm = CngAlgorithm.Sha256;
                bool valid = verifier.VerifyData(CipherText, Signature);
                Console.WriteLine("Signature valid? " + valid);
            }
            Data.Message message = new Data.Message
            {
                SenderID = SenderID,
                CipherTextBase64 = Convert.ToBase64String(CipherText),
                SenderPublicKeyBase64 = Convert.ToBase64String(BytePublicKey),
                SenderSigningKeyBase64 = Convert.ToBase64String(ByteSigningPublicKey),
                SignatureBase64 = Convert.ToBase64String(Signature),
                IVBase64 = Convert.ToBase64String(iv),
                HmacBase64 = Convert.ToBase64String(hmac),
                Attachments = "TEST",
                SentAt = DateTime.Now,
                ChatID = ComplexHelper<string>.ChatID,
                ID = Guid.NewGuid().ToString()
            };
            Console.WriteLine($"SignatureBase64.Length -> {message.SignatureBase64.Length}");
            Console.WriteLine($"IVBase64.Length -> {message.IVBase64.Length}");
            Console.WriteLine($"SenderPublicKeyBase64.Length -> {message.SenderPublicKeyBase64.Length}");
            Console.WriteLine($" SenderSigningKeyBase64.Length -> {message.SenderSigningKeyBase64.Length}");
            Console.WriteLine($"CipherTextBase64.Length -> {message.CipherTextBase64.Length}");
            Console.WriteLine($"HmacBase64.Length -> {message.HmacBase64.Length}");
            IV = iv;
            HMAC = hmac;
            CipherTxt = CipherText;
            BPublic = BytePublicKey;
            return message;
        }
        public static T UnprotectObj(T ProtectedObj, IDataProtector dataProtector)
        {
            foreach(PropertyInfo info in typeof(T).GetProperties())
            {
                if (info.CanWrite)
                {
                    var val = info.GetValue(ProtectedObj);
                    var Unprotect = dataProtector.Unprotect(Convert.FromBase64String(val.ToString()));
                    info.SetValue(ProtectedObj, Convert.ToBase64String(Unprotect));
                }
            }
            return ProtectedObj;
        }
        public static T ProtectObj(T UnprotectedObj, IDataProtector dataProtector)
        {
            foreach(PropertyInfo info in typeof(T).GetProperties())
            {
                if (info.CanWrite)
                {
                    var val = info.GetValue(UnprotectedObj);
                    var Protected = dataProtector.Protect(Convert.FromBase64String(val.ToString()));
                    info.SetValue(UnprotectedObj, Convert.ToBase64String(Protected));
                }               
            }
            return UnprotectedObj;
        }
        public static T SanitizeObj(T Unsanitized, IHtmlSanitizer sanitizer)
        {
            foreach(PropertyInfo info in typeof(T).GetProperties())
            {
                if(info.PropertyType == typeof(string))
                {
                    string val = (string)info.GetValue(Unsanitized);
                    sanitizer.Sanitize(val);
                }
            }
            return Unsanitized;
        }
        public static async Task<string> SaveIMG(IFormFile PFP)
        {
            string Folder = Path.Combine("C:\\Trivia_Game\\WhatsAppClone\\Web Service", "wwwroot", "Users");
            string IMG = Guid.NewGuid().ToString() + "_" + Path.GetFileName(PFP.FileName);
            var Save = Path.Combine(Folder, IMG);
            using (var stream = new FileStream(Save, FileMode.Create))
            {
                await PFP.CopyToAsync(stream);
            }
            return IMG;
        }

        //check the image for any malicious bites/types like .exe

        public static bool CheckValidIMG(IFormFile pfp)
        {
            string[] Mimes = { "image/jpg", "image/png", "image/jpeg" };
            if (!Mimes.Contains(pfp.FileName))
            {
                return false;
            }
            byte[] FileHeader = new byte[8];
            using (var stream = pfp.OpenReadStream())
            using (BinaryReader BinReader = new BinaryReader(stream))
            {
                int bytesRead = BinReader.Read(FileHeader, 0, FileHeader.Length);
                if (bytesRead < 4)
                {
                    return false;
                }
                
            }
            if (FileHeader[0] == 0x89 && FileHeader[1] == 0x50 &&
                FileHeader[2] == 0x4E && FileHeader[3] == 0x47)
            {
                return true;
            }
            if (FileHeader[0] == 0xFF && FileHeader[1] == 0xD8 && FileHeader[2] == 0xFF)
            {
                return true;
            }
            return false;
        }
    }
}
