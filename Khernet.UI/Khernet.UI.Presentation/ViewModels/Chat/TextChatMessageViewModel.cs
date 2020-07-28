using Khernet.Core.Host;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Khernet.UI
{
    /// <summary>
    /// View model for text messages (XAML inner format).
    /// </summary>
    public class TextChatMessageViewModel : TextMessageItemViewModel, ITextObserver
    {
        #region Properties

        private readonly IMessageManager messageManager;

        public ReadOnlyCollection<byte> TextContent
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates if there is a previous message replied
        /// </summary>
        public bool HasReplyMessage
        {
            get { return ReplyMessage != null; }
        }


        public TextRequest Text { get; set; }

        #endregion

        public TextChatMessageViewModel(IMessageManager messageManager)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");

            ReplyCommand = new RelayCommand(Reply, IsReadyMessage);
            ResendCommand = new RelayCommand(Resend, IsReadyMessage);

            State = ChatMessageState.Pending;

            UID = Guid.NewGuid().ToString().Replace("-", "");
            TimeId = DateTimeOffset.Now.Ticks;
        }

        /// <summary>
        /// Prepares this message to be resend to other user
        /// </summary>
        /// <param name="obj"></param>
        private void Resend()
        {
            messageManager.ResendMessage(this);
        }

        /// <summary>
        /// Replies a received or sent message
        /// </summary>
        /// <param name="obj"></param>
        private void Reply()
        {
            messageManager.SendReplyMessage(this);
        }

        /// <summary>
        /// Gets a summary about this message
        /// </summary>
        /// <param name="operation">The operation to do this this summary</param>
        /// <returns>A <see cref="ReplyMessageViewModel"/> object containing summary</returns>
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
                reply.User.ReadDisplayName(peer.FullName);
                reply.User.BuildDisplayName();
            }
            else
                reply.User = DisplayUser;

            reply.IsSentByMe = IsSentByMe;
            reply.State = State;
            reply.IsReplying = true;
            reply.SetTextContent(TextContent.ToArray());
            reply.Operation = operation;
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
                FileType = MessageType.Text,
                ChatMessage = this,
            };

            if (ReplyMessage != null)
            {
                Text.IdReplyMessage = ReplyMessage.Id;
            }

            IoCContainer.Text.ProcessText(this);
        }

        public override void ProcessResend()
        {
            Text = new TextRequest
            {
                OperationRequest = MessageOperation.Upload,
                Content = TextContent.ToArray(),
                FileType = MessageType.Text,
                ChatMessage = this,
            };


            IoCContainer.Text.ProcessText(this);
        }

        /// <summary>
        /// Get a copy of this chat message
        /// </summary>
        /// <returns></returns>
        public override ChatMessageItemViewModel GetInstanceCopy()
        {
            TextChatMessageViewModel chatMessage = new TextChatMessageViewModel(messageManager);
            chatMessage.IsSentByMe = true;

            byte[] message = new byte[TextContent.Count];
            TextContent.CopyTo(message, 0);

            chatMessage.SetTextContent(message);

            return chatMessage;
        }

        public void SetTextContent(byte[] textContentBytes)
        {
            TextContent = new ReadOnlyCollection<byte>(textContentBytes);
            OnPropertyChanged(nameof(TextContent));
        }

        #region ITextObserver members

        public void OnError(Exception exception)
        {
            State = ChatMessageState.Error;
        }

        public void OnGetMetadata(TextResponse info)
        {
            SetTextContent(info.Content);

            if (info.Operation == MessageOperation.Download)
            {
                UID = info.UID;
                TimeId = info.TimeId;
            }

            if (info.Operation == MessageOperation.Download && info.IdReplyMessage != 0)
            {
                var detail = IoCContainer.Get<Messenger>().GetMessageDetail(info.IdReplyMessage);
                ReplyMessageViewModel reply = new ReplyMessageViewModel();
                reply.BuildReplyMessage(detail, info.IdReplyMessage);

                ReplyMessage = reply;

                OnPropertyChanged(nameof(HasReplyMessage));
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