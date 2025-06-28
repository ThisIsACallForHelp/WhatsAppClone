using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public MainPage GetMainPage(string userID = "Nmx", string ChatID = "FR", bool IsGroup = false)
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

        //[HttpGet]
        //public MainPage GetArchivedChats(string ID)
        //{
        //    List<Chat> chatList = new List<Chat>();
        //    List<GroupChat> groupChatList = new List<GroupChat>();
        //    List<Message> messageList = new List<Message>();
        //    try
        //    {
        //        this.dbContext.OpenConnection();
        //        chatList = this.UOW.ChatRepository.GetArchived(ID);
        //    }
        //}
    }
}
