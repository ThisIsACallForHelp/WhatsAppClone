using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web_Service
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        DBContext dbContext;
        UOW UOW;
        //USER FOR TESTING: gjifodsa
        public UserController()
        {
            this.dbContext = DBContext.GetInstance();
            this.UOW = new UOW(this.dbContext);
        }

        [HttpGet]
        //works
        public MainPage GetMainPage(string userID , string ChatID = null, bool IsGroup = false)
        {
            List<Message> Convo = new List<Message>();
            List<Message> LastMessages = new List<Message>();
            List<GroupChat> groupChats = new List<GroupChat>();
            List<Chat> chats = new List<Chat>();
            MainPage mainPage = new MainPage();
            try
            {
                this.dbContext.OpenConnection();
                groupChats = this.UOW.GroupChatRepository.GetAllByUserID(userID);
                chats = this.UOW.ChatRepository.GetChatsByUser(userID);
                if (ChatID != null && !IsGroup)
                {
                    Convo = this.UOW.MessageRepository.Conversation(ChatID); 
                }
                else if (ChatID != null && IsGroup)
                {
                    Convo = this.UOW.MessageRepository.GroupConversation(ChatID);
                }
                foreach(var chat in chats)
                {
                    LastMessages.Add(this.UOW.MessageRepository.GetLastMessages(userID, chat.ID));
                }
                foreach (var Group in groupChats)
                {
                    LastMessages.Add(this.UOW.MessageRepository.GetLastGroupMessages(userID, Group.ID));
                }
                mainPage.GroupChats = groupChats;
                mainPage.Messages = LastMessages;
                mainPage.User = this.UOW.UserRepository.GetByID(userID);
                mainPage.Chats = chats;
                mainPage.Convo = Convo;
                return mainPage;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            finally{
                this.dbContext.ClearParameters();
                this.dbContext.CloseConnection();   
            }
        }
        
        [HttpGet]
        public Search SearchBarResult(string Request,string UserID)
        {
            //works
            List<Chat> chats = new List<Chat>();
            List<GroupChat> groups = new List<GroupChat>();
            List<Message> LastMessages = new List<Message>();
            try
            {
                this.dbContext.OpenConnection();
                chats = this.UOW.ChatRepository.GetBySearch(Request, UserID);
                groups = this.UOW.GroupChatRepository.GetGroupBySearch(Request);
                foreach (var chat in chats)
                {
                    LastMessages.Add(this.UOW.MessageRepository.GetLastMessages(UserID, chat.ID));
                }
                foreach (var Group in groups)
                {
                    LastMessages.Add(this.UOW.MessageRepository.GetLastGroupMessages(UserID, Group.ID));
                }
                return new Search {
                    LatestMessages = LastMessages,
                    GroupChats = groups, 
                    Chats = chats
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            finally
            {
                this.dbContext.ClearParameters();
                this.dbContext.CloseConnection();
            }
        }

        [HttpGet]
        public User GetDetails(string ID)
        {
            //works
            try
            {
                this.dbContext.OpenConnection();
                return this.UOW.UserRepository.GetByID(ID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            finally
            {
                this.dbContext.ClearParameters();
                this.dbContext.CloseConnection();
            }
        }
        [HttpPost]
        public string Register(User user)
        {
            //works
            try
            {
                this.dbContext.OpenConnection();
                return this.UOW.UserRepository.Register(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "NULL";
            }
            finally
            {
                this.dbContext.ClearParameters();
                this.dbContext.CloseConnection(); 
            }
        }

        [HttpGet]
        
        public MainPage GetArchivedChats(string UserID)
        {
            //works
            List<Chat> chatList = new List<Chat>();
            List<GroupChat> groupChatList = new List<GroupChat>();
            List<Message> messageList = new List<Message>();
            try
            {
                this.dbContext.OpenConnection();
                chatList = this.UOW.ChatRepository.GetArchived(UserID);
                groupChatList = this.UOW.GroupChatRepository.GetArchived(UserID);               
                foreach (Chat chat in chatList)
                {
                    messageList.Add(this.UOW.MessageRepository.GetLastMessages(UserID, chat.ID));
                }
                foreach(GroupChat chat in groupChatList)
                {
                    messageList.Add(this.UOW.MessageRepository.GetLastGroupMessages(UserID, chat.ID));
                }
                return new MainPage()
                { 
                    Chats = chatList,
                    GroupChats = groupChatList,
                    Messages = messageList
                };
                //but what about Convo?
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            finally 
            {
                this.dbContext.ClearParameters();
                this.dbContext.CloseConnection(); 
            }
        }

        [HttpPost]
        
        public bool ArchiveChat(Chat chat, string UserID)
        {
            //works
            try
            {
                this.dbContext.OpenConnection();
                return this.UOW.ArchiveRepository.Create(new Archive(){ ChatID = chat.ID, UserID = UserID });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally 
            {
                this.dbContext.ClearParameters();
                this.dbContext.CloseConnection(); 
            }
        }
        [HttpPost]
        public bool DeleteChat(Chat chat)
        {
            //works
            try
            {
                this.dbContext.OpenConnection();
                return this.UOW.ChatRepository.DeleteByID(chat.ID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                this.dbContext.ClearParameters();
                this.dbContext.CloseConnection();
            }
        }

        [HttpGet]
        public IActionResult GetQR()
        {
            //works
            try
            {
                this.dbContext.OpenConnection();
                Token token = new Token()
                {
                    ID = QRCode_Creator.GetToken(),
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(1),
                    UserID = "NONE"
                };
                if (!this.UOW.TokenRepository.Create(token))
                {
                    return null;
                }
                byte[] qrBytes = QRCode_Creator.Create(token.ID);
                Bitmap bitmap;
                using (MemoryStream MStream = new MemoryStream(qrBytes))
                {
                    bitmap = new Bitmap(MStream);
                }
                Bitmap styledBitmap = QRCode_Creator.GradientQR(bitmap);
                using (var stream = new MemoryStream())
                {
                    styledBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream.ToArray(), "image/png");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            finally
            {
                this.dbContext.ClearParameters();
                this.dbContext.CloseConnection();
            }
        }


        [HttpPost]
        public bool SendMessage(Message msg)
        {
            //works HERE
            try
            {                              
                this.dbContext.OpenConnection();                
                return this.UOW.MessageRepository.Create(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                this.dbContext.ClearParameters();
                this.dbContext.CloseConnection();
            }
        }
    }
}
