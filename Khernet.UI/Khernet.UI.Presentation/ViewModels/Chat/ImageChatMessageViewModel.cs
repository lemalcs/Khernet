using Khernet.Core.Host;
using Khernet.UI.Files;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
    /// </summary>
    public class ImageChatMessageViewModel : FileMessageItemViewModel, IFileObserver
    {
        #region Properties

        private readonly IMessageManager messageManager;

        public MediaRequest Media { get; set; }

        private IApplicationDialog applicationDialog;

        public ReadOnlyCollection<byte> Thumbnail { get; private set; }

        #endregion

        #region Commands
        public ICommand OpenImageCommand { get; private set; }


        #endregion

        public ImageChatMessageViewModel(IMessageManager messageManager,IApplicationDialog applicationDialog):base(applicationDialog)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");
            this.applicationDialog=applicationDialog?? throw new ArgumentNullException($"{nameof(IApplicationDialog)} cannot be null");
            
            OpenImageCommand = new RelayCommand(OpenImage, VerifyLoadedImage);
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

            reply.IsReplying = true;
            reply.SetThumbnail(Thumbnail.ToArray());
            reply.FileName = "Image";
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

        private bool VerifyLoadedImage(object obj)
        {
            return IsFileLoaded;
        }

        /// <summary>
        /// Retrieve an image from file system.
        /// </summary>
        /// <param name="fileName">The path of image</param>
        public void ProcessImage(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (MemoryStream mem = new MemoryStream())
            {
                List<byte> imageBytes = new List<byte>();

                int chuckSize = 1024;

                byte[] buffer = new byte[chuckSize];
                int readBytes = fs.Read(buffer, 0, chuckSize);

                while (readBytes > 0)
                {
                    mem.Write(buffer, 0, readBytes);
                    readBytes = fs.Read(buffer, 0, chuckSize);
                }

                SetImageThumbnail(mem.ToArray());
            }

            //Request to updaload a image from file system
            Media = new MediaRequest
            {
                FileName = fileName,
                FileType = MessageType.Image,
                OperationRequest = MessageOperation.Upload,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            //Process the image file
            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        public void SetImageThumbnail(byte[] thumbnailBytes)
        {
            Thumbnail = new ReadOnlyCollection<byte>(thumbnailBytes);
            OnPropertyChanged(nameof(Thumbnail));
        }

        /// <summary>
        /// Retrieve and image from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="rawImage">The stream of image</param>
        public void ProcessImage(Stream rawImage)
        {
            using (MemoryStream mem = (MemoryStream)ImageHelper.GetStreamFromClipboardImage(rawImage))
            {
                SetImageThumbnail(mem.ToArray());
            }

            //Request to upload and image retrieved from clipboard
            Media = new MediaRequest
            {
                FileName = null,//Show that data comes from a stream
                FileData = rawImage,
                FileType = MessageType.Image,
                OperationRequest = MessageOperation.Upload,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            //Process the request
            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        public void ProcessImage(int idMessage)
        {
            //Request to upload and image retrieved from database
            Media = new MediaRequest
            {
                Id = idMessage,
                FileType = MessageType.Image,
                OperationRequest = MessageOperation.GetMetadata
            };

            Id = idMessage;

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        /// <summary>
        /// Opens an image in its original size within a model dialog
        /// </summary>
        public async void OpenImage(object parameter)
        {
            FileOperations operations = new FileOperations();
            if (File.Exists(FilePath) && operations.VerifyFileIntegrity(FilePath, FileSize, Id))
            {
                //Show image in dialog
                await applicationDialog.ShowDialog(this).ConfigureAwait(false);
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

        private void DownloadFile(int idMessage)
        {
            //Request to upload and image retrieved from database
            Media = new MediaRequest
            {
                Id = idMessage,
                FileType = MessageType.Image,
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
                if (info.ThumbnailBytes != null)
                {
                    SetImageThumbnail(info.ThumbnailBytes);
                }
                UID = info.UID;
                FileName = info.OriginalFileName;
                FileSize = info.Size;

                SendDate = info.SendDate;
            }

            FilePath = info.FilePath;

            IsLoading = false;

            OnPropertyChanged(nameof(IsFileLoaded));
            OnPropertyChanged(nameof(FilePath));

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

        /// <summary>
        /// Get a copy of this message with the given dependencies
        /// </summary>
        /// <param name="messageManager">The chat list to which this message belongs</param>
        /// <param name="applicationDialog">The application window that this message belongs</param>
        /// <returns>A <see cref="ChatMessageItemViewModel"/> instace with a copy of this message</returns>
        public FileMessageItemViewModel GetInstanceCopy(IMessageManager messageManager,IApplicationDialog applicationDialog)
        {
            ImageChatMessageViewModel chatMessage = new ImageChatMessageViewModel(messageManager, applicationDialog);
            chatMessage.IsSentByMe = true;
            chatMessage.FilePath = FilePath;
            chatMessage.FileName = FileName;
            chatMessage.ResendId = Id;
            chatMessage.Thumbnail = Thumbnail;
            chatMessage.SetImageThumbnail(Thumbnail.ToArray());

            return chatMessage;
        }

        public override void ProcessResend()
        {
            //Get image thumbnail
            if (File.Exists(FilePath))
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (MemoryStream mem = new MemoryStream())
                {
                    int chuckSize = 1024;

                    byte[] buffer = new byte[chuckSize];
                    int readBytes = fs.Read(buffer, 0, chuckSize);

                    while (readBytes > 0)
                    {
                        mem.Write(buffer, 0, readBytes);
                        readBytes = fs.Read(buffer, 0, chuckSize);
                    }

                    SetImageThumbnail(mem.ToArray());
                }
            }

            //Request to updaload a image from file system
            Media = new MediaRequest
            {
                Id = ResendId,
                FileName = FileName,
                FileType = MessageType.Image,
                OperationRequest = MessageOperation.Resend,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            //Process the image file
            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }
    }
}