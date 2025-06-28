

namespace Web_Service
{
    public class ModelFactory
    {
        AttachmentsCreator attachmentsCreator;
        ChatCreator chatCreator;
        GroupChatCreator groupChatCreator;
        MessageCreator messageCreator;
        ReadMessageCreator readMessageCreator;
        UserCreator userCreator;
        ArchiveCreator archiveCreator;
        JoinedCreator joinedCreator;
        public AttachmentsCreator AttachmentsCreator
        {
            get
            {
                if (this.attachmentsCreator == null)
                {
                    this.attachmentsCreator = new AttachmentsCreator();
                }
                return this.attachmentsCreator;
            }
        }

        public ChatCreator ChatCreator
        {
            get
            {
                if (this.chatCreator == null)
                {
                    this.chatCreator = new ChatCreator();
                }
                return this.chatCreator;
            }
        }

        public GroupChatCreator GroupChatCreator
        {
            get
            {
                if (this.groupChatCreator == null)
                {
                    this.groupChatCreator = new GroupChatCreator();
                }
                return this.groupChatCreator;
            }
        }

        public MessageCreator MessageCreator
        {
            get
            {
                if (this.messageCreator == null)
                {
                    this.messageCreator = new MessageCreator();
                }
                return this.messageCreator;
            }
        }

        public ReadMessageCreator ReadMessageCreator
        {
            get
            {
                if (this.readMessageCreator == null)
                {
                    this.readMessageCreator = new ReadMessageCreator();
                }
                return this.readMessageCreator;
            }
        }

        public UserCreator UserCreator
        {
            get
            {
                if (this.userCreator == null)
                {
                    this.userCreator = new UserCreator();
                }
                return this.userCreator;
            }
        }

        public ArchiveCreator ArchiveCreator
        {
            get
            {
                if (this.archiveCreator == null)
                {
                    this.archiveCreator = new ArchiveCreator();
                }
                return this.archiveCreator;
            }
        }

        public JoinedCreator JoinedCreator
        {
            get
            {
                if (this.joinedCreator == null)
                {
                    this.joinedCreator = new JoinedCreator();
                }
                return this.joinedCreator;

            }
        }
    }
}

