using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace Web_Service
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        DBContext dbContext;
        UOW UOW;

        public UserController()
        {
            this.dbContext = DBContext.GetInstance();
            this.UOW = new UOW(this.dbContext);
        }

        [HttpGet("Main")]
        //works
        public MainPage GetMainPage(string userID , string ChatID, bool IsGroup = false)
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
                this.dbContext.CloseConnection();   
            }
        }
        //check that 
        [HttpGet("Search")]
        public Search SearchBarResult(string Request,string UserID)
        {
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
                this.dbContext.CloseConnection();
            }
        }

        [HttpGet("Profile_Info")]
        //no way this wont work
        public User GetDetails(string ID)
        {
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
                this.dbContext.CloseConnection();
            }
        }
        [HttpPost("Register")]
        //should work 
        public int Register(User user)
        {
            try
            {
                this.dbContext.OpenConnection();
                return this.UOW.UserRepository.Register(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
            finally
            {
                this.dbContext.CloseConnection(); 
            }
        }

        [HttpGet]
        //check that
        public MainPage GetArchivedChats(string UserID)
        {
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
                this.dbContext.CloseConnection(); 
            }
        }

        [HttpPost]
        //should work, check that 
        public bool ArchiveChat(Chat chat, string UserID)
        {
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
                this.dbContext.CloseConnection(); 
            }
        }
        [HttpPost]
        public bool DeleteChat(Chat chat)
        {
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
                this.dbContext.CloseConnection();
            }
        }

        [HttpGet]
        public IActionResult GetQR()
        {
            try
            {
                Token token = new Token()
                {
                    ID = QRCode_Creator.GetToken(),
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(1),
                    UserID = null
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
                this.dbContext.CloseConnection();
            }
        }


        [HttpPost]
        public bool SendMessage(Message message)
        {
            try
            {
                this.dbContext.OpenConnection();
                byte[] CipherText = Convert.FromBase64String(message.CipherTextBase64);
                byte[] SenderSigningKey = Convert.FromBase64String(message.SenderSigningKeyBase64);
                byte[] Signature = Convert.FromBase64String(message.SignatureBase64);
                if (!DHEncryption.VerifySignature(CipherText, Signature, SenderSigningKey))
                {
                    throw new CryptographicException("Signature Verification Failed");
                }
                return this.UOW.MessageRepository.Create(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                this.dbContext.CloseConnection();
            }
        }
    }
}
