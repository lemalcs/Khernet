using Khernet.UI.IoC;
using Khernet.UI.Media;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for notifications.
    /// </summary>
    public class NotificationViewModel : BaseModel
    {
        #region Properties

        /// <summary>
        /// The current user.
        /// </summary>
        private UserItemViewModel user;

        /// <summary>
        /// The type of context based on <see cref="Media.MessageType"/>.
        /// </summary>
        private MessageType messageType;

        /// <summary>
        /// Indicates if this notification is visible.
        /// </summary>
        private bool isVisible;

        /// <summary>
        /// The id of message received.
        /// </summary>
        private int idMessage;

        /// <summary>
        /// The type of message that was received.
        /// </summary>
        public MessageType MessageType
        {
            get => messageType;
            set
            {
                if (messageType != value)
                {
                    messageType = value;
                    OnPropertyChanged(nameof(MessageType));
                }
            }
        }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
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

        public int IdMessage
        {
            get => idMessage;
            set
            {
                if (idMessage != value)
                {
                    idMessage = value;
                    OnPropertyChanged(nameof(IdMessage));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to open the received message.
        /// </summary>
        public ICommand OpenChatCommand { get; private set; }

        /// <summary>
        /// Command to close this notification.
        /// </summary>
        public ICommand CloseCommand { get; private set; }

        #endregion

        public NotificationViewModel()
        {
            OpenChatCommand = new UI.RelayCommand(OpenChat);
            CloseCommand = new RelayCommand(Close);
            IsVisible = true;
        }

        private void Close()
        {
            IsVisible = false;
        }

        /// <summary>
        /// Open the chat list of user whom sent the message.
        /// </summary>
        private void OpenChat()
        {
            IoCContainer.Get<UserListViewModel>().SelectUser(User.Token);
            IsVisible = false;

            IoCContainer.UI.ShowWindow();
        }
    }
}
