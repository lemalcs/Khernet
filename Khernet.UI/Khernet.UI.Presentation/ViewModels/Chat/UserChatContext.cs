using Khernet.UI.Media;
using System.Collections.Generic;

namespace Khernet.UI
{
    public class UserChatContext : BaseModel
    {
        #region Properties

        /// <summary>
        /// Total unread message for current chat list
        /// </summary>
        private int unreadMessagesNumber;

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

        public int UnreadMessagesNumber
        {
            get => unreadMessagesNumber;
            private set
            {
                if (unreadMessagesNumber != value)
                {
                    unreadMessagesNumber = value;
                    OnPropertyChanged(nameof(UnreadMessagesNumber));
                }
            }
        }

        /// <summary>
        /// Stores the first index if unread message list
        /// </summary>
        public int FirstUnreadMessageIndex { get; set; }

        /// <summary>
        /// Pendding message to be sent
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

        public List<ChatMessageItemViewModel> SentMessages { get; private set; }

        public UserChatContext(UserItemViewModel user)
        {
            User = user;
            DraftMessageFormat = MessageType.Html;
        }

        #endregion

        public void SetUnreadMessage(int quantity)
        {
            if (quantity >= 0)
            {
                UnreadMessagesNumber = quantity;
            }

            if (UnreadMessagesNumber == 0)
                User.ClearUnReadMessages();
        }

        public void DecreaseUnreadMessage()
        {
            if (UnreadMessagesNumber > 0)
                UnreadMessagesNumber--;

            if (UnreadMessagesNumber == 0)
                User.ClearUnReadMessages();
        }

        /// <summary>
        /// Store message that current user sent.
        /// </summary>
        /// <param name="chatMessage">The model of chat message</param>
        public void AddSentMessage(ChatMessageItemViewModel chatMessage)
        {
            if (SentMessages == null)
                SentMessages = new List<ChatMessageItemViewModel>();

            if (!SentMessages.Contains(chatMessage))
                SentMessages.Add(chatMessage);
        }
    }
}
