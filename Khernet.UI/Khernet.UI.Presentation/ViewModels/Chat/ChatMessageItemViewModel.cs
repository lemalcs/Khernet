using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// The state of chat messages
    /// </summary>
    public enum ChatMessageState
    {
        /// <summary>
        /// The message is not ready to be used
        /// </summary>
        UnCommited = -1,

        /// <summary>
        /// The message is pendding to be sent
        /// </summary>
        Pendding = 0,

        /// <summary>
        /// The message was sent or received successfully
        /// </summary>
        Processed = 1,

        /// <summary>
        /// There is an error sending or receiveing the message
        /// </summary>
        Error = 2
    }
    public abstract class ChatMessageItemViewModel : BaseModel
    {
        #region Properties

        private readonly object syncLock = new object();

        /// <summary>
        /// The view model of user for who sent message or received message;
        /// </summary>
        private UserItemViewModel user;

        /// <summary>
        /// The date that messages was sent
        /// </summary>
        private DateTimeOffset sendDate;

        /// <summary>
        /// Indicates if it is a message send by current user
        /// </summary>
        private bool isSelfMessage;

        /// <summary>
        /// The identifier for chat message
        /// </summary>
        private int id;

        /// <summary>
        /// The replied message 
        /// </summary>
        private ReplyMessageViewModel replyMessage;

        /// <summary>
        /// The state of chat message
        /// </summary>
        private ChatMessageState state;

        /// <summary>
        /// Indicates is message is loaded and ready to read
        /// </summary>
        private bool isMessageLoaded;

        /// <summary>
        /// Indicates if this message was read
        /// </summary>
        private bool isRead;

        /// <summary>
        /// The universal identifier of chat message
        /// </summary>
        public string UID { get; protected set; }

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

        public UserItemViewModel User
        {
            get => user;
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged(nameof(User));
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
        /// Reply and chat message
        /// </summary>
        public ICommand ReplyCommand { get; set; }

        /// <summary>
        /// Resend a chat message
        /// </summary>
        public ICommand ResendCommand { get; set; }

        #endregion

        /// <summary>
        /// Gets a symmary of this message
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public abstract ReplyMessageViewModel GetMessageSummary(MessageDirection operation);

        /// <summary>
        /// Sends this message to other user
        /// </summary>
        public abstract void ProcessResend();

        /// <summary>
        /// Syncronizes the update to <see cref="State"/> field of chat message
        /// </summary>
        /// <param name="state"></param>
        public void SetChatState(ChatMessageState state)
        {
            lock (syncLock)
            {
                State = state;
            }
        }

        /// <summary>
        /// Checks is file is saved on database and ready to be read
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        protected bool IsReadyMessage(object obj)
        {
            return IsMessageLoaded;
        }
    }
}
