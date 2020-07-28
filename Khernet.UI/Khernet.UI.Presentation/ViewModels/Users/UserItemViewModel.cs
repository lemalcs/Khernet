using Khernet.UI.IoC;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Khernet.UI
{
    public class UserItemViewModel : BaseModel,IIdentity
    {
        #region Properties
        /// <summary>
        /// The name initials for defualt picture profile
        /// </summary>
        private string initials;

        /// <summary>
        /// The Hexadecimal value for color, for example: F5A2D8
        /// </summary>
        private string colorHex;

        /// <summary>
        /// The state of user
        /// </summary>
        private string state;

        /// <summary>
        /// The group of user
        /// </summary>
        private string group;

        /// <summary>
        /// The slogan of user
        /// </summary>
        private string slogan;

        /// <summary>
        /// Indicates if user is selected
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// The username
        /// </summary>
        private string username;

        /// <summary>
        /// Gets the number of unread messages
        /// </summary>
        private int unreadMessages;

        /// <summary>
        /// Indicates if user is writing a message
        /// </summary>
        private bool isWritingMessage;

        /// <summary>
        /// Indicates if user is sending a file
        /// </summary>
        private bool isSendingFile;

        /// <summary>
        /// The HTML format of full name
        /// </summary>
        private string sourceFullName;

        /// <summary>
        /// The HTML format of display name
        /// </summary>
        private string sourceDisplayName;

        private ReadOnlyCollection<byte> fullName;

        private ReadOnlyCollection<byte> displayName;

        private ReadOnlyCollection<byte> avatar;

        public string Initials
        {
            get { return initials; }
            set
            {
                if (initials != value)
                {
                    initials = value;
                    OnPropertyChanged(nameof(Initials));
                }
            }
        }

        /// <summary>
        /// The full name of user (can include emojis)
        /// </summary>
        public ReadOnlyCollection<byte> FullName
        {
            get => fullName;
            private set
            {
                if (fullName != value)
                {
                    fullName = value;
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }

        public string State
        {
            get
            {
                return state;
            }

            set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public string Group
        {
            get
            {
                return group;
            }

            set
            {
                if (group != value)
                {
                    group = value;
                    OnPropertyChanged(nameof(Group));
                }
            }
        }

        public string Slogan
        {
            get
            {
                return slogan;
            }

            set
            {
                if (slogan != value)
                {
                    slogan = value;
                    OnPropertyChanged(nameof(Slogan));
                }
            }
        }

        public string ColorHex
        {
            get { return colorHex; }
            set
            {
                if (colorHex != value)
                {
                    colorHex = value;
                    OnPropertyChanged(nameof(ColorHex));
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        /// <summary>
        /// The token of user
        /// </summary>
        public string Token
        {
            get;
            set;
        }

        /// <summary>
        /// The custon name to display for this user
        /// </summary>
        public ReadOnlyCollection<byte> DisplayName
        {
            get => displayName;
            private set
            {
                if (displayName != value)
                {
                    displayName = value;
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public string Username
        {
            get => username;
            set
            {
                if (username != value)
                {
                    username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public int UnreadMessages
        {
            get => unreadMessages;
            private set
            {
                if (unreadMessages != value)
                {
                    unreadMessages = value;
                    OnPropertyChanged(nameof(UnreadMessages));
                }
            }
        }

        public ReadOnlyCollection<byte> Avatar
        {
            get => avatar;
            private set
            {
                if (avatar != value)
                {
                    avatar = value;
                    OnPropertyChanged(nameof(Avatar));
                }
            }
        }

        public bool IsWritingMessage
        {
            get => isWritingMessage;
            private set
            {
                if (isWritingMessage != value)
                {
                    isWritingMessage = value;
                    OnPropertyChanged(nameof(IsWritingMessage));
                }
            }
        }

        public bool IsSendingFile
        {
            get => isSendingFile;
            private set
            {
                if (isSendingFile != value)
                {
                    isSendingFile = value;
                    OnPropertyChanged(nameof(IsSendingFile));
                }
            }
        }

        /// <summary>
        /// Stores the time when last writing message was received
        /// </summary>
        private DateTime lastWritingTime = DateTime.Now;

        public string SourceFullName
        {
            get => sourceFullName;
            private set
            {
                if (sourceFullName != value)
                {
                    sourceFullName = value;
                    OnPropertyChanged(nameof(SourceFullName));
                }
            }
        }

        public string SourceDisplayName
        {
            get => sourceDisplayName;
            private set
            {
                if (sourceDisplayName != value)
                {
                    sourceDisplayName = value;
                    OnPropertyChanged(nameof(SourceDisplayName));
                }
            }
        }
        #endregion

        /// <summary>
        /// Opens a chat for selected user
        /// </summary>
        public ICommand OpenChatCommand { get; private set; }

        public UserItemViewModel()
        {
            OpenChatCommand = new RelayCommand(OpenChat);
        }

        private void OpenChat()
        {
            IoCContainer.Get<ChatMessageListViewModel>().OpenChatListFor(this);
        }

        /// <summary>
        /// Increase in one the number of unread messages
        /// </summary>
        public void IncreaseUnReadMessages()
        {
            UnreadMessages++;
            IoCContainer.Get<UserListViewModel>().AddUnreadMessages(1);
        }

        /// <summary>
        /// Decrease in one the number of unread messages
        /// </summary>
        public void DecreaseUnReadMessages()
        {
            if (UnreadMessages > 0)
            {
                UnreadMessages--;
                IoCContainer.Get<UserListViewModel>().SubtractUnreadMessages(1);
                IoCContainer.UI.ShowUnreadMessagesNumber(IoCContainer.Get<UserListViewModel>().TotalUnreadMessages);
            }
        }

        /// <summary>
        /// Sets unread message number to zero
        /// </summary>
        public void ClearUnReadMessages()
        {
            UnreadMessages = 0;
            IoCContainer.Get<UserListViewModel>().ClearUnreadMessages();
        }

        /// <summary>
        /// Sets unread message number to zero
        /// </summary>
        public void SetUnReadMessages(int messageQuantity)
        {
            if (messageQuantity < 0)
                throw new ArgumentException("Message quatity must be equals or greater than zero");

            UnreadMessages = messageQuantity;

            IoCContainer.Get<UserListViewModel>().AddUnreadMessages(messageQuantity);
        }

        public void SetAvatarThumbnail(byte[] avatarBytes)
        {
            if (avatarBytes == null || avatarBytes.Length == 0)
                return;

            Avatar = new ReadOnlyCollection<byte>(avatarBytes);
            OnPropertyChanged(nameof(Avatar));
        }

        public void ReadFullName(byte[] fullNameBytes)
        {
            if (fullNameBytes == null)
                return;

            SourceFullName = Encoding.UTF8.GetString(fullNameBytes);

            var name = IoCContainer.UI.ConvertHtmlToDocument(Encoding.UTF8.GetString(fullNameBytes));

            FullName = new ReadOnlyCollection<byte>(name);
            OnPropertyChanged(nameof(FullName));
        }

        public void ReadDisplayName(byte[] displayNameBytes)
        {
            if (displayNameBytes == null)
                return;

            SourceDisplayName = Encoding.UTF8.GetString(displayNameBytes);
            var name = IoCContainer.UI.ConvertHtmlToDocument(Encoding.UTF8.GetString(displayNameBytes));


            DisplayName = new ReadOnlyCollection<byte>(name);
            OnPropertyChanged(nameof(DisplayName));
        }

        /// <summary>
        /// Gets a copy of this object.
        /// </summary>
        /// <returns>A new <see cref="UserItemViewModel"/> object</returns>
        public UserItemViewModel Clone()
        {
            UserItemViewModel user = new UserItemViewModel();
            user.Token = Token;
            user.Username = Username;
            user.Initials = Initials;
            user.Group = Group;
            user.Slogan = Slogan;
            user.ColorHex = ColorHex;
            user.FullName = FullName;
            user.DisplayName = DisplayName;
            user.Avatar = Avatar;

            return user;
        }

        /// <summary>
        /// Indicate that this user is writing a message
        /// </summary>
        public async void ShowUserWriting()
        {
            lastWritingTime = DateTime.Now;

            IsWritingMessage = true;

            await TaskEx.Run(() =>
            {
                do
                {
                    Thread.Sleep(1000);

                    //If writing message has not been received after 3 seconds then hide animation
                    if ((DateTime.Now - lastWritingTime).TotalSeconds > 3)
                        IsWritingMessage = false;

                } while (IsWritingMessage);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Indicate that this user stopped writing a message
        /// </summary>
        public void HideUserWriting()
        {
            IsWritingMessage = false;
        }

        /// <summary>
        /// Indicate that user is sending file.
        /// </summary>
        public void ShowUserSendingFile()
        {
            IsSendingFile = true;
        }

        /// <summary>
        /// Indicate that user is not sending file
        /// </summary>
        public void HideUserSendingFile()
        {
            IsSendingFile = false;
        }

        /// <summary>
        /// Sets display name selecting the first field that is not null: display name, full name and user name.
        /// </summary>
        public void BuildDisplayName()
        {
            if (DisplayName == null && FullName != null)
            {
                DisplayName = FullName;
                OnPropertyChanged(nameof(DisplayName));
            }

            if (DisplayName == null && !string.IsNullOrEmpty(Username))
                ReadDisplayName(Encoding.UTF8.GetBytes(Username));
        }
    }
}
