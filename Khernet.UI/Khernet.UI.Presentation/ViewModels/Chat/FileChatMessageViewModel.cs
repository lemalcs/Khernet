using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.Files;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.IO;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for any binary file.
    /// </summary>
    public class FileChatMessageViewModel : FileMessageItemViewModel, IFileObserver
    {
        #region Properties

        private readonly IMessageManager messageManager;

        public MediaRequest Media { get; set; }

        private IApplicationDialog applicationDialog;

        #endregion

        public ICommand OpenFileCommand { get; private set; }

        public FileChatMessageViewModel(IMessageManager messageManager, IApplicationDialog applicationDialog) : base(applicationDialog)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");
            this.applicationDialog = applicationDialog ?? throw new ArgumentNullException($"{nameof(IApplicationDialog)} cannot be null");

            OpenFileCommand = new RelayCommand(OpenFile, VerifyLoadedFile);
            ReplyCommand = new RelayCommand(Reply, IsReadyMessage);
            ResendCommand = new RelayCommand(Resend, IsReadyMessage);

            State = ChatMessageState.Pending;

            UID = Guid.NewGuid().ToString().Replace("-", "");
            TimeId = DateTimeOffset.Now.Ticks;

            FileState = FileChatState.NotDownloaded;
        }

        private bool VerifyLoadedFile()
        {
            return IsMessageLoaded && (State == ChatMessageState.Pending || State == ChatMessageState.Processed);
        }

        private void Resend()
        {
            messageManager.ResendMessage(this);
        }

        /// <summary>
        /// Gets a summary about this message.
        /// </summary>
        /// <param name="operation">The operation to do this summary.</param>
        /// <returns>A <see cref="ReplyMessageViewModel"/>An object containing summary.</returns>
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
            reply.FileName = FileName;
            reply.IconName = "File";
            reply.Operation = operation;
            reply.Id = Id;

            return reply;
        }

        /// <summary>
        /// Replies a message that was sent.
        /// </summary>
        private void Reply()
        {
            messageManager.SendReplyMessage(this);
        }

        /// <summary>
        /// Process binary from file.
        /// </summary>
        /// <param name="fileName">The path of file.</param>
        public override void Send(string filePath)
        {
            //Get file name
            FileName = Path.GetFileName(filePath);

            //Get full path of file
            FilePath = filePath;

            //Request upload a file
            Media = new MediaRequest
            {
                FileName = filePath,
                FileType = MessageType.Binary,
                OperationRequest = MessageOperation.Upload,
                ChatMessage = this,
            };

            //Process file
            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        public override void Load(MessageItem messageItem)
        {
            base.Load(messageItem);

            //Request to upload and image retrieved from database
            Media = new MediaRequest
            {
                FileType = MessageType.Binary,
                OperationRequest = MessageOperation.GetMetadata,
                ChatMessage = this,
            };

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        private void DownloadFile()
        {
            //Request to download file from database
            Media = new MediaRequest
            {
                FileType = MessageType.Binary,
                OperationRequest = MessageOperation.Download,
                ChatMessage = this,
            };

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        /// <summary>
        /// Open requested binary file.
        /// </summary>
        private void OpenFile()
        {
            try
            {
                FileOperations operations = new FileOperations();
                if (operations.VerifyFileIntegrity(FilePath, FileSize, Id))
                {
                    FileState = FileChatState.Ready;

                    //Open file with default external program
                    IoCContainer.UI.OpenFile(FilePath);
                }
                else if (IsFileLoaded && FileState == FileChatState.Ready)
                {
                    //Request to download to cache
                    FileState = FileChatState.NotDownloaded;
                    FilePath = string.Empty;
                }
                else
                {
                    //Download the actual file to cache
                    IsFileLoaded = false;
                    DownloadFile();
                }
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
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
        /// Get a copy of this message with the given dependencies.
        /// </summary>
        /// <param name="messageManager">The chat list to which this message belongs.</param>
        /// <param name="applicationDialog">The application window that this message belongs.</param>
        /// <returns>A <see cref="ChatMessageItemViewModel"/> instance with a copy of this message.</returns>
        public FileMessageItemViewModel GetInstanceCopy(IMessageManager messageManager, IApplicationDialog applicationDialog)
        {
            FileChatMessageViewModel chatMessage = new FileChatMessageViewModel(messageManager, applicationDialog);
            chatMessage.IsSentByMe = true;
            chatMessage.FilePath = FilePath;
            chatMessage.FileName = FileName;
            chatMessage.ResendFileId = Id;
            chatMessage.FileSize = FileSize;

            return chatMessage;
        }


        #region IFileObserver members

        public void OnGetMetadata(FileResponse info)
        {
            if (info.Operation == MessageOperation.GetMetadata)
            {
                FileName = info.OriginalFileName;
                UID = info.UID;
                TimeId = info.TimeId;

                //Get file size in bytes
                FileSize = info.Size;

                SendDate = info.SendDate;
            }

            FilePath = info.FilePath;

            IsLoading = false;
            IsReadingFile = true;
            FileState = FileChatState.Ready;
        }

        public void OnProcessing(long bytesProcessed)
        {
            CurrentReadBytes = bytesProcessed;
        }

        public void OnCompleted(ChatMessageProcessResult result)
        {
            Id = result.Id;

            SetChatState(result.State);

            IsReadingFile = false;
            IsFileLoaded = true;

            if (State == ChatMessageState.Error || State == ChatMessageState.UnCommited)
                FileState = FileChatState.Damaged;

            IsMessageLoaded = true;
        }

        public void OnError(Exception exception)
        {
            IsReadingFile = false;
            IsLoading = false;

            FileState = FileChatState.Damaged;
        }

        #endregion

        /// <summary>
        /// Sends this message to other user.
        /// </summary>
        public override void ProcessResend()
        {
            //Request upload a file
            Media = new MediaRequest
            {
                FileName = FileName,
                FileType = MessageType.Binary,
                OperationRequest = MessageOperation.Resend,
                ChatMessage = this,
            };

            //Process file
            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }
    }
}
