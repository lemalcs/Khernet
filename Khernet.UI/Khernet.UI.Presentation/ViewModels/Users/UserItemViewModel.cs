using Khernet.Core.Host;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Khernet.UI
{
    public class UserItemViewModel : BaseModel, IIdentity, IMessageEventObserver
    {
        #region Properties
        /// <summary>
        /// The name initials for default picture profile.
        /// </summary>
        private string initials;

        /// <summary>
        /// The Hexadecimal value for color, for example: F5A2D8.
        /// </summary>
        private string colorHex;

        /// <summary>
        /// The state of user.
        /// </summary>
        private string state;

        /// <summary>
        /// The group of user.
        /// </summary>
        private string group;

        /// <summary>
        /// The slogan of user.
        /// </summary>
        private string slogan;

        /// <summary>
        /// Indicates if user is selected.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// The user-name.
        /// </summary>
        private string username;

        /// <summary>
        /// Gets the number of unread messages.
        /// </summary>
        private int unreadMessages;

        /// <summary>
        /// Indicates if user is writing a message.
        /// </summary>
        private bool isWritingMessage;

        /// <summary>
        /// Indicates if user is sending a file.
        /// </summary>
        private bool isSendingFile;

        /// <summary>
        /// The HTML format of full name.
        /// </summary>
        private string sourceFullName;

        /// <summary>
        /// The HTML format of display name.
        /// </summary>
        private string sourceDisplayName;

        private ReadOnlyCollection<byte> fullName;

        private ReadOnlyCollection<byte> displayName;

        private ReadOnlyCollection<byte> avatar;

        /// <summary>
        /// The list of services exposed by peer.
        /// </summary>
        private ObservableCollection<ServiceInfo> serviceList;

        /// <summary>
        /// The certificate of peer.
        /// </summary>
        private string certificate;

        /// <summary>
        /// Indicates if the peer is the current logged user.
        /// </summary>
        private bool isSelfUser;

        /// <summary>
        /// The host name of gateway address.
        /// </summary>
        private string hostName;

        /// <summary>
        /// The port of gateway address.
        /// </summary>
        private int port;

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
        /// The custom name to display for this user.
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
        public ObservableCollection<ServiceInfo> ServiceList
        {
            get => serviceList;
            set
            {
                if (serviceList != value)
                {
                    serviceList = value;
                    OnPropertyChanged(nameof(ServiceList));
                }
            }
        }
        public string Certificate
        {
            get => certificate;
        }

        public bool IsSelfUser
        {
            get => isSelfUser;
            set
            {
                if (isSelfUser != value)
                {
                    isSelfUser = value;
                    OnPropertyChanged(nameof(IsSelfUser));
                }
            }
        }

        public string HostName
        {
            get => hostName;
            set
            {
                if (hostName != value)
                {
                    hostName = value;
                    OnPropertyChanged(nameof(HostName));
                }
            }
        }

        public int Port
        {
            get => port;
            set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }

        #endregion

        #region IIdentity members

        /// <summary>
        /// The token of user.
        /// </summary>
        public string Token
        {
            get;
            set;
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

        #endregion

        /// <summary>
        /// Opens a chat for selected user.
        /// </summary>
        public ICommand OpenChatCommand { get; private set; }

        public UserItemViewModel()
        {
            OpenChatCommand = new RelayCommand(OpenChat);
        }

        public UserItemViewModel(string certificate)
        {
            OpenChatCommand = new RelayCommand(OpenChat);
            this.certificate = certificate;
        }

        #region Methods

        private void OpenChat()
        {
            IoCContainer.Get<ChatMessageListViewModel>().OpenChatListFor(this);
        }

        /// <summary>
        /// Increase in one the number of unread messages.
        /// </summary>
        public void IncreaseUnreadMessages()
        {
            UnreadMessages++;
            IoCContainer.Get<UserListViewModel>().AddUnreadMessages(1);
        }

        /// <summary>
        /// Decrease in one the number of unread messages.
        /// </summary>
        public void DecreaseUnreadMessages()
        {
            if (UnreadMessages > 0)
            {
                UnreadMessages--;
            }
            IoCContainer.Get<UserListViewModel>().SubtractUnreadMessages(1);
        }

        public void AddUnreadMessages(int messageQuantity)
        {
            if (messageQuantity < 0)
                throw new ArgumentException("Message quantity must be equals or greater than zero");

            UnreadMessages += messageQuantity;

            IoCContainer.Get<UserListViewModel>().AddUnreadMessages(messageQuantity);
        }

        public void ClearUnreadMessages()
        {
            IoCContainer.Get<UserListViewModel>().SubtractUnreadMessages(UnreadMessages);
            UnreadMessages = 0;
        }

        public void SetAvatarThumbnail(byte[] avatarBytes)
        {
            if (avatarBytes == null || avatarBytes.Length == 0)
                return;

            Avatar = new ReadOnlyCollection<byte>(avatarBytes);
            OnPropertyChanged(nameof(Avatar));
        }

        public void SetFullName(byte[] fullNameBytes)
        {
            if (fullNameBytes == null)
            {
                FullName = null;
                OnPropertyChanged(nameof(FullName));
                return;
            }

            SourceFullName = Encoding.UTF8.GetString(fullNameBytes);

            var name = IoCContainer.UI.ConvertHtmlToDocument(Encoding.UTF8.GetString(fullNameBytes));

            FullName = new ReadOnlyCollection<byte>(name);
            OnPropertyChanged(nameof(FullName));
        }

        public void SetDisplayName(byte[] displayNameBytes)
        {
            if (displayNameBytes == null)
            {
                DisplayName = null;
                SourceDisplayName = null;
                OnPropertyChanged(nameof(DisplayName));
                return;
            }

            SourceDisplayName = Encoding.UTF8.GetString(displayNameBytes);
            var name = IoCContainer.UI.ConvertHtmlToDocument(Encoding.UTF8.GetString(displayNameBytes));


            DisplayName = new ReadOnlyCollection<byte>(name);
            OnPropertyChanged(nameof(DisplayName));
        }

        /// <summary>
        /// Gets a copy of this object.
        /// </summary>
        /// <returns>A new <see cref="UserItemViewModel"/> object.</returns>
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

            if (ServiceList != null)
            {
                ObservableCollection<ServiceInfo> services = new ObservableCollection<ServiceInfo>();
                foreach (ServiceInfo serviceInfo in ServiceList)
                {
                    services.Add(serviceInfo);
                }
                user.serviceList = services;
            }

            return user;
        }

        /// <summary>
        /// Indicate that this user stopped writing a message.
        /// </summary>
        public void HideUserWriting()
        {
            IsWritingMessage = false;
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
                SetDisplayName(Encoding.UTF8.GetBytes(Username));
        }

        /// <summary>
        /// Gets the gateway address of peer.
        /// </summary>
        public void LoadGatewayInformation()
        {
            string gateway = IsSelfUser ? IoCContainer.Get<Messenger>().GetSelfGatewayAddress() : IoCContainer.Get<Messenger>().GetPeerGatewayAddress(Token);
            if (gateway != null)
            {
                Uri gatewayUri = new Uri(gateway);
                HostName = gatewayUri.Host;
                Port = gatewayUri.Port;
            }
        }

        #endregion

        #region IMessageEventObserver members

        public void OnUpdate(MessageEventData info)
        {
            if (info.EventType == MessageEvent.BeginWriting || info.EventType == MessageEvent.EndWriting)
                IsWritingMessage = info.EventType == MessageEvent.BeginWriting;

            if (info.EventType == MessageEvent.BeginSendingFile || info.EventType == MessageEvent.EndSendingFile)
                IsSendingFile = info.EventType == MessageEvent.BeginSendingFile;
        }

        #endregion
    }
}
