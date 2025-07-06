using API;
using Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Web_Service;
using System.IO;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
namespace FrutigerWebApp
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Chats")]
        public async Task<IActionResult> GetChats(string UserID)
        {
            Client<MainPage> Client = new Client<MainPage>()
            {
                Path = "api/User/GetMainPage"
            };
            Client.AddParams("UserID", UserID);
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
        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            Client<Message> client = new Client<Message>()
            {
                Path = "api/User/SendMessage"
            };
            if (await client.PostAsync(message))
            {
                byte[] sharedKey = DHEncryption.DeriveSharedKey(Convert.FromBase64String(message.SenderPublicKeyBase64));
                if (!DHEncryption.VerifySignature(Convert.FromBase64String(message.CipherTextBase64), Convert.FromBase64String(message.SignatureBase64), Convert.FromBase64String(message.SenderSigningKeyBase64)))
                {
                    throw new Exception("Invalid signature");
                }
                string plainText = DHEncryption.DecryptMessage(Convert.FromBase64String(message.CipherTextBase64), sharedKey, Convert.FromBase64String(message.IVBase64), Convert.FromBase64String(message.HmacBase64));
                return View(message);
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
