using API;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
using NuGet.Protocol.Plugins;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Web_Service;
using static System.Net.Mime.MediaTypeNames;
namespace FrutigerWebApp
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetChats(string UserID = "Nmx", string? ChatID = "FR", bool IsGroup = false)
        {
            Client<MainPage> Client = new Client<MainPage>()
            {
                Path = "api/User/GetMainPage",
                Host = "localhost",
                Port = 7189,
                Schema = "https"
            };
            Client.AddParams("UserID", UserID);
            Client.AddParams("ChatID", ChatID);
            Client.AddParams("IsGroup", IsGroup.ToString());
            return View(await Client.GetAsync());
        }

        [HttpGet]
        public IActionResult Intro()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(User user, IFormFile PFP)
        {
            string PathToIMG = string.Empty;
            string Folder = Path.Combine("C:\\Trivia_Game\\WhatsAppClone\\Web Service", "wwwroot", "Users");
            string IMG = Guid.NewGuid().ToString() + "_" + Path.GetFileName(PFP.FileName);
            var Save = Path.Combine(Folder, IMG);
            using (var stream = new FileStream(Save, FileMode.Create))
            {
                await PFP.CopyToAsync(stream);
            }
            user.Avatar = IMG;
            Client<User> client = new Client<User>()
            {
                Schema = "https",
                Host = "localhost",
                Port = 7189,
                Path = "api/User/Register"
            };
            return View(await client.PostAsync(user));
        }
        [HttpGet]
        public User GetUser(string ID)
        {
            Client<User> client = new Client<User>()
            {
                Host = "localhost",
                Port = 7189,
                Schema = "https",
                Path = "api/User/GetUser"
            };
            client.AddParams("UserID", ID);
            return client.GetAsync().Result;
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(string Text, string SenderID)
        {
            //ts should work 
            Client<Data.Message> client = new Client<Data.Message>()
            {
                Host = "localhost",
                Port = 7189,
                Schema = "https",
                Path = "api/User/SendMessage"
            };            
            DHEncryption dh = new DHEncryption();
            ECDsaCng dsa = new ECDsaCng(CngKey.Create(CngAlgorithm.ECDsaP256));
            dsa.HashAlgorithm = CngAlgorithm.Sha256;
            User user = GetUser(SenderID);
            byte[] SingningPublicKey = dsa.Key.Export(CngKeyBlobFormat.EccPublicBlob);
            byte[] PublicKey = DHEncryption.DeriveSharedKey(Convert.FromBase64String(user.RecipientPublicKeyBase64));
            byte[] SenderSigningPublicKey = Convert.FromBase64String(user.RecipientSigningKeyBase64);
            byte[] hmac, iv;
            //encrypted text
            byte[] CipherText = DHEncryption.EncryptMessage(Text, PublicKey, out iv, out hmac);
            //Sign the data 
            byte[] Signature = dsa.SignData(CipherText);
            using (ECDsaCng verifier = new ECDsaCng(CngKey.Import(SingningPublicKey, CngKeyBlobFormat.EccPublicBlob)))
            {
                verifier.HashAlgorithm = CngAlgorithm.Sha256;
                bool valid = verifier.VerifyData(CipherText, Signature);
                Console.WriteLine("Signature valid? " + valid);
            }
            //gjifodsa
            Data.Message message = new Data.Message
            {
                SenderID = SenderID,
                CipherTextBase64 = Convert.ToBase64String(CipherText),
                SenderPublicKeyBase64 = Convert.ToBase64String(PublicKey),
                SenderSigningKeyBase64 = Convert.ToBase64String(SenderSigningPublicKey),
                SignatureBase64 = Convert.ToBase64String(Signature),
                IVBase64 = Convert.ToBase64String(iv),
                HmacBase64 = Convert.ToBase64String(hmac),
                Attachments = "TEST",
                SentAt = DateTime.Now,
                ChatID = "TEST",
                ID = Guid.NewGuid().ToString()
            };
            Console.WriteLine($"SignatureBase64.Length -> {message.SignatureBase64.Length}");
            Console.WriteLine($"IVBase64.Length -> {message.IVBase64.Length}");
            Console.WriteLine($"SenderPublicKeyBase64.Length -> {message.SenderPublicKeyBase64.Length}");
            Console.WriteLine($" SenderSigningKeyBase64.Length -> {message.SenderSigningKeyBase64.Length}");
            Console.WriteLine($"CipherTextBase64.Length -> {message.CipherTextBase64.Length}");
            Console.WriteLine($"HmacBase64.Length -> {message.HmacBase64.Length}");
            if(await client.PostAsync(message))
            {
                message.CipherTextBase64 = DHEncryption.DecryptMessage(CipherText, PublicKey, iv, hmac);
                return View(message);
            }
            return null;
        }

        //i will save the first code sample if anything happens
        //[HttpPost]
        //public async Task<IActionResult> SendMessage(string Text, string SenderID)
        //{
        //    Client<Data.Message> client = new Client<Data.Message>()
        //    {
        //        Host = "localhost",
        //        Port = 7189,
        //        Schema = "https",
        //        Path = "api/User/SendMessage"
        //    };
        //    DHEncryption dh = new DHEncryption();
        //    ECDsaCng dsa = new ECDsaCng(CngKey.Create(CngAlgorithm.ECDsaP256));
        //    dsa.HashAlgorithm = CngAlgorithm.Sha256;
        //    User user = GetUser(SenderID);
        //    byte[] SingningPublicKey = dsa.Key.Export(CngKeyBlobFormat.EccPublicBlob);
        //    byte[] PublicKey = DHEncryption.DeriveSharedKey(Convert.FromBase64String(user.RecipientPublicKeyBase64));
        //    byte[] SenderSigningPublicKey = Convert.FromBase64String(user.RecipientSigningKeyBase64);
        //    byte[] hmac, iv;
        //    //encrypted text
        //    byte[] CipherText = DHEncryption.EncryptMessage(Text, PublicKey, out iv, out hmac);
        //    //Sign the data 
        //    byte[] Signature = dsa.SignData(CipherText);
        //    using (ECDsaCng verifier = new ECDsaCng(CngKey.Import(SingningPublicKey, CngKeyBlobFormat.EccPublicBlob)))
        //    {
        //        verifier.HashAlgorithm = CngAlgorithm.Sha256;
        //        bool valid = verifier.VerifyData(CipherText, Signature);
        //        Console.WriteLine("Signature valid? " + valid);
        //    }
        //    //gjifodsa
        //    Data.Message message = new Data.Message
        //    {
        //        SenderID = SenderID,
        //        CipherTextBase64 = Convert.ToBase64String(CipherText),
        //        SenderPublicKeyBase64 = Convert.ToBase64String(PublicKey),
        //        SenderSigningKeyBase64 = Convert.ToBase64String(SenderSigningPublicKey),
        //        SignatureBase64 = Convert.ToBase64String(Signature),
        //        IVBase64 = Convert.ToBase64String(iv),
        //        HmacBase64 = Convert.ToBase64String(hmac),
        //        Attachments = "TEST",
        //        SentAt = DateTime.Now,
        //        ChatID = "TEST",
        //        ID = Guid.NewGuid().ToString()
        //    };
        //    Console.WriteLine($"SignatureBase64.Length -> {message.SignatureBase64.Length}");
        //    Console.WriteLine($"IVBase64.Length -> {message.IVBase64.Length}");
        //    Console.WriteLine($"SenderPublicKeyBase64.Length -> {message.SenderPublicKeyBase64.Length}");
        //    Console.WriteLine($" SenderSigningKeyBase64.Length -> {message.SenderSigningKeyBase64.Length}");
        //    Console.WriteLine($"CipherTextBase64.Length -> {message.CipherTextBase64.Length}");
        //    Console.WriteLine($"HmacBase64.Length -> {message.HmacBase64.Length}");
        //    if (await client.PostAsync(message))
        //    {

        //    }

        //}




        [HttpGet]
        public async Task<IActionResult> SignInViaQR()
        {
            Client<QRCode> client = new Client<QRCode>()
            {
                Schema = "https",
                Host = "localhost",
                Port = 7189,
                Path = "api/User/GetQR"
            };
            QRCode QrCode = new QRCode()
            {
                QR_Code = await client.GetQR()
            };
            return View(QrCode);
        }

        [HttpGet]
        public async Task<IActionResult> QRCodeAuth(string Token)
        {
            Client<bool> client = new Client<bool>()
            {
                Path = "api/User/QRAuth"
            };
            client.AddParams("Token", Token);
            if (await client.GetAsync())
            {
                return RedirectToAction();
            }
            ViewBag.Error = true;
            return RedirectToAction();
        }
    }
}
