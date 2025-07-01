using Microsoft.AspNetCore.Mvc;
using System.Net;
using Data;
using API; 
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
    }
}
