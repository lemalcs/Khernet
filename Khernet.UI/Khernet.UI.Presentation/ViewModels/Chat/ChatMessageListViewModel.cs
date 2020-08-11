using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Khernet.UI
{
    public class ChatMessageListViewModel : BaseModel, IMessageManager
    {
        #region Properties

        /// <summary>
        /// Indicates is media gallery (emojis and GIF) is open.
        /// </summary>
        private bool isMediaGalleryOpen;

        /// <summary>
        /// Indicates if GIF gallery is open.
        /// </summary>
        private bool isGIFGalleryOpen;

        /// <summary>
        /// The view model for media gallery
        /// </summary>
        private MediaGalleryViewModel mediaVM;

        /// <summary>
        /// Indicates if ther is a pending message
        /// </summary>
        private bool hasMessage;

        /// <summary>
        /// Indicates if other peer is writing a message
        /// </summary>
        private bool isPeerWriting;

        /// <summary>
        /// Indicates the last timea message was send to peer
        /// </summary>
        private DateTime lastMessageSendTime;

        /// <summary>
        /// Indicates whether text field has the focus, the specific value of this property does not matter, 
        /// only its changes
        /// </summary>
        private bool isTextBoxFocused;

        /// <summary>
        /// Indicates of unread popup should be shown
        /// </summary>
        private bool canShowUnreadPopup;

        /// <summary>
        /// Scrolls the chat list
        /// </summary>
        public Action ScrollToCurrentContent { get; set; }

        /// <summary>
        /// Set draft message
        /// </summary>
        public Action<byte[]> SetContent { get; set; }

        /// <summary>
        /// Get draft message
        /// </summary>
        public Func<byte[]> GetContent { get; set; }

        /// <summary>
        /// Get draft message
        /// </summary>
        public Action<ChatMessageItemViewModel, int> ScrollToChatMessage { get; set; }

        /// <summary>
        /// The state of user
        /// </summary>
        private UserChatContext userContext;

        /// <summary>
        /// The format of text message
        /// </summary>
        private MessageType messageFormat;

        /// <summary>
        /// Object used to sync access to chat list
        /// </summary>
        private object SyncObject = new object();

        /// <summary>
        /// The message of chat list
        /// </summary>
        private ObservableCollection<ChatMessageItemViewModel> items;

        /// <summary>
        /// Represents the objet to display dialogs
        /// </summary>
        private PresentationApplicationDialog applicationDialog;

        public MessageType MessageFormat
        {
            get => messageFormat;
            set
            {
                if (messageFormat != value)
                {
                    messageFormat = value;
                    OnPropertyChanged(nameof(MessageFormat));
                }
            }
        }

        public ObservableCollection<ChatMessageItemViewModel> Items
        {
            get => items;
            set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }

        public bool IsMediaGalleryOpen
        {
            get => isMediaGalleryOpen;
            set
            {
                if (isMediaGalleryOpen != value)
                {
                    isMediaGalleryOpen = value;
                    OnPropertyChanged(nameof(IsMediaGalleryOpen));
                }
            }
        }

        public bool IsGIFGalleryOpen
        {
            get => isGIFGalleryOpen;
            set
            {
                if (isGIFGalleryOpen != value)
                {
                    isGIFGalleryOpen = value;
                    OnPropertyChanged(nameof(IsGIFGalleryOpen));
                }
            }
        }

        public MediaGalleryViewModel MediaVM
        {
            get => mediaVM;
            set
            {
                if (mediaVM != value)
                {
                    mediaVM = value;
                    OnPropertyChanged(nameof(MediaVM));
                }
            }
        }

        /// <summary>
        /// Indicates if there is a reply message pending
        /// </summary>
        public bool ReplyMessagePending
        {
            get { return UserContext.ReplyMessage != null; }
        }

        public bool IsPeerWriting
        {
            get => isPeerWriting;
            set
            {
                if (isPeerWriting != value)
                {
                    isPeerWriting = value;
                    OnPropertyChanged(nameof(IsPeerWriting));
                }
            }
        }

        public bool HasMessage
        {
            get => hasMessage;
            set
            {
                if (hasMessage != value)
                {
                    hasMessage = value;
                    OnPropertyChanged(nameof(HasMessage));
                }
            }
        }

        public bool CanSendMessage
        {
            get { return HasMessage || (UserContext != null && UserContext.ResendMessage != null); }
        }

        public bool IsTextBoxFocused
        {
            get => isTextBoxFocused;
            set
            {
                if (isTextBoxFocused != value)
                {
                    isTextBoxFocused = value;
                    OnPropertyChanged(nameof(IsTextBoxFocused));
                }
            }
        }

        public bool CanShowUnreadPopup
        {
            get => canShowUnreadPopup;
            set
            {
                if (canShowUnreadPopup != value)
                {
                    canShowUnreadPopup = value;
                    OnPropertyChanged(nameof(CanShowUnreadPopup));
                }
            }
        }

        public UserChatContext UserContext
        {
            get => userContext;
            set
            {
                if (userContext != value)
                {
                    userContext = value;
                    OnPropertyChanged(nameof(UserContext));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to view user profile
        /// </summary>
        public ICommand ViewProfileCommand { get; private set; }

        /// <summary>
        /// Command to send message
        /// </summary>
        public ICommand SendCommand { get; private set; }

        /// <summary>
        /// Command to send a file
        /// </summary>
        public ICommand OpenFileCommand { get; private set; }

        /// <summary>
        /// Command to open emoji gallery
        /// </summary>
        public ICommand OpenMediaGalleryCommand { get; private set; }

        /// <summary>
        /// Command to open GIF gallery
        /// </summary>
        public ICommand OpenGIFGalleryCommand { get; private set; }

        /// <summary>
        /// Command to discard a reply message
        /// </summary>
        public ICommand CloseReplyMessage { get; private set; }

        /// <summary>
        /// Command to got to the bottom of chat list
        /// </summary>
        public ICommand GoToBottomCommand { get; private set; }

        #endregion

        public ChatMessageListViewModel()
        {
            SendCommand = new RelayCommand(Send, CanSend);
            ViewProfileCommand = new RelayCommand(ViewProfile);
            OpenFileCommand = new RelayCommand(OpenFile);
            OpenMediaGalleryCommand = new RelayCommand(OpenEmojiGallery);
            CloseReplyMessage = new RelayCommand(CloseReply);
            OpenGIFGalleryCommand = new RelayCommand(OpenGIFGallery);
            GoToBottomCommand = new RelayCommand(GoToBottom);

            MessageFormat = MessageType.Html;

            applicationDialog = new PresentationApplicationDialog();
        }

        private void GoToBottom(object parameter)
        {
            MarkAsReadMessages();

            CanShowUnreadPopup = false;
            UserContext.User.ClearUnreadMessages();

            IDocumentContainer container = parameter as IDocumentContainer;

            UserContext.CurrentChatModel = Items[Items.Count - 1];

            container.ScrollToCurrentContent();
        }

        /// <summary>
        /// Marks all unread message as read
        /// </summary>
        private void MarkAsReadMessages()
        {
            if (UserContext.FirstUnreadMessageIndex > 0 && UserContext.FirstUnreadMessageIndex < Items.Count)
            {
                for (int i = UserContext.FirstUnreadMessageIndex; i < Items.Count; i++)
                {
                    IoCContainer.Get<Messenger>().MarkAsReadMessage(Items[i].Id);
                }
                UserContext.FirstUnreadMessageIndex = 0;
            }
            if (Items.Count > 0)
                IoCContainer.Get<Messenger>().BulkMarkAsReadMessage(Items[Items.Count - 1].Id);
        }

        private void OpenGIFGallery(object obj)
        {
            if (MediaVM == null)
            {
                MediaVM = new MediaGalleryViewModel();
            }

            IsGIFGalleryOpen = true;
        }

        private void CloseReply(object obj)
        {
            UserContext.ReplyMessage = null;
            UserContext.ResendMessage = null;
            OnPropertyChanged(nameof(ReplyMessagePending));
            OnPropertyChanged(nameof(CanSendMessage));
        }

        private void OpenEmojiGallery(object obj)
        {
            IsMediaGalleryOpen = true;
        }

        private bool CanSend(object obj)
        {
            return HasMessage || (UserContext != null && UserContext.ResendMessage != null);
        }

        /// <summary>
        /// Opens a dialog to select files
        /// </summary>
        /// <param name="obj">A <see cref="IDocumentContainer"/> object</param>
        private void OpenFile(object parameter)
        {
            //Get file paths
            string[] files = IoCContainer.Get<IUIManager>().ShowOpenFileDialog();

            if (files == null)
                return;

            //Send all selected files
            Send(files);

            IDocumentContainer container = parameter as IDocumentContainer;

            AddCurrentChatModel(Items[Items.Count - 1]);
            container.ScrollToCurrentContent();
        }

        /// <summary>
        /// Send a one or more files
        /// </summary>
        /// <param name="files">The paths of files</param>
        public async void Send(string[] files)
        {
            //Do not do anything if there is not a file
            if (files == null || files.Length == 0)
                return;

            if (Items == null)
                Items = new ObservableCollection<ChatMessageItemViewModel>();

            //Send Files
            foreach (string f in files)
            {
                if (FileHelper.GetFileSize(f) > FileHelper.GIGABYTE)
                {
                    await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                    {
                        Message = "File size must be less o equal to 2 GB",
                        Title = "Khernet",
                        ShowAcceptOption = true,
                        AcceptOptionLabel = "OK",
                        ShowCancelOption = false,
                    });
                    continue;
                }

                FileMessageItemViewModel fileMessage = null;

                switch (FileHelper.GetContentType(f))
                {
                    case MessageType.Image:
                        fileMessage = new ImageChatMessageViewModel(this, applicationDialog);
                        break;

                    case MessageType.GIF:
                        fileMessage = new AnimationChatMessageViewModel(this, applicationDialog);
                        break;

                    case MessageType.Video:
                        fileMessage = new VideoChatMessageViewModel(this, applicationDialog);
                        break;

                    case MessageType.Audio:
                        fileMessage = new AudioChatMessageViewModel(this, applicationDialog);
                        break;

                    default:
                        fileMessage = new FileChatMessageViewModel(this, applicationDialog);
                        break;
                }

                fileMessage.DisplayUser = UserContext.User;
                fileMessage.IsSentByMe = true;
                fileMessage.SendDate = DateTimeOffset.Now;
                fileMessage.IsRead = true;
                fileMessage.SenderUserId = IoCContainer.Get<IIdentity>();
                fileMessage.ReceiverUserId = UserContext.User;

                fileMessage.Send(f);

                Items.Add(fileMessage);
            }
        }

        /// <summary>
        /// Send a image from a stream (used for images from cliboard)
        /// </summary>
        /// <param name="media"></param>
        public void Send(Stream media)
        {
            //Do not do anything if there is not a file
            if (media == null)
                return;

            if (Items == null)
                Items = new ObservableCollection<ChatMessageItemViewModel>();

            var imageMessage = new ImageChatMessageViewModel(this, applicationDialog);
            imageMessage.DisplayUser = UserContext.User;
            imageMessage.IsSentByMe = true;
            imageMessage.SendDate = DateTimeOffset.Now;
            imageMessage.IsRead = true;
            imageMessage.SenderUserId = IoCContainer.Get<IIdentity>();
            imageMessage.ReceiverUserId = UserContext.User;

            imageMessage.Send(media);

            Items.Add(imageMessage);
        }

        /// <summary>
        /// View current profile
        /// </summary>
        /// <param name="obj"></param>
        private void ViewProfile(object obj)
        {
            ProfileViewModel profile = new ProfileViewModel(this)
            {
                User = UserContext.User,
            };
            profile.ShowProfile();
        }

        /// <summary>
        /// Sends text message
        /// </summary>
        /// <param name="parameter">An <see cref="IDocumentContainer"/> object</param>
        public void Send(object parameter)
        {
            if (parameter == null)
                return;

            //Check if there is a text message
            IDocumentContainer container = parameter as IDocumentContainer;

            if (HasMessage)
            {
                //Get current message content
                byte[] messageContent = container.GetDocument(MessageFormat);

                //Send message
                Send(messageContent);
            }
            else
                Send((byte[])null);

            //Clear current message
            container.ClearContent();

            AddCurrentChatModel(Items[Items.Count - 1]);
            container.ScrollToCurrentContent();
        }

        /// <summary>
        /// Sends text message with XAML format
        /// </summary>
        /// <param name="parameter"></param>
        public async void Send(byte[] message)
        {
            try
            {
                //Do not do anything if there is not a message or if it just white spaces.
                if ((message == null || message.Length == 0) && UserContext.ResendMessage == null)
                    return;

                if (Items == null)
                    Items = new ObservableCollection<ChatMessageItemViewModel>();

                ReplyMessageViewModel reply = null;

                //Copy reply message view model for current message
                if (UserContext.ReplyMessage != null && UserContext.ResendMessage == null)
                {
                    reply = UserContext.ReplyMessage.GetSendCopy();
                }

                if (UserContext.User.UnreadMessages > Items.Count)
                {
                    Items.Clear();
                    LoadMessages(false);
                    MarkAsReadMessages();
                }

                //Resend the message
                if (UserContext.ResendMessage != null)
                {
                    var chat = UserContext.ResendMessage;
                    chat.DisplayUser = UserContext.User;
                    chat.SenderUserId = IoCContainer.Get<IIdentity>();
                    chat.ReceiverUserId = UserContext.User;
                    chat.SendDate = DateTimeOffset.Now;

                    chat.ProcessResend();

                    Items.Add(chat);

                    UserContext.ResendMessage = null;

                    SetCurrentChatModel(chat);
                }

                //Check if there is a message to sent
                if (message != null)
                {
                    TextMessageItemViewModel newMessage = null;

                    if (MessageFormat == MessageType.Html)
                    {
                        newMessage = new HtmlChatMessageViewModel(this);
                    }
                    else if (MessageFormat == MessageType.Markdown)
                    {
                        newMessage = new MarkdownChatMessageViewModel(this,applicationDialog);
                    }

                    newMessage.DisplayUser = UserContext.User;
                    newMessage.IsSentByMe = true;
                    newMessage.SendDate = DateTimeOffset.Now;
                    newMessage.ReplyMessage = reply;
                    newMessage.IsRead = true;
                    newMessage.SenderUserId = IoCContainer.Get<IIdentity>();
                    newMessage.ReceiverUserId = UserContext.User;

                    newMessage.Send(message);

                    Items.Add(newMessage);

                    SetCurrentChatModel(newMessage);
                }

                //Clear reply message
                UserContext.ReplyMessage = null;

                OnPropertyChanged(nameof(ReplyMessagePending));
                OnPropertyChanged(nameof(CanSendMessage));

                CanShowUnreadPopup = false;
                UserContext.User.ClearUnreadMessages();
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error while sending message",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }

        public async void SendAnimation(GIFItemViewModel animation, IDocumentContainer document)
        {
            if (animation == null)
                return;
            try
            {
                AnimationChatMessageViewModel animationChat = new AnimationChatMessageViewModel(this, applicationDialog);

                animationChat.ResendFileId = animation.Id;
                animationChat.FileName = animation.FileName;
                animationChat.FilePath = animation.FilePath;
                animationChat.Width = animation.Width;
                animationChat.Height = animation.Height;
                animationChat.SetThumbnail(animation.Thumbnail.ToArray());
                animationChat.SendDate = DateTimeOffset.Now;
                animationChat.DisplayUser = UserContext.User;
                animationChat.IsSentByMe = true;
                animationChat.IsRead = true;
                animationChat.SenderUserId = IoCContainer.Get<IIdentity>();
                animationChat.ReceiverUserId = UserContext.User;

                animationChat.ProcessResend();

                Items.Add(animationChat);

                IsTextBoxFocused = !IsTextBoxFocused;
                OnPropertyChanged(nameof(IsTextBoxFocused));

                AddCurrentChatModel(Items[Items.Count - 1]);
                document.ScrollToCurrentContent();
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error while sending GIF",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }

        /// <summary>
        /// Sets <see cref="HasMessage"/> that indicates if there is a message
        /// </summary>
        /// <param name="hasMessage">True if there is message otherwise false</param>
        public void SetHasMessage(bool hasMessage)
        {
            this.HasMessage = hasMessage;
            OnPropertyChanged(nameof(CanSendMessage));
        }

        /// <summary>
        /// Send an notification about writing a message to receiver peer.
        /// </summary>
        public void SendWritingMessage()
        {
            //Send writing message every 3 seconds
            if ((DateTime.Now - lastMessageSendTime).TotalSeconds > 3)
            {
                TaskEx.Run(() =>
                {
                    //Get token of user logged into application
                    string senderToken = IoCContainer.Get<IIdentity>().Token;
                    IoCContainer.Get<Messenger>().SendWrtitingMessage(senderToken, UserContext.User.Token);
                });

                lastMessageSendTime = DateTime.Now;
            }
        }

        private void LoadMessages(bool loadForward, int idMessage)
        {
            lock (SyncObject)
            {
                if (Items == null)
                    Items = new ObservableCollection<ChatMessageItemViewModel>();

                if (UserContext == null)
                    return;

                long firstTimeId = 0;
                long lastTimeId = 0;

                //Get first and last message ids
                if (Items.Count > 0)
                {
                    firstTimeId = Items[0].TimeId;
                    lastTimeId = Items[Items.Count - 1].TimeId;
                }

                long lastTimeIdMessage = 0;
                long newMessageTimeId = IoCContainer.Get<Messenger>().GetTimeIdMessage(idMessage);
                bool middleLocation = newMessageTimeId > firstTimeId && newMessageTimeId < lastTimeId;

                if (middleLocation)
                {
                    lastTimeIdMessage = newMessageTimeId - 1;
                }
                else if (loadForward)
                {
                    lastTimeIdMessage = lastTimeId;
                }
                else
                    lastTimeIdMessage = firstTimeId;

                List<MessageItem> unreadMessages = null;
                int unreadMessageNumber = 0;
                int messageQuantity = 30;

                if (newMessageTimeId > 0)
                {
                    messageQuantity = 1;
                }

                if (idMessage == 0)
                {
                    unreadMessages = IoCContainer.Get<Messenger>().GetLastMessages(UserContext.User.Token, loadForward, lastTimeIdMessage, messageQuantity);

                    if (unreadMessages == null)
                        return;

                    List<MessageItem> newMessages = IoCContainer.Get<Messenger>().GetUnreadMessages(UserContext.User.Token);
                    unreadMessageNumber = newMessages == null ? 0 : newMessages.Count;
                }
                else
                {
                    unreadMessages = new List<MessageItem>();
                    unreadMessages.Add(IoCContainer.Get<Messenger>().GetMessageHeader(idMessage));
                    unreadMessageNumber = unreadMessages[0].IsRead ? 0 : 1;
                }

                UserContext.User.HideUserWriting();

                bool savedUnreadMessageLocation = false;
                ChatMessageItemViewModel firstUnreadMessageModel = null;
                int lastIndexChat = Items.IndexOf(UserContext.CurrentChatModel);
                bool scrollToFirstUnreadmessage = lastIndexChat == Items.Count - 1;
                bool isFirstLoad = Items.Count == 0;

                for (int i = 0; i < unreadMessages.Count; i++)
                {
                    AddMessageToList(unreadMessages[i], !loadForward);

                    if (!isFirstLoad && !unreadMessages[i].IsRead)
                    {
                        CanShowUnreadPopup = UserContext.User.UnreadMessages > 0;
                    }

                    //Save the first id to perform a whole read update to unread messages
                    if (!unreadMessages[i].IsRead && (loadForward && !savedUnreadMessageLocation))
                    {
                        UserContext.FirstUnreadMessageIndex = Items.Count - 1;
                        firstUnreadMessageModel = Items[Items.Count - 1];
                        savedUnreadMessageLocation = true;
                    }
                }

                //Check if it has to scroll to first unread message
                if (scrollToFirstUnreadmessage && firstUnreadMessageModel != null)
                {
                    if (IoCContainer.UI.IsMainWindowActive())
                        ScrollToChatMessage(firstUnreadMessageModel, userContext.FirstUnreadMessageIndex);
                }
                else if (isFirstLoad)
                {
                    SetCurrentChatModel(Items[Items.Count - 1]);
                    ScrollToChatMessage(Items[Items.Count - 1], Items.Count - 1);
                }

                //Scroll to current sent message
                if (Items.Count > 1 && (Items[Items.Count - 2] == UserContext.CurrentChatModel && UserContext.CurrentChatModel.IsSentByMe))
                {
                    ScrollToCurrentContent?.Invoke();
                }
            }
        }


        public void LoadMessage(int idMessage)
        {
            LoadMessages(true, idMessage);
        }

        public void LoadMessages(bool loadForward)
        {
            LoadMessages(loadForward, 0);
        }

        private void AddMessageToList(MessageItem messageItem, bool addAtStart)
        {
            MessageType type = (MessageType)(int)messageItem.Format;

            ChatMessageItemViewModel chatMessage;

            switch (type)
            {
                case MessageType.Text:
                    chatMessage = new TextChatMessageViewModel(this);
                    break;

                case MessageType.Html:
                    chatMessage = new HtmlChatMessageViewModel(this);
                    break;

                case MessageType.Markdown:
                    chatMessage = new MarkdownChatMessageViewModel(this,applicationDialog);
                    break;

                case MessageType.Image:
                    chatMessage = new ImageChatMessageViewModel(this, applicationDialog);
                    break;

                case MessageType.GIF:
                    chatMessage = new AnimationChatMessageViewModel(this, applicationDialog);
                    break;

                case MessageType.Video:
                    chatMessage = new VideoChatMessageViewModel(this, applicationDialog);
                    break;

                case MessageType.Audio:
                    chatMessage = new AudioChatMessageViewModel(this, applicationDialog);
                    break;

                default:
                    chatMessage = new FileChatMessageViewModel(this, applicationDialog);
                    break;
            }

            chatMessage.DisplayUser = UserContext.User;
            chatMessage.SenderUserId = messageItem.IdSenderPeer == 0 ? IoCContainer.Get<IIdentity>() : UserContext.User;
            chatMessage.ReceiverUserId = messageItem.IdSenderPeer == 0 ? UserContext.User : IoCContainer.Get<IIdentity>();

            chatMessage.Load(messageItem);

            if (addAtStart)
                Items.Insert(0, chatMessage);
            else
            {
                int i = Items.Count - 1;
                for (; i > 0; i--)
                {
                    //Search the message added just before the new message
                    if (Items[i].TimeId < chatMessage.TimeId ||
                        Items[i].TimeId == chatMessage.TimeId && Items[i].Id != chatMessage.Id)
                        break;

                    //Check if there is a duplicate message
                    if (Items[i].TimeId == chatMessage.TimeId && Items[i].Id == chatMessage.Id)
                        return;
                }
                if (i == items.Count - 1)
                    items.Add(chatMessage);
                else
                    Items.Insert(i + 1, chatMessage);
            }
        }

        public void AddAnimationToGallery(int idAnimation)
        {
            if (MediaVM != null)
            {
                MediaVM.AddAnimation(idAnimation);
            }
        }

        public async void ResendMessage(ChatMessageItemViewModel messageModel, UserItemViewModel receiver = null)
        {
            if (receiver != null)
            {
                UserContext.ReplyMessage = messageModel.GetMessageSummary(MessageDirection.Resend);
                UserContext.ResendMessage = GetChatMessageCopy(messageModel);

                OnPropertyChanged(nameof(CanSendMessage));
                return;
            }

            var pagedVM = IoCContainer.Get<PagedDialogViewModel>();

            //Set list of settings as first page
            pagedVM.CurrentPage = ApplicationPage.Resend;

            //Title for page
            pagedVM.Category = "Select a user: ";

            ResendViewModel resend = new ResendViewModel
            {
                Items = IoCContainer.Get<UserListViewModel>().Clone(),
                SelectedUser = null,
                Message = messageModel,
            };

            //Set the view model for settings list
            pagedVM.CurrentViewModel = resend;

            pagedVM.SetHomePage(pagedVM.CurrentPage, pagedVM.Category, pagedVM.CurrentViewModel);

            await IoCContainer.UI.ShowDialog(pagedVM);

            if (resend.SelectedUser != null)
            {
                UserContext.ReplyMessage = messageModel.GetMessageSummary(MessageDirection.Resend);
                UserContext.ResendMessage = GetChatMessageCopy(messageModel);
                OnPropertyChanged(nameof(CanSendMessage));
            }
            else
                FocusTextBox();
        }

        /// <summary>
        /// Get a copy of a chat message that id being resend
        /// </summary>
        /// <param name="chatMessage">The original chat message to copy to</param>
        /// <returns>A new instance of <see cref="ChatMessageItemViewModel"/></returns>
        private ChatMessageItemViewModel GetChatMessageCopy(ChatMessageItemViewModel chatMessage)
        {
            if (chatMessage is ImageChatMessageViewModel imageMessage)
            {
                return imageMessage.GetInstanceCopy(this, applicationDialog);
            }
            else if (chatMessage is VideoChatMessageViewModel videoMessage)
            {
                return videoMessage.GetInstanceCopy(this, applicationDialog);
            }
            else if (chatMessage is FileChatMessageViewModel FileMessage)
            {
                return FileMessage.GetInstanceCopy(this, applicationDialog);
            }
            else if (chatMessage is AnimationChatMessageViewModel animationMessage)
            {
                return animationMessage.GetInstanceCopy(this, applicationDialog);
            }
            else if (chatMessage is AudioChatMessageViewModel audioMessage)
            {
                return audioMessage.GetInstanceCopy(this, applicationDialog);
            }
            else
                return ((TextMessageItemViewModel)chatMessage).GetInstanceCopy();
        }


        public void SendReplyMessage(ChatMessageItemViewModel messageModel)
        {
            if (messageModel == null)
                throw new ArgumentNullException($"Parameter {nameof(messageModel)} cannot be null");

            UserContext.ReplyMessage = messageModel.GetMessageSummary(MessageDirection.Reply);

            OnPropertyChanged(nameof(ReplyMessagePending));
        }

        public void CheckUnreadMessageAsRead(ChatMessageItemViewModel messageModel)
        {
            if (messageModel == null)
                return;

            if (messageModel != null && !messageModel.IsRead)
            {
                IoCContainer.Get<Messenger>().MarkAsReadMessage(messageModel.Id);
                messageModel.IsRead = true;
                UserContext.User.DecreaseUnreadMessages();
                CanShowUnreadPopup = UserContext.User.UnreadMessages > 0;
            }

            if (Items.Count > 0 && Items[Items.Count - 1] == UserContext.CurrentChatModel)
            {
                MarkAsReadMessages();

                UserContext.User.ClearUnreadMessages();
                CanShowUnreadPopup = UserContext.User.UnreadMessages > 0;
            }
        }

        public void SetCurrentChatModel(ChatMessageItemViewModel messageModel)
        {
            AddCurrentChatModel(messageModel);
        }

        public void SetFirstViewChatModel(ChatMessageItemViewModel messageModel)
        {
            if (UserContext.FirstViewChatModel != messageModel)
                UserContext.FirstViewChatModel = messageModel;
        }

        public ChatMessageItemViewModel GetCurrentChatModel()
        {
            return UserContext == null ? null : UserContext.CurrentChatModel;
        }

        private void AddCurrentChatModel(ChatMessageItemViewModel chatModel)
        {
            UserContext.CurrentChatModel = chatModel;
        }

        public void OpenChatListFor(UserItemViewModel user)
        {
            if (user == null)
                return;

            //Got to chat page to start to send messages
            IoCContainer.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Chat, IoCContainer.Get<ChatMessageListViewModel>());

            //By default indicate that there is not a draft message
            HasMessage = false;

            SaveDraftMessage();

            //Retrieve chat list from cache, add if it does not exist
            ObservableCollection<ChatMessageItemViewModel> chatList = IoCContainer.Chat.GetChat(user);
            if (chatList == null)
            {
                IoCContainer.Chat.AddChatList(user);
            }
            chatList = IoCContainer.Chat.GetChat(user);
            UserContext = IoCContainer.Chat.GetUserContext(user);

            //Set chat list to view model
            SetChatList(chatList);

            //Get draft message
            MessageFormat = UserContext.DraftMessageFormat;
            SetContent?.Invoke(UserContext.DraftMessage);
            OnPropertyChanged(nameof(CanSendMessage));

            //Set index to zero so unread message can be marked as a whole
            UserContext.FirstUnreadMessageIndex = 0;

            //Load messages
            if (UserContext.User.UnreadMessages > 0)
                LoadMessages(true);
            else if (Items != null && Items.Count == 0)
                LoadMessages(false);

            CanShowUnreadPopup = UserContext.User.UnreadMessages > 0;

            FocusTextBox();

            //Scroll to last viewed message
            int startIndex = Items.IndexOf(UserContext.CurrentChatModel);
            ScrollToChatMessage?.Invoke(UserContext.CurrentChatModel, startIndex);
        }

        /// <summary>
        /// Saves draft message in HTML format if any exists.
        /// </summary>
        public void SaveDraftMessage()
        {
            if (UserContext != null)
            {
                UserContext.DraftMessageFormat = MessageFormat;
                UserContext.DraftMessage = GetContent?.Invoke();
            }
        }

        public void FocusTextBox()
        {
            IsTextBoxFocused = !IsTextBoxFocused;
            OnPropertyChanged(nameof(IsTextBoxFocused));
        }

        protected void SetChatList(IEnumerable<ChatMessageItemViewModel> chatList)
        {
            Items = (ObservableCollection<ChatMessageItemViewModel>)chatList;
        }
    }
}
