using Khernet.Services.Messages;
using Khernet.UI.IoC;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// The state of chat messages.
    /// </summary>
    public enum ChatMessageState
    {
        /// <summary>
        /// The message is not ready to be used.
        /// </summary>
        UnCommited = -1,

        /// <summary>
        /// The message is pending to be sent.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// The message was sent or received successfully.
        /// </summary>
        Processed = 1,

        /// <summary>
        /// There is an error sending or receiving the message.
        /// </summary>
        Error = 2
    }
    public abstract class ChatMessageItemViewModel : BaseModel
    {
        #region Properties

        private readonly object syncLock = new object();

        /// <summary>
        /// The view model of user to display in user interface.
        /// </summary>
        private UserItemViewModel displayUser;

        /// <summary>
        /// The user that send the message.
        /// </summary>
        public IIdentity SenderUserId { get; set; }

        /// <summary>
        /// The user that receive the message.
        /// </summary>
        public IIdentity ReceiverUserId { get; set; }

        /// <summary>
        /// The date that messages was sent.
        /// </summary>
        private DateTimeOffset sendDate;

        /// <summary>
        /// Indicates if it is a message send by current user.
        /// </summary>
        private bool isSelfMessage;

        /// <summary>
        /// The identifier for chat message.
        /// </summary>
        private int id;

        /// <summary>
        /// The replied message.
        /// </summary>
        private ReplyMessageViewModel replyMessage;

        /// <summary>
        /// The state of chat message.
        /// </summary>
        private ChatMessageState state;

        /// <summary>
        /// Indicates is message is loaded and ready to read.
        /// </summary>
        private bool isMessageLoaded;

        /// <summary>
        /// Indicates if this message was read.
        /// </summary>
        private bool isRead;

        /// <summary>
        /// The universal identifier of chat message.
        /// </summary>
        public string UID { get; protected set; }

        /// <summary>
        /// The number of ticks that indicates when message was sent.
        /// </summary>
        public long TimeId { get; protected set; }

        public DateTimeOffset SendDate
        {
            get
            {
                return sendDate;
            }

            set
            {
                if (sendDate != value)
                {
                    sendDate = value;
                    OnPropertyChanged(nameof(SendDate));
                }
            }
        }

        public bool IsSentByMe
        {
            get
            {
                return isSelfMessage;
            }

            set
            {
                if (isSelfMessage != value)
                {
                    isSelfMessage = value;
                    OnPropertyChanged(nameof(IsSentByMe));
                }
            }
        }

        public int Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

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

        public UserItemViewModel DisplayUser
        {
            get => displayUser;
            set
            {
                if (displayUser != value)
                {
                    displayUser = value;
                    OnPropertyChanged(nameof(DisplayUser));
                }
            }
        }

        public ChatMessageState State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public bool IsMessageLoaded
        {
            get => isMessageLoaded;
            protected set
            {
                if (isMessageLoaded != value)
                {
                    isMessageLoaded = value;
                    OnPropertyChanged(nameof(IsMessageLoaded));
                }
            }
        }

        public bool IsRead
        {
            get => isRead;
            set
            {
                if (isRead != value)
                {
                    isRead = value;
                    OnPropertyChanged(nameof(IsRead));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Reply and chat message.
        /// </summary>
        public ICommand ReplyCommand { get; set; }

        /// <summary>
        /// Resend a chat message.
        /// </summary>
        public ICommand ResendCommand { get; set; }
        #endregion

        /// <summary>
        /// Gets a summary of this message.
        /// </summary>
        /// <param name="operation">The purpose of message summary.</param>
        /// <returns>A <see cref="ReplyMessageViewModel"/> object.</returns>
        public abstract ReplyMessageViewModel GetMessageSummary(MessageDirection operation);

        /// <summary>
        /// Sends this message to other user.
        /// </summary>
        public abstract void ProcessResend();

        /// <summary>
        /// Synchronizes the update to <see cref="State"/> field of chat message.
        /// </summary>
        /// <param name="state">The state of chat message.</param>
        public void SetChatState(ChatMessageState state)
        {
            lock (syncLock)
            {
                State = state;
            }
        }

        /// <summary>
        /// Checks is file is saved on database and ready to be read.
        /// </summary>
        /// <returns>True if message is read to used to otherwise false.</returns>
        [DebuggerStepThrough]
        protected bool IsReadyMessage()
        {
            return IsMessageLoaded;
        }

        /// <summary>
        /// Load the chat message.
        /// </summary>
        /// <param name="messageItem">The header of chat message.</param>
        public abstract void Load(MessageItem messageItem);
    }
}
