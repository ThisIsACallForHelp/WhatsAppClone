using API;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
using NuGet.Protocol.Plugins;
using System.IO;
using System.Net;
using Ganss.Xss;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Web_Service;
using static System.Net.Mime.MediaTypeNames;
namespace FrutigerWebApp
{
    public class UserController : Controller
    {
        private readonly IHtmlSanitizer _XSS_Protector;
        private readonly IDataProtector _QueryDataProtector;
        private readonly IDataProtector _MessageProtector;
        public UserController(IDataProtectionProvider dataProtectionProvider, IHtmlSanitizer sanitizer)
        {
            this._QueryDataProtector = dataProtectionProvider.CreateProtector("Query.Param.Protector");
            this._XSS_Protector = sanitizer;
            this._MessageProtector = dataProtectionProvider.CreateProtector("Message.Protector");
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetChats(string? ChatID, string UserID = "gjifodsa", bool IsGroup = false)
        {
            Client<MainPage> Client = new Client<MainPage>()
            {
                Path = "api/User/GetMainPage",
                Host = "localhost",
                Port = 7189,
                Schema = "https"
            };
            _XSS_Protector.Sanitize(ChatID);
            _XSS_Protector.Sanitize(UserID);
            //i need to make this code cleaner 
            ComplexHelper<string>.ChatID = ChatID;
            Console.WriteLine("chat id -> " + ChatID);
            Client.AddParams("UserID", UserID);
            Client.AddParams("ChatID", ChatID);
            Client.AddParams("IsGroup", IsGroup.ToString());
            Console.WriteLine("chat id -> " + ComplexHelper<string>.ChatID);
            MainPage Main = await Client.GetAsync();
            if (Main.Messages != null && Main.Messages.Count > 0)
            {
                List<Data.Message> Decrypted = new List<Data.Message>();
                foreach (Data.Message message in Main.Messages)
                {
                    byte[] PublicKey = Convert.FromBase64String(message.SenderPublicKeyBase64);
                    byte[] hmac = Convert.FromBase64String(message.HmacBase64);
                    byte[] iv = Convert.FromBase64String(message.IVBase64);
                    byte[] CipherText = Convert.FromBase64String(message.CipherTextBase64);
                    message.CipherTextBase64 = DHEncryption.DecryptMessage(CipherText, PublicKey, iv, hmac);
                    Decrypted.Add(message);
                }
                Main.Messages = Decrypted;
            }
            if (Main.Convo != null && Main.Convo.Count > 0)
            {
                List<Data.Message> Decrypted = new List<Data.Message>();
                foreach (Data.Message message in Main.Convo)
                {
                    byte[] PublicKey = Convert.FromBase64String(message.SenderPublicKeyBase64);
                    byte[] hmac = Convert.FromBase64String(message.HmacBase64);
                    byte[] iv = Convert.FromBase64String(message.IVBase64);
                    byte[] CipherText = Convert.FromBase64String(message.CipherTextBase64);
                    message.CipherTextBase64 = DHEncryption.DecryptMessage(CipherText, PublicKey, iv, hmac);
                    Decrypted.Add(message);
                }
                Main.Convo = Decrypted;
            }
            return View(Main);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(User user, IFormFile PFP)
        {
            string[] Mimes = { "image/jpg", "image/png", "image/jpeg" };
            if(PFP == null || PFP.Length == 0 || !ComplexHelper<IFormFile>.CheckValidIMG(PFP))
            {
                return null;
            }            
            user.Avatar = await ComplexHelper<string>.SaveIMG(PFP);
            Client<User> client = new Client<User>()
            {
                Schema = "https",
                Host = "localhost",
                Port = 7189,
                Path = "api/User/Register"
            };
            User UInfo = await client.Register(user);
            if(UInfo != null)
            {
                HttpContext.Session.SetString("UserID", UInfo.ID);
                HttpContext.Session.SetString("PublicKey", UInfo.RecipientPublicKeyBase64);
                HttpContext.Session.SetString("PublicSigningKey", UInfo.RecipientSigningKeyBase64);

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string Text, string SenderID = "gjifodsa")
        {
            //ts should work 
            //i should simplify ts 
            _XSS_Protector.Sanitize(Text);
            string PublicKey = HttpContext.Session.GetString("PublicKey");
            string PublicSigningKey = HttpContext.Session.GetString("PublicSigningKey");
            Client<Data.Message> client = new Client<Data.Message>()
            {
                Host = "localhost",
                Port = 7189,
                Schema = "https",
                Path = "api/User/SendMessage"
            };
            byte[] iv, Hmac, ByteText, BytePublicKey;
            Data.Message msg = ComplexHelper<Data.Message>.SendMSG(SenderID, Text, PublicKey, PublicSigningKey, 
                                                out iv, out Hmac, out ByteText, out BytePublicKey);           
            if (await client.PostAsync(msg))
            {
                msg.CipherTextBase64 = DHEncryption.DecryptMessage(ByteText, BytePublicKey, iv, Hmac);
                return RedirectToAction("GetChats", "User", new { ChatID = ComplexHelper<string>.ChatID, UserID = SenderID });
                //giving the function the user's ID is not necessary
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
            Client<User> client = new Client<User>()
            {
                Host = "localhost",
                Port = 7189,
                Schema = "https",
                Path = "api/User/QRAuth"
            };
            client.AddParams("Token", Token);
            User user = await client.GetAsync();
            if (user != null)
            {
                HttpContext.Session.SetString("UserID", user.ID);
                HttpContext.Session.SetString("PublicKey", user.RecipientPublicKeyBase64);
                HttpContext.Session.SetString("PublicSigningKey", user.RecipientSigningKeyBase64);
                return RedirectToAction();
            }
            ViewBag.Error = true;
            return RedirectToAction();
        }
    }
}
