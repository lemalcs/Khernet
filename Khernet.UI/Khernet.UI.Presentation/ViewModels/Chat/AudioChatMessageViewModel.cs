using Khernet.Core.Host;
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
    /// View model for audio messages.
    /// </summary>
    public class AudioChatMessageViewModel : FileMessageItemViewModel, IFileObserver
    {
        #region Properties

        private readonly IMessageManager messageManager;

        /// <summary>
        /// The duration of video
        /// </summary>
        private TimeSpan duration;

        public MediaRequest Media { get; set; }

        private IApplicationDialog applicationDialog;

        public TimeSpan Duration
        {
            get => duration;
            set
            {
                if (duration != value)
                {
                    duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        #endregion

        #region Commands
        public ICommand OpenMediaCommand { get; private set; }

        public ICommand CloseMediaCommand { get; private set; }

        public ICommand PlayCommand { get; set; }

        #endregion

        public AudioChatMessageViewModel(IMessageManager messageManager,IApplicationDialog applicationDialog):base(applicationDialog)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");
            this.applicationDialog = applicationDialog ?? throw new ArgumentNullException($"{nameof(IApplicationDialog)} cannot be null");

            OpenMediaCommand = new RelayCommand(OpenAudio, VerifyLoadedAudio);
            CloseMediaCommand = new RelayCommand(CloseMedia);
            ReplyCommand = new RelayCommand(Reply, IsReadyMessage);
            ResendCommand = new RelayCommand(Resend, IsReadyMessage);

            State = ChatMessageState.Pendding;

            UID = Guid.NewGuid().ToString().Replace("-", "");
        }

        private bool VerifyLoadedAudio(object obj)
        {
            return IsMessageLoaded && (State == ChatMessageState.Pendding || State == ChatMessageState.Processed);
        }

        private void Resend(object obj)
        {
            messageManager.ResendMessage(this);
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
                reply.User = User;

            reply.IsSentByMe = IsSentByMe;
            reply.State = State;
            reply.IsReplying = true;
            reply.FileName = FileName;
            reply.IconName = "Play";
            reply.Operation = operation;
            reply.Id = Id;

            return reply;
        }

        /// <summary>
        /// Replies a message that was sent
        /// </summary>
        private void Reply(object obj)
        {
            messageManager.SendReplyMessage(this);
        }

        private void CloseMedia(object obj)
        {
            IoCContainer.Get<ApplicationViewModel>().IsPlayerVisible = false;
        }

        public void ProcessAudio(string fileName)
        {
            //Get audio file name
            FileName = Path.GetFileName(fileName);

            //Get full path of audio file
            FilePath = fileName;

            //Request upload an audio file
            Media = new MediaRequest
            {
                FileName = fileName,
                OperationRequest = MessageOperation.Upload,
                FileType = MessageType.Audio,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            //Process request
            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        public void ProcessAudio(int idMessage)
        {
            //Request to upload and image retrieved from database
            Media = new MediaRequest
            {
                Id = idMessage,
                FileType = MessageType.Audio,
                OperationRequest = MessageOperation.GetMetadata
            };

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        /// <summary>
        /// Opens an image in its original size within a model dialog
        /// </summary>
        public void OpenAudio(object par)
        {
            FileOperations operations = new FileOperations();
            if (File.Exists(FilePath) && operations.VerifyFileIntegrity(FilePath, FileSize, Id))
            {
                //Show audio player
                IoCContainer.UI.ShowPlayer(this);
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
                DownloadFile(Id);
            }
        }

        /// <summary>
        /// Get a copy of this message with the given dependencies
        /// </summary>
        /// <param name="messageManager">The chat list to which this message belongs</param>
        /// <param name="applicationDialog">The application window that this message belongs</param>
        /// <returns>A <see cref="ChatMessageItemViewModel"/> instace with a copy of this message</returns>
        public FileMessageItemViewModel GetInstanceCopy(IMessageManager messageManager, IApplicationDialog applicationDialog)
        {
            AudioChatMessageViewModel chatMessage = new AudioChatMessageViewModel(messageManager, applicationDialog);
            chatMessage.IsSentByMe = true;
            chatMessage.FilePath = FilePath;
            chatMessage.FileName = FileName;
            chatMessage.ResendId = Id;
            chatMessage.FileSize = FileSize;

            return chatMessage;
        }

        public override void ProcessResend()
        {
            //Request upload an audio file
            Media = new MediaRequest
            {
                Id = ResendId,
                FileName = FileName,
                OperationRequest = MessageOperation.Resend,
                FileType = MessageType.Audio,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            //Process request
            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        private void DownloadFile(int idMessage)
        {
            //Request to upload and image retrieved from database
            Media = new MediaRequest
            {
                Id = idMessage,
                FileType = MessageType.Audio,
                OperationRequest = MessageOperation.Download
            };

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        #region IFileObserver members

        public void OnGetMetadata(FileResponse info)
        {
            if (info.Operation == MessageOperation.GetMetadata)
            {
                FileName = info.OriginalFileName;
                FilePath = info.FilePath;
                UID = info.UID;

                //Get duration of audio file
                Duration = info.Duration;

                FileSize = info.Size;

                //Get file name with extension
                FileName = Path.GetFileName(info.OriginalFileName);

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
        }

        #endregion
    }
}