using Khernet.UI.Media;

namespace Khernet.UI
{
    public class UserChatContext : BaseModel
    {
        #region Properties

        public UserItemViewModel User { get; private set; }

        /// <summary>
        /// The current chat model viewed by user 
        /// </summary>
        public ChatMessageItemViewModel CurrentChatModel { get; set; }

        /// <summary>
        /// The fisrt chat message displayed in chat list
        /// </summary>
        public ChatMessageItemViewModel FirstViewChatModel { get; set; }

        private ReplyMessageViewModel replyMessage;

        /// <summary>
        /// Stores the first index if unread message list
        /// </summary>
        public int FirstUnreadMessageIndex { get; set; }

        /// <summary>
        /// Pending message to be sent
        /// </summary>
        public byte[] DraftMessage { get; set; }

        /// <summary>
        /// The format of draft message
        /// </summary>
        public MessageType DraftMessageFormat { get; set; }

        public ReplyMessageViewModel ReplyMessage
        {
            get => replyMessage;
            set
            {
                if (replyMessage != value)
                {
                    replyMessage = value;
                    OnPropertyChanged(nameof(ReplyMessage));
                }
            }
        }

        public ChatMessageItemViewModel ResendMessage
        {
            get;
            set;
        }

        public UserChatContext(UserItemViewModel user)
        {
            User = user;
            DraftMessageFormat = MessageType.Html;
        }

        #endregion
    }
}
