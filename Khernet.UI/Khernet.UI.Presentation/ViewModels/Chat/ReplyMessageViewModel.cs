using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Khernet.UI
{
    public enum MessageDirection
    {
        /// <summary>
        /// Send a reply message.
        /// </summary>
        Reply = 0,

        /// <summary>
        /// Send a message to an different or same user.
        /// </summary>
        Resend = 1
    }

    /// <summary>
    /// View model for replied messages.
    /// </summary>
    public class ReplyMessageViewModel : BaseModel
    {

        #region Properties
        /// <summary>
        /// The user whom sent the message.
        /// </summary>
        private UserItemViewModel user;

        /// <summary>
        /// The name of file.
        /// </summary>
        private string fileName;

        /// <summary>
        /// The name of icon that indicates type of message.
        /// </summary>
        private string iconName;

        /// <summary>
        /// Indicates if messages is being replying (true) or begin resending.
        /// </summary>
        private bool isReplying;

        /// <summary>
        /// The operations to be done over message.
        /// </summary>
        private MessageDirection operation;

        private ChatMessageItemViewModel chatMessage;

        private bool isSentByMe;

        private ChatMessageState state;

        public UserItemViewModel User
        {
            get
            {
                return user;
            }

            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        /// <summary>
        /// The thumbnail used by the message.
        /// </summary>
        public ReadOnlyCollection<byte> Thumbnail
        {
            get; private set;
        }

        public string FileName
        {
            get => fileName;
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        public string IconName
        {
            get => iconName;
            set
            {
                if (iconName != value)
                {
                    iconName = value;
                    OnPropertyChanged(nameof(IconName));
                }
            }
        }

        /// <summary>
        /// The text message.
        /// </summary>
        public ReadOnlyCollection<byte> TextContent
        {
            get;
            private set;
        }

        public bool IsReplying
        {
            get => isReplying;
            set
            {
                if (isReplying != value)
                {
                    isReplying = value;
                    OnPropertyChanged(nameof(IsReplying));
                }
            }
        }

        public MessageDirection Operation
        {
            get => operation;
            set
            {
                if (operation != value)
                {
                    operation = value;
                    OnPropertyChanged(nameof(Operation));
                }
            }
        }

        /// <summary>
        /// The internal id of message.
        /// </summary>
        public int Id
        {
            get;
            set;
        }
        public ChatMessageItemViewModel ChatMessage
        {
            get => chatMessage;
            set
            {
                if (chatMessage != value)
                {
                    chatMessage = value;
                    OnPropertyChanged(nameof(ChatMessage));
                }
            }
        }
        public bool IsSentByMe
        {
            get => isSentByMe;
            set
            {
                if (isSentByMe != value)
                {
                    isSentByMe = value;
                    OnPropertyChanged(nameof(IsSentByMe));
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

        #endregion

        public ICommand OpenMessageCommand { get; private set; }

        public ReplyMessageViewModel()
        {
            OpenMessageCommand = new RelayCommand(OpenMessage);
            IconName = "File";
        }

        private void OpenMessage()
        {

        }

        public void SetThumbnail(byte[] thumbnailBytes)
        {
            Thumbnail = new ReadOnlyCollection<byte>(thumbnailBytes);
            OnPropertyChanged(nameof(Thumbnail));
        }

        public void SetTextContent(byte[] textContentBytes)
        {
            TextContent = new ReadOnlyCollection<byte>(textContentBytes);
            OnPropertyChanged(nameof(TextContent));
        }

        /// <summary>
        /// Set details of this reply message.
        /// </summary>
        /// <param name="messageDetail">The message being replied.</param>
        /// <param name="idReplyMessage">The id of reply message.</param>
        public void BuildReplyMessage(ChatMessage messageDetail, int idReplyMessage)
        {
            if (messageDetail == null)
                throw new ArgumentNullException($"Parameter {nameof(messageDetail)} cannot be null");

            IsReplying = false;

            if (messageDetail.SenderToken == IoCContainer.Get<IIdentity>().Token)
            {
                var peer = IoCContainer.Get<Messenger>().GetProfile();

                User = new UserItemViewModel
                {
                    Username = peer.UserName,
                    Token = peer.AccountToken,
                };
                User.SetDisplayName(peer.FullName);
                User.BuildDisplayName();
                IsSentByMe = true;
            }
            else
            {
                User = IoCContainer.Get<UserListViewModel>().FindUser(messageDetail.SenderToken);
                IsSentByMe = false;
            }

            var replyContent = IoCContainer.Get<Messenger>().GetMessageContent(idReplyMessage);

            Operation = MessageDirection.Reply;

            State = (ChatMessageState)((int)messageDetail.State);

            switch (messageDetail.Type)
            {
                case ContentType.Text:
                    SetTextContent(replyContent);
                    break;
                case ContentType.Html:
                    byte[] messageContent = IoCContainer.UI.ConvertHtmlToDocument(Encoding.UTF8.GetString(replyContent));
                    SetTextContent(messageContent);
                    break;

                case ContentType.Markdown:

                    messageContent = IoCContainer.UI.ConvertMarkdownToDocument(Encoding.UTF8.GetString(replyContent));
                    SetTextContent(messageContent);

                    break;

                case ContentType.Image:
                case ContentType.GIF:

                    byte[] thumbnail = IoCContainer.Get<Messenger>().GetThumbnail(idReplyMessage);

                    if (thumbnail != null)
                    {
                        SetThumbnail(thumbnail);
                    }

                    FileName = "Image";

                    break;

                case ContentType.Video:

                    thumbnail = IoCContainer.Get<Messenger>().GetThumbnail(idReplyMessage);

                    if (thumbnail != null)
                    {
                        SetThumbnail(thumbnail);
                    }

                    FileInformation videoInformation = JSONSerializer<FileInformation>.DeSerialize(replyContent);
                    FileName = videoInformation.FileName;

                    break;

                case ContentType.Audio:

                    IconName = "Play";
                    FileInformation audioInformation = JSONSerializer<FileInformation>.DeSerialize(replyContent);
                    FileName = audioInformation.FileName;
                    break;

                case ContentType.Binary:

                    IconName = "File";
                    FileInformation fileInformation = JSONSerializer<FileInformation>.DeSerialize(replyContent);
                    FileName = fileInformation.FileName;
                    break;

                case ContentType.Contact:

                    IconName = "AccountCircle";
                    PeerService peerService = JSONSerializer<PeerService>.DeSerialize(replyContent);
                    FileName = peerService.Username;
                    break;
            }


            Id = idReplyMessage;
        }

        /// <summary>
        /// Get a copy of this instance to be attached to response message.
        /// </summary>
        /// <returns>An instance of <see cref="ReplyMessageViewModel"/>.</returns>
        public ReplyMessageViewModel GetSendCopy()
        {
            ReplyMessageViewModel replyMessage = new ReplyMessageViewModel();
            replyMessage.FileName = FileName;
            replyMessage.IconName = IconName;
            replyMessage.User = User;
            replyMessage.IsReplying = false;
            replyMessage.IsSentByMe = IsSentByMe;
            replyMessage.Id = Id;
            replyMessage.State = State;
            replyMessage.Operation = Operation;

            if (Thumbnail != null)
                replyMessage.Thumbnail = new ReadOnlyCollection<byte>(Thumbnail.ToArray());

            if (TextContent != null)
                replyMessage.TextContent = new ReadOnlyCollection<byte>(TextContent.ToArray());

            return replyMessage;
        }
    }
}