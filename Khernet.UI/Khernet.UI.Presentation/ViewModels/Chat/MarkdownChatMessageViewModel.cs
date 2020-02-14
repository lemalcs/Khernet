using Khernet.Core.Host;
using Khernet.Core.Utility;
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
    /// View model for text messages.
    /// </summary>
    public class MarkdownChatMessageViewModel : ChatMessageItemViewModel, ITextObserver
    {
        #region Properties

        private readonly IMessageManager messageManager;

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

        public ICommand SaveMarkdownSourceCommand { get; private set; }
        public ICommand SaveHtmlSourceCommand { get; private set; }

        public MarkdownChatMessageViewModel(IMessageManager messageManager)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");

            ReplyCommand = new RelayCommand(Reply, IsReadyMessage);
            ResendCommand = new RelayCommand(Resend, IsReadyMessage);

            State = ChatMessageState.Pendding;
            SaveMarkdownSourceCommand = new RelayCommand(SaveMarkdownSource);
            SaveHtmlSourceCommand = new RelayCommand(SaveHtmlSource);

            UID = Guid.NewGuid().ToString().Replace("-", "");
        }

        private async void SaveHtmlSource(object obj)
        {
            try
            {
                string newFileName = IoCContainer.UI.ShowSaveFileDialog("HTML document | *.html | All files | *.*", null);
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
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
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
                string newFileName = IoCContainer.UI.ShowSaveFileDialog("Markdown document | *.md | All files | *.*", null);
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
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
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
        /// Prepares this message to be resend to other user
        /// </summary>
        /// <param name="obj"></param>
        private void Resend(object obj)
        {
            messageManager.ResendMessage(this);
        }

        /// <summary>
        /// Replies a received or sent message
        /// </summary>
        /// <param name="obj"></param>
        private void Reply(object obj)
        {
            messageManager.SendReplyMessage(this);
        }

        /// <summary>
        /// Gets a summary about this message
        /// </summary>
        /// <param name="operation">The operation to do this this summary</param>
        /// <returns>A <see cref="ReplyMessageViewModel"/> containing summary</returns>
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
                reply.User = User;
            reply.IsReplying = true;

            var messageContent = IoCContainer.UI.ConvertMarkdownToDocument(TextContent);
            reply.SetTextContent(messageContent);

            reply.Operation = operation;
            reply.Id = Id;

            return reply;
        }

        public void ProcessMessage(int idMessage)
        {
            Text = new TextRequest
            {
                Id = idMessage,
                OperationRequest = MessageOperation.Download,
            };

            IoCContainer.Text.ProcessText(this);
        }

        public void ProcessMessage(byte[] message)
        {
            Text = new TextRequest
            {
                Content = message,
                OperationRequest = MessageOperation.Upload,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                FileType = MessageType.Markdown,
                UID = UID,
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
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                Content = EncodeText(TextContent),
                FileType = MessageType.Markdown,
                UID = UID,
            };

            IoCContainer.Text.ProcessText(this);
        }

        /// <summary>
        /// Get a copy of this chat message
        /// </summary>
        /// <returns></returns>
        public override ChatMessageItemViewModel Clone()
        {
            MarkdownChatMessageViewModel chatMessage = new MarkdownChatMessageViewModel(messageManager);
            chatMessage.IsSentByMe = true;
            chatMessage.TextContent = TextContent;

            return chatMessage;
        }

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
    }
}