using Khernet.Core.Host;
using Khernet.UI.Files;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
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

        public AudioChatMessageViewModel(IMessageManager messageManager)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");

            OpenMediaCommand = new RelayCommand(OpenAudio);
            CloseMediaCommand = new RelayCommand(CloseMedia);
            ReplyCommand = new RelayCommand(Reply, IsReadyMessage);
            ResendCommand = new RelayCommand(Resend, IsReadyMessage);

            State = ChatMessageState.Pendding;

            UID = Guid.NewGuid().ToString().Replace("-", "");
        }

        private void Resend(object obj)
        {
            messageManager.ResendMessage(this);
        }

        /// <summary>
        /// Gets a summary about this message
        /// </summary>
        /// <param name="operation">The operation to do this this summary</param>
        /// <returns>A <see cref="ReplyMessageViewModel"/>An object containing summary</returns>
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
                OperationRequest = Managers.MessageOperation.Upload,
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
                OperationRequest = Managers.MessageOperation.Download
            };

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        /// <summary>
        /// Opens an image in its original size within a model dialog
        /// </summary>
        public void OpenAudio()
        {
            FileOperations operations = new FileOperations();
            if (File.Exists(FilePath) && operations.VerifyFileIntegrity(FilePath, FileSize, Id))
            {
                //Show audio player
                IoCContainer.UI.ShowPlayer(this);
            }
            else
            {
                IsFileLoaded = false;
                ProcessAudio(Id);
            }
        }

        /// <summary>
        /// Gets a copy of this message
        /// </summary>
        /// <returns>A <see cref="AudioChatMessageViewModel"/> object</returns>
        public override ChatMessageItemViewModel Clone()
        {
            AudioChatMessageViewModel chatMessage = new AudioChatMessageViewModel(messageManager);
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
                OperationRequest = Managers.MessageOperation.Resend,
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

        #region IFileObserver members

        public void OnGetMetadata(FileResponse info)
        {
            if (info.Operation == Managers.MessageOperation.Download)
            {
                FileName = info.OriginalFileName;
                FilePath = info.FilePath;
                UID = info.UID;
            }

            //Get duration of audio file
            Duration = info.Duration;

            FileSize = info.Size;

            //Get file name with extension
            FileName = Path.GetFileName(info.OriginalFileName);

            IsLoading = false;
            IsReadingFile = true;
        }

        public void OnProcessing(long bytesProcessed)
        {
            CurrentReadBytes = bytesProcessed;
        }

        public void OnCompleted(ChatMessageProcessResult result)
        {
            Id = result.Id;
            IsMessageLoaded = true;
            SetChatState(result.State);

            IsReadingFile = false;
            IsFileLoaded = true;
        }

        public void OnError(Exception exception)
        {
            IsReadingFile = false;
            IsLoading = false;
            SetChatState(ChatMessageState.Error);
        }

        #endregion
    }
}