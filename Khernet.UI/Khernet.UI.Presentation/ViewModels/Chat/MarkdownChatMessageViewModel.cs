using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for markdown messages.
    /// </summary>
    public class MarkdownChatMessageViewModel : TextMessageItemViewModel, ITextObserver
    {
        #region Properties

        private readonly IMessageManager messageManager;

        private readonly IApplicationDialog applicationDialog;

        private string textContent;

        public string TextContent
        {
            get => textContent;
            private set
            {
                if (textContent != value)
                {
                    textContent = value;
                    OnPropertyChanged(nameof(TextContent));
                }
            }
        }

        public TextRequest Text { get; set; }

        #endregion

        #region Commands

        public ICommand SaveMarkdownSourceCommand { get; private set; }
        public ICommand SaveHtmlSourceCommand { get; private set; }

        #endregion

        public MarkdownChatMessageViewModel(IMessageManager messageManager, IApplicationDialog applicationDialog)//:base(applicationDialog)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");
            this.applicationDialog = applicationDialog ?? throw new ArgumentNullException($"{nameof(IApplicationDialog)} cannot be null");

            ReplyCommand = new RelayCommand(Reply, IsReadyMessage);
            ResendCommand = new RelayCommand(Resend, IsReadyMessage);

            State = ChatMessageState.Pending;
            SaveMarkdownSourceCommand = new RelayCommand(SaveMarkdownSource);
            SaveHtmlSourceCommand = new RelayCommand(SaveHtmlSource);

            UID = Guid.NewGuid().ToString().Replace("-", "");
            TimeId = DateTimeOffset.Now.Ticks;
        }

        private async void SaveHtmlSource()
        {
            try
            {
                string newFileName = applicationDialog.ShowSaveFileDialog("HTML document | *.html | All files | *.*", null);
                if (string.IsNullOrEmpty(newFileName))
                    return;

                using (StreamWriter sw = new StreamWriter(newFileName))
                {
                    sw.Write(Markdig.Markdown.ToHtml(TextContent));
                }
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await applicationDialog.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = error.Message,
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }

        private async void SaveMarkdownSource()
        {
            try
            {
                string newFileName = applicationDialog.ShowSaveFileDialog("Markdown document | *.md | All files | *.*", null);
                if (string.IsNullOrEmpty(newFileName))
                    return;

                using (StreamWriter sw = new StreamWriter(newFileName))
                {
                    sw.Write(TextContent);
                }
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await applicationDialog.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = error.Message,
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
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

            var messageContent = IoCContainer.UI.ConvertMarkdownToDocument(TextContent);
            reply.SetTextContent(messageContent);

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
                FileType = MessageType.Markdown,
                ChatMessage = this,
            };

            if (ReplyMessage != null)
            {
                Text.IdReplyMessage = ReplyMessage.Id;
            }

            TextContent = DecodeText(message);

            IoCContainer.Text.ProcessText(this);
        }

        private byte[] EncodeText(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        private string DecodeText(byte[] text)
        {
            return Encoding.UTF8.GetString(text);
        }

        public override void ProcessResend()
        {
            Text = new TextRequest
            {
                OperationRequest = MessageOperation.Upload,
                Content = EncodeText(TextContent),
                FileType = MessageType.Markdown,
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
            MarkdownChatMessageViewModel chatMessage = new MarkdownChatMessageViewModel(messageManager, applicationDialog);
            chatMessage.IsSentByMe = true;
            chatMessage.TextContent = TextContent;

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
                TextContent = DecodeText(info.Content);
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