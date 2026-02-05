using API;
using Data;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
using NuGet.Common;
using NuGet.Protocol.Plugins;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Twilio.Rest.Events.V1.Sink;
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
        [HttpGet]
        public async Task<IActionResult> VerifyQR(string Token)
        {
            ComplexHelper<string>.UserIP = HttpContext.Connection.RemoteIpAddress?.ToString();
            ComplexHelper<string>.UserAgent = Request.Headers["User-Agent"];
            Client<Data.Token> Client = new Client<Data.Token>()
            {
                Path = "api/User/GetToken",
                Host = "localhost",
                Port = 7189,
                Schema = "https"
            };
            Client<Data.Token> DeleteToken = new Client<Data.Token>()
            {
                Path = "api/User/DeleteToken",
                Host = "localhost",
                Port = 7189,
                Schema = "https"
            };
            Client<User> LoggedUser = new Client<User>()
            {
                Host = "localhost",
                Port = 7189,
                Schema = "https",
                Path = "api/User/GetDetails"
            };
            HttpContext.Session.SetString("QR_VER", "1");
            Client.AddParams("TokenStr", Token);
            Data.Token token = await Client.GetAsync();
            if(token == null)
            {
                await DeleteToken.PostAsync(token);
                return BadRequest();
            }
            if (token.ExpiresAt < DateTime.UtcNow)
            {
                await DeleteToken.PostAsync(token);
                return Unauthorized("Expired");
            }
            if(token.BrowserConnectID != ComplexHelper<string>.SessionID)
            {
                await DeleteToken.PostAsync(token);
                return BadRequest();
            }
            token.AuthUserID = "gjifodsa";
            HttpContext.Session.SetString("UserID", "gjifodsa");
            LoggedUser.AddParams("ID", token.AuthUserID);
            User user = await LoggedUser.GetAsync();
            await DeleteToken.PostAsync(token);
            if (user == null)
            {
                return BadRequest();
            }
            await ComplexHelper<string>.SendConfirmation();

            return RedirectToAction("GetChats", "User", new { ChatID = "", UserID = user.ID, IsGroup = false });
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
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
            ComplexHelper<string>.SessionID = HttpContext.Session.Id;
            client.AddParams("SessID", ComplexHelper<string>.SessionID);
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

        [HttpGet]

        public async Task<bool> VerifyMail(string Prev)
        {
            string CurrIP = HttpContext.Connection.RemoteIpAddress?.ToString();
            string UserAgent = Request.Headers["User-Agent"];

            if(CurrIP != ComplexHelper<string>.UserIP)
            {
                DropConnection();
            }
            if(UserAgent != ComplexHelper<string>.UserAgent)
            {
                DropConnection();
            }
        }

        [HttpGet]
        public void DropConnection()
        {

        }
    }
}
