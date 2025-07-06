namespace Web_Service
{
    public class UOW
    {
        AttachmentRepository attachmentRepository;
        ChatRepository chatRepository;
        GroupChatRepository groupChatRepository;
        MessageRepository messageRepository;
        ReadMessageRepository readMessageRepository;
        UserRepository userRepository;
        ArchiveRepository archiveRepository;
        JoinedRepository joinedRepository;
        TokenRepository tokenRepository;

        DBContext dbContext;
        public UOW(DBContext dbcontext)
        {
            this.dbContext = DBContext.GetInstance();
        }
        public TokenRepository TokenRepository
        {
            get
            {
                if (this.tokenRepository == null)
                {
                    this.tokenRepository = new TokenRepository(this.dbContext);
                }
                return this.tokenRepository;
            }
        }
        public AttachmentRepository AttachmentRepository 
        {
            get
            {
                if(this.attachmentRepository == null)
                {
                    this.attachmentRepository = new AttachmentRepository(this.dbContext);
                }
                return this.attachmentRepository;
            }
        }
        public ChatRepository ChatRepository
        {
            get
            {
                if (this.chatRepository == null)
                {
                    this.chatRepository = new ChatRepository(this.dbContext);
                }
                return this.chatRepository;
            }
        }

        public GroupChatRepository GroupChatRepository
        {
            get
            {
                if (this.groupChatRepository == null)
                {
                    this.groupChatRepository = new GroupChatRepository(this.dbContext);
                }
                return this.groupChatRepository;
            }
        }

        public MessageRepository MessageRepository
        {
            get
            {
                if (this.messageRepository == null)
                {
                    this.messageRepository = new MessageRepository(this.dbContext);
                }
                return this.messageRepository;
            }
        }

        public ReadMessageRepository ReadMessageRepository
        {
            get
            {
                if (this.readMessageRepository == null)
                {
                    this.readMessageRepository = new ReadMessageRepository(this.dbContext);
                }
                return this.readMessageRepository;
            }
        }
        public UserRepository UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new UserRepository(this.dbContext);
                }
                return this.userRepository;
            }
        }

        public ArchiveRepository ArchiveRepository
        {
            get
            {
                if (this.archiveRepository == null)
                {
                    this.archiveRepository = new ArchiveRepository(this.dbContext);
                }
                return this.archiveRepository;
            }
        }

        public JoinedRepository JoinedRepository
        {
            get
            {
                if (this.joinedRepository == null)
                {
                    this.joinedRepository = new JoinedRepository(this.dbContext);
                }
                return this.joinedRepository;
            }
        }
    }
}
