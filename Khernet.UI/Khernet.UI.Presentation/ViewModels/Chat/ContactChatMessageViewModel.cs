using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model to share contact information to other peers.
    /// </summary>
    public class ContactChatMessageViewModel : TextMessageItemViewModel, ITextObserver
    {
        #region Properties

        private readonly IMessageManager messageManager;

        /// <summary>
        /// The information about contact (peer).
        /// </summary>
        private UserItemViewModel contact;

        /// <summary>
        /// Indicates whether a contact is exists in the current contact list.
        /// </summary>
        private bool exists;

        public TextRequest Text { get; set; }

        public UserItemViewModel Contact
        {
            get => contact;
            set
            {
                if (contact != value)
                {
                    contact = value;
                    OnPropertyChanged(nameof(Contact));
                }
            }
        }

        public bool Exists
        {
            get => exists;
            set
            {
                if (exists != value)
                {
                    exists = value;
                    OnPropertyChanged(nameof(Exists));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// View the profile of contact.
        /// </summary>
        public ICommand ViewProfileCommand { get; private set; }

        /// <summary>
        /// Adds the contact to list.
        /// </summary>
        public ICommand AddContactCommand { get; private set; }

        /// <summary>
        /// Open a chat message page to start send messages to the selected contact.
        /// </summary>
        public ICommand SendMessageCommand { get; private set; }

        #endregion

        public ContactChatMessageViewModel(IMessageManager messageManager)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");

            ReplyCommand = new RelayCommand(Reply, IsReadyMessage);
            ResendCommand = new RelayCommand(Resend, IsReadyMessage);
            ViewProfileCommand = new RelayCommand(ViewProfile);
            AddContactCommand = new RelayCommand(AddContact);
            SendMessageCommand = new RelayCommand(SendMessage);

            State = ChatMessageState.Pending;

            UID = Guid.NewGuid().ToString().Replace("-", "");
            TimeId = DateTimeOffset.Now.Ticks;
        }

        private async void SendMessage()
        {
            if (IoCContainer.Get<UserListViewModel>().FindUser(Contact.Token) != null)
                IoCContainer.Get<UserListViewModel>().SelectUser(Contact.Token);
            else
            {
                MessageBoxViewModel messageModel = new MessageBoxViewModel
                {
                    Message = $"User is being loaded, please try again later.",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                };
                await IoCContainer.UI.ShowMessageBox(messageModel, true);
            }
        }

        private void AddContact()
        {
            if (!Exists)
            {
                IoCContainer.Get<Messenger>().AddContact(new PeerService
                {
                    Token = Contact.Token,
                    Username = Contact.Username,
                    Certificate = Contact.Certificate,
                    ServiceList = Contact.ServiceList.ToList(),
                });
                Exists = true;
            }
        }

        private void ViewProfile()
        {
            if (Contact.IsSelfUser)
            {
                // Load gateway address
                Contact.LoadGatewayInformation();
            }

            ProfileViewModel profile = new ProfileViewModel(messageManager)
            {
                User = Contact,
            };
            profile.ShowProfile();
        }

        /// <summary>
        /// Prepares this message to be resend to other user.
        /// </summary>
        private void Resend()
        {
            messageManager.ResendMessage(this);
        }

        /// <summary>
        /// Replies a received or sent message.
        /// </summary>
        private void Reply()
        {
            messageManager.SendReplyMessage(this);
        }

        /// <summary>
        /// Gets a summary about this message.
        /// </summary>
        /// <param name="operation">The operation to do this summary.</param>
        /// <returns>A <see cref="ReplyMessageViewModel"/> object containing summary.</returns>
        public override ReplyMessageViewModel GetMessageSummary(MessageDirection operation)
        {
            ReplyMessageViewModel reply = new ReplyMessageViewModel();

            if (IsSentByMe)
            {
                var peer = IoCContainer.Get<Messenger>().GetProfile();

                reply.User = new UserItemViewModel
                {
                    Username = peer.UserName,
                    Token = peer.AccountToken,
                };
                reply.User.SetDisplayName(peer.FullName);
                reply.User.BuildDisplayName();
            }
            else
                reply.User = DisplayUser;

            reply.IsSentByMe = IsSentByMe;
            reply.State = State;
            reply.IsReplying = true;
            reply.FileName = Contact.Username;
            reply.Operation = operation;
            reply.IconName = "AccountCircle";
            reply.Id = Id;

            return reply;
        }

        public override void Load(MessageItem messageItem)
        {
            base.Load(messageItem);

            Text = new TextRequest
            {
                OperationRequest = MessageOperation.Download,
                ChatMessage = this,
            };

            IoCContainer.Text.ProcessText(this);
        }

        public override void Send(byte[] message)
        {
            Text = new TextRequest
            {
                Content = message,
                OperationRequest = MessageOperation.Upload,
                FileType = MessageType.Contact,
                ChatMessage = this,
            };

            if (ReplyMessage != null)
            {
                Text.IdReplyMessage = ReplyMessage.Id;
            }

            UserItemViewModel userItemView = IoCContainer.Get<UserListViewModel>().FindUser(Encoding.UTF8.GetString(message));
            if (userItemView != null)
            {
                Contact = userItemView.Clone();
                Exists = true;
            }

            IoCContainer.Text.ProcessText(this);
        }

        public override void ProcessResend()
        {
            Text = new TextRequest
            {
                OperationRequest = MessageOperation.Upload,
                Content = Encoding.UTF8.GetBytes(Contact.Token),
                FileType = MessageType.Contact,
                ChatMessage = this,
            };

            IoCContainer.Text.ProcessText(this);
        }

        /// <summary>
        /// Get a copy of this chat message.
        /// </summary>
        /// <returns>A <see cref="ChatMessageItemViewModel"/> for chat message.</returns>
        public override ChatMessageItemViewModel GetInstanceCopy()
        {
            ContactChatMessageViewModel chatMessage = new ContactChatMessageViewModel(messageManager);
            chatMessage.IsSentByMe = true;
            chatMessage.Contact = Contact;
            chatMessage.Exists = Exists;
            return chatMessage;
        }

        #region ITextObserver members

        public void OnError(Exception exception)
        {
            State = ChatMessageState.Error;
        }

        public void OnGetMetadata(TextResponse info)
        {
            if (info.Operation == MessageOperation.Download)
            {
                PeerService peerService = JSONSerializer<PeerService>.DeSerialize(info.Content);

                if (IoCContainer.Get<IIdentity>().Token == peerService.Token)
                {
                    Peer peer = IoCContainer.Get<Messenger>().GetProfile();

                    UserItemViewModel userItemView = new UserItemViewModel
                    {
                        Username = peer.UserName,
                        Token = peer.AccountToken,
                        Group = peer.Group,
                        Slogan = peer.Slogan,
                        State = peer.State.ToString(),
                        IsSelfUser = true,
                    };
                    userItemView.SetFullName(peer.FullName);
                    userItemView.SetAvatarThumbnail(IoCContainer.Get<Messenger>().GetAvatar());
                    userItemView.BuildDisplayName();
                    Contact = userItemView;
                }
                else
                {

                    Peer peer = IoCContainer.Get<Messenger>().GetPeerProfile(peerService.Token);

                    Contact = new UserItemViewModel(peerService.Certificate)
                    {
                        Username = peerService.Username,
                        Token = peerService.Token,
                        Initials = peerService.Username.Substring(0, 2).ToUpper(),
                    };
                    Contact.BuildDisplayName();

                    if (peer != null)
                    {
                        var userList = IoCContainer.Get<UserListViewModel>();
                        UserItemViewModel user = userList.FindUser(peerService.Token);
                        if (user != null)
                            Contact = user.Clone();
                        Exists = true;
                    }

                    if (!Exists)
                    {
                        Contact.ServiceList = new ObservableCollection<ServiceInfo>();
                        foreach (ServiceInfo service in peerService.ServiceList)
                        {
                            Contact.ServiceList.Add(service);
                        }
                    }
                }

                UID = info.UID;
                TimeId = info.TimeId;
            }

            if (info.Operation == MessageOperation.Download && info.IdReplyMessage != 0)
            {
                var detail = IoCContainer.Get<Messenger>().GetMessageDetail(info.IdReplyMessage);
                ReplyMessageViewModel reply = new ReplyMessageViewModel();
                reply.BuildReplyMessage(detail, info.IdReplyMessage);

                ReplyMessage = reply;
            }
        }

        public void OnCompleted(ChatMessageProcessResult result)
        {
            Id = result.Id;
            IsMessageLoaded = true;
            SetChatState(result.State);
        }

        #endregion
    }
}