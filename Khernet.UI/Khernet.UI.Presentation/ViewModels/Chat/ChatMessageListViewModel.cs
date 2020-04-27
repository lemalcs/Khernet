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
        /// Indicates if ther is a pendding message
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
        public Action<ChatMessageItemViewModel,int> ScrollToChatMessage { get; set; }

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
            get { return HasMessage || (UserContext!=null&& UserContext.ResendMessage != null); }
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
            UserContext.SetUnreadMessage(0);

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
            return HasMessage || (UserContext!=null && UserContext.ResendMessage != null);
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

                switch (FileHelper.GetContentType(f))
                {
                    case MessageType.Image:

                        var imageMessage = new ImageChatMessageViewModel(this,applicationDialog)
                        {
                            User = UserContext.User,
                            IsSentByMe = true,
                            SendDate = DateTimeOffset.Now,
                        };

                        UserContext.AddSentMessage(imageMessage);

                        //Process image from file system
                        imageMessage.ProcessImage(f);

                        Items.Add(imageMessage);

                        break;
                    case MessageType.GIF:

                        var animationMessage = new AnimationChatMessageViewModel(this, applicationDialog)
                        {
                            User = UserContext.User,
                            IsSentByMe = true,
                            SendDate = DateTimeOffset.Now,
                        };

                        UserContext.AddSentMessage(animationMessage);

                        animationMessage.ProcessAnimation(f);

                        Items.Add(animationMessage);

                        break;
                    case MessageType.Video:
                        
                        var videoMessage = new VideoChatMessageViewModel(this, applicationDialog)
                        {
                            User = UserContext.User,
                            IsSentByMe = true,
                            SendDate = DateTimeOffset.Now,
                        };

                        UserContext.AddSentMessage(videoMessage);

                        videoMessage.ProcessVideo(f);

                        Items.Add(videoMessage);
                        break;
                    case MessageType.Audio:

                        var audioMessage = new AudioChatMessageViewModel(this,applicationDialog)
                        {
                            User = UserContext.User,
                            IsSentByMe = true,
                            SendDate = DateTimeOffset.Now,
                        };

                        UserContext.AddSentMessage(audioMessage);

                        audioMessage.ProcessAudio(f);

                        Items.Add(audioMessage);
                        break;
                    default:
                        
                        var fileMessage = new FileChatMessageViewModel(this,applicationDialog)
                        {
                            User = UserContext.User,
                            IsSentByMe = true,
                            SendDate = DateTimeOffset.Now,
                        };

                        UserContext.AddSentMessage(fileMessage);

                        fileMessage.ProcessFile(f);

                        Items.Add(fileMessage);
                        break;
                }
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

            var imageMessage = new ImageChatMessageViewModel(this,applicationDialog);
            imageMessage.User = UserContext.User;
            imageMessage.IsSentByMe = true;
            imageMessage.SendDate = DateTimeOffset.Now;

            UserContext.AddSentMessage(imageMessage);

            imageMessage.ProcessImage(media);

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
        /// <param name="parameter">A <see cref="IDocumentContainer"/> object</param>
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

                if (UserContext.UnreadMessagesNumber > Items.Count)
                {
                    Items.Clear();
                    UserContext.FirstIdMessage = 0;
                    UserContext.LastIdMessage = 0;
                    LoadMessages(false);
                    MarkAsReadMessages();
                }

                //Check if there is a message to sent
                if (message != null)
                {
                    ChatMessageItemViewModel newMessage = null;

                    if (MessageFormat == MessageType.Html)
                    {
                        newMessage = new HtmlChatMessageViewModel(this)
                        {
                            User = UserContext.User,
                            IsSentByMe = true,
                            SendDate = DateTimeOffset.Now,
                            ReplyMessage = reply,
                            IsRead = true,
                        };

                        UserContext.AddSentMessage(newMessage);

                        ((HtmlChatMessageViewModel)newMessage).ProcessMessage(message);
                        Items.Add(newMessage);
                    }
                    else if (MessageFormat == MessageType.Markdown)
                    {
                        newMessage = new MarkdownChatMessageViewModel(this)
                        {
                            User = UserContext.User,
                            IsSentByMe = true,
                            SendDate = DateTimeOffset.Now,
                            ReplyMessage = reply,
                            IsRead = true,
                        };

                        UserContext.AddSentMessage(newMessage);

                        ((MarkdownChatMessageViewModel)newMessage).ProcessMessage(message);
                        Items.Add(newMessage);
                    }

                    SetCurrentChatModel(newMessage);
                }

                //Resend the message
                if (UserContext.ResendMessage != null)
                {
                    var chat = UserContext.ResendMessage;
                    chat.User = UserContext.User;
                    chat.SendDate = DateTimeOffset.Now;

                    UserContext.AddSentMessage(chat);

                    chat.ProcessResend();

                    Items.Add(chat);

                    UserContext.ResendMessage = null;

                    SetCurrentChatModel(chat);
                }

                //Clear reply message
                UserContext.ReplyMessage = null;

                OnPropertyChanged(nameof(ReplyMessagePending));
                OnPropertyChanged(nameof(CanSendMessage));

                CanShowUnreadPopup = false;
                UserContext.SetUnreadMessage(0);
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
                AnimationChatMessageViewModel animationChat = new AnimationChatMessageViewModel(this,applicationDialog);

                animationChat.ResendId = animation.Id;
                animationChat.FileName = animation.FileName;
                animationChat.FilePath = animation.FilePath;
                animationChat.Width = animation.Width;
                animationChat.Height = animation.Height;
                animationChat.SetThumbnail(animation.Thumbnail.ToArray());
                animationChat.SendDate = DateTimeOffset.Now;
                animationChat.User = UserContext.User;
                animationChat.IsSentByMe = true;

                animationChat.ProcessResend();

                UserContext.AddSentMessage(animationChat);

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
        /// Send an notification about writing a message to receipt peer
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

                int lastIdMessage = 0;

                bool middleLocation = idMessage > UserContext.FirstIdMessage && idMessage < UserContext.LastIdMessage;

                if (middleLocation)
                {
                    lastIdMessage = idMessage - 1;
                }
                else if (loadForward)
                {
                    lastIdMessage = UserContext.LastIdMessage;
                }
                else
                    lastIdMessage = UserContext.FirstIdMessage;

                int messageQuantity = 20;

                if (idMessage > 0)
                {
                    messageQuantity = 1;
                    if (UserContext.SentMessages != null && UserContext.SentMessages.Count > 0)
                        messageQuantity += UserContext.SentMessages.Count;
                }

                //Load messages ten by ten at time
                List<MessageItem> unreadMessages = IoCContainer.Get<Messenger>().GetLastMessages(UserContext.User.Token, loadForward, lastIdMessage, messageQuantity);

                if (unreadMessages == null)
                    return;

                //Get total number of unread messages
                List<MessageItem> newMessages = IoCContainer.Get<Messenger>().GetUnreadMessages(UserContext.User.Token);

                //If chat of current is opened increase the number of unread messages on chat list indicator
                if (newMessages != null)
                {
                    UserContext.SetUnreadMessage(newMessages.Count);
                    CanShowUnreadPopup = UserContext.UnreadMessagesNumber > 0;
                }

                UserContext.User.HideUserWriting();

                if (!middleLocation && loadForward)
                {
                    if (UserContext.FirstIdMessage == 0 || UserContext.FirstIdMessage > unreadMessages[0].Id)
                        UserContext.FirstIdMessage = unreadMessages[0].Id;

                    if (UserContext.LastIdMessage < unreadMessages[unreadMessages.Count - 1].Id)
                        UserContext.LastIdMessage = unreadMessages[unreadMessages.Count - 1].Id;
                }
                else if (!middleLocation)
                {
                    if (UserContext.FirstIdMessage == 0 || UserContext.FirstIdMessage > unreadMessages[unreadMessages.Count - 1].Id)
                        UserContext.FirstIdMessage = unreadMessages[unreadMessages.Count - 1].Id;

                    if (UserContext.LastIdMessage < unreadMessages[0].Id)
                        UserContext.LastIdMessage = unreadMessages[0].Id;
                }

                bool savedUnreadMessageLocation = false;

                ChatMessageItemViewModel firstUnreadMessageModel = null;

                int lastIndexChat = Items.IndexOf(UserContext.CurrentChatModel);

                bool scrollToFirstUnreadmessage = lastIndexChat == Items.Count - 1;

                bool isFirstLoad = Items.Count == 0;

                for (int i = 0; i < unreadMessages.Count; i++)
                {
                    if (UserContext.SentMessages != null)
                    {
                        var res = UserContext.SentMessages.FirstOrDefault((c) => c.UID == unreadMessages[i].UID);
                        if (res != null)
                        {
                            UserContext.SentMessages.Remove(res);
                            continue;
                        }
                    }

                    AddMessageToList(unreadMessages[i], !loadForward);

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
                    ScrollToChatMessage(firstUnreadMessageModel, userContext.FirstUnreadMessageIndex);
                else if (isFirstLoad)
                {
                    SetCurrentChatModel(Items[Items.Count - 1]);
                    ScrollToChatMessage(Items[Items.Count - 1], Items.Count - 1);
                }

                //Scroll to current sent message
                if (Items.Count > 1 && Items[Items.Count - 2] == UserContext.CurrentChatModel)
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
            ChatMessage chat = IoCContainer.Get<Messenger>().GetMessageDetail(messageItem.Id);

            UserItemViewModel user = chat.SenderToken == IoCContainer.Get<IIdentity>().Token ? null : UserContext.User;
            bool isSentByMe = user == null;

            MessageType type = (MessageType)(int)chat.Type;

            switch (type)
            {
                case MessageType.Text:
                    TextChatMessageViewModel message = new TextChatMessageViewModel(this)
                    {
                        Id = messageItem.Id,
                        User = UserContext.User,
                        IsSentByMe = isSentByMe,
                        SendDate = chat.SendDate,
                        IsRead = messageItem.IsRead,
                    };

                    message.ProcessMessage(messageItem.Id);

                    if (addAtStart)
                        Items.Insert(0, message);
                    else
                        Items.Add(message);

                    break;

                case MessageType.Html:
                    HtmlChatMessageViewModel htmlMessage = new HtmlChatMessageViewModel(this)
                    {
                        Id = messageItem.Id,
                        User = UserContext.User,
                        IsSentByMe = isSentByMe,
                        SendDate = chat.SendDate,
                        IsRead = messageItem.IsRead,
                    };

                    htmlMessage.ProcessMessage(messageItem.Id);

                    if (addAtStart)
                        Items.Insert(0, htmlMessage);
                    else
                        Items.Add(htmlMessage);

                    break;

                case MessageType.Markdown:
                    MarkdownChatMessageViewModel markdownMessage = new MarkdownChatMessageViewModel(this)
                    {
                        Id = messageItem.Id,
                        User = UserContext.User,
                        IsSentByMe = isSentByMe,
                        SendDate = chat.SendDate,
                        IsRead = messageItem.IsRead,
                    };

                    markdownMessage.ProcessMessage(messageItem.Id);

                    if (addAtStart)
                        Items.Insert(0, markdownMessage);
                    else
                        Items.Add(markdownMessage);

                    break;

                case MessageType.Image:

                    var imageMessage = new ImageChatMessageViewModel(this,applicationDialog)
                    {
                        Id = messageItem.Id,
                        User = UserContext.User,
                        IsSentByMe = isSentByMe,
                        SendDate = chat.SendDate,
                        IsRead = messageItem.IsRead,
                    };

                    //Process image from file system
                    imageMessage.ProcessImage(messageItem.Id);

                    if (addAtStart)
                        Items.Insert(0, imageMessage);
                    else
                        Items.Add(imageMessage);

                    break;
                case MessageType.GIF:
                    
                    var animationMessage = new AnimationChatMessageViewModel(this, applicationDialog)
                    {
                        Id = messageItem.Id,
                        User = UserContext.User,
                        IsSentByMe = isSentByMe,
                        SendDate = chat.SendDate,
                        IsRead = messageItem.IsRead,
                    };
                    animationMessage.ProcessAnimation(messageItem.Id);

                    if (addAtStart)
                        Items.Insert(0, animationMessage);
                    else
                        Items.Add(animationMessage);

                    break;
                case MessageType.Video:

                    var videoMessage = new VideoChatMessageViewModel(this, applicationDialog)
                    {
                        Id = messageItem.Id,
                        User = UserContext.User,
                        IsSentByMe = isSentByMe,
                        SendDate = chat.SendDate,
                        IsRead = messageItem.IsRead,
                    };
                    videoMessage.ProcessVideo(messageItem.Id);

                    if (addAtStart)
                        Items.Insert(0, videoMessage);
                    else
                        Items.Add(videoMessage);

                    break;
                case MessageType.Audio:

                    var audioMessage = new AudioChatMessageViewModel(this,applicationDialog)
                    {
                        Id = messageItem.Id,
                        User = UserContext.User,
                        IsSentByMe = isSentByMe,
                        SendDate = chat.SendDate,
                        IsRead = messageItem.IsRead,
                    };

                    audioMessage.ProcessAudio(messageItem.Id);

                    if (addAtStart)
                        Items.Insert(0, audioMessage);
                    else
                        Items.Add(audioMessage);

                    break;
                default:
                    
                    var fileMessage = new FileChatMessageViewModel(this,applicationDialog)
                    {
                        Id = messageItem.Id,
                        User = UserContext.User,
                        IsSentByMe = isSentByMe,
                        SendDate = chat.SendDate,
                        IsRead = messageItem.IsRead,
                    };

                    fileMessage.ProcessFile(messageItem.Id);

                    if (addAtStart)
                        Items.Insert(0, fileMessage);
                    else
                        Items.Add(fileMessage);

                    break;
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
                UserContext.DecreaseUnreadMessage();
                CanShowUnreadPopup = UserContext.UnreadMessagesNumber > 0;

                UserContext.User.DecreaseUnReadMessages();
            }

            if (Items.Count > 0 && Items[Items.Count - 1] == UserContext.CurrentChatModel)
            {
                MarkAsReadMessages();

                UserContext.SetUnreadMessage(0);
                CanShowUnreadPopup = UserContext.UnreadMessagesNumber > 0;
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

            CanShowUnreadPopup = UserContext.UnreadMessagesNumber > 0;

            FocusTextBox();

            //Scroll to last viewed message
            if (UserContext.UnreadMessagesNumber == 0)
            {
                int startIndex = Items.IndexOf(UserContext.CurrentChatModel);
                ScrollToChatMessage?.Invoke(UserContext.CurrentChatModel, startIndex);
            }
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
