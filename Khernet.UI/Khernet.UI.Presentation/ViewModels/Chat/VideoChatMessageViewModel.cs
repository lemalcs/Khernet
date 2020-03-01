using Khernet.Core.Host;
using Khernet.UI.Files;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for video messages.
    /// </summary>
    public class VideoChatMessageViewModel : FileMessageItemViewModel, IFileObserver
    {
        #region Properties

        private readonly IMessageManager messageManager;

        /// <summary>
        /// The default width of video
        /// </summary>
        private double videoWidth;

        /// <summary>
        /// The default height of video
        /// </summary>
        private double videoHeight;

        private readonly double defaultVideoWidth = 200;
        private readonly double defaultVideoHeight = 112.5;

        /// <summary>
        /// The duration of video
        /// </summary>
        private TimeSpan duration;

        /// <summary>
        /// Holds a request of video metadata
        /// </summary>
        public MediaRequest Media { get; set; }

        /// <summary>
        /// The duration of video
        /// </summary>
        public TimeSpan Duration { get => duration; set => duration = value; }

        /// <summary>
        /// The thumbnail of video
        /// </summary>
        private byte[] thumbnail;

        public double VideoWidth
        {
            get => videoWidth;
            set
            {
                if (videoWidth != value)
                {
                    videoWidth = value;
                    OnPropertyChanged(nameof(VideoWidth));
                }
            }
        }

        public double VideoHeight
        {
            get => videoHeight;
            set
            {
                if (videoHeight != value)
                {
                    videoHeight = value;
                    OnPropertyChanged(nameof(VideoHeight));
                }
            }
        }

        public ReadOnlyCollection<byte> Thumbnail
        {
            get; private set;
        }

        #endregion

        public ICommand OpenMediaCommand { get; private set; }

        public VideoChatMessageViewModel(IMessageManager messageManager)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");

            OpenMediaCommand = new RelayCommand(OpenVideo, VerifyLoadedVideo);
            ReplyCommand = new RelayCommand(Reply, IsReadyMessage);
            ResendCommand = new RelayCommand(Resend, IsReadyMessage);

            //Aspect ratio of 16:9
            VideoWidth = defaultVideoWidth;
            VideoHeight = defaultVideoHeight;

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
            reply.SetThumbnail(Thumbnail.ToArray());
            reply.FileName = FileName;
            reply.Operation = operation;
            reply.Id = Id;

            return reply;
        }

        /// <summary>
        /// Replies a received or sent message
        /// </summary>
        private void Reply(object obj)
        {
            messageManager.SendReplyMessage(this);
        }

        private bool VerifyLoadedVideo(object obj)
        {
            return IsFileLoaded;
        }

        /// <summary>
        /// Process video from file
        /// </summary>
        /// <param name="fileName"></param>
        public void ProcessVideo(string fileName)
        {
            //Get file name with extension
            FileName = Path.GetFileName(fileName);

            Media = new MediaRequest
            {
                FileName = fileName,
                FileType = MessageType.Video,
                OperationRequest = MessageOperation.Upload,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            IoCContainer.Media.ProcessFile(this);
            IsLoading = true;
        }

        public void ProcessVideo(int idMessage)
        {
            Id = idMessage;

            //Request to upload and image retrieved from database
            Media = new MediaRequest
            {
                Id = idMessage,
                FileType = MessageType.Video,
                OperationRequest = MessageOperation.Download
            };

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        /// <summary>
        /// Opens an image in its original size within a model dialog
        /// </summary>
        public async void OpenVideo(object parameter)
        {
            FileOperations operations = new FileOperations();
            if (File.Exists(FilePath) && operations.VerifyFileIntegrity(FilePath, FileSize, Id))
            {
                //Show image in dialog
                await IoCContainer.UI.ShowDialog(this);
            }
            else
            {
                IsFileLoaded = false;
                ProcessVideo(Id);
            }
        }

        public override ChatMessageItemViewModel Clone()
        {
            VideoChatMessageViewModel chatMessage = new VideoChatMessageViewModel(messageManager);
            chatMessage.IsSentByMe = true;
            chatMessage.FilePath = FilePath;
            chatMessage.FileName = FileName;
            chatMessage.ResendId = Id;
            chatMessage.Thumbnail = Thumbnail;
            chatMessage.VideoHeight = VideoHeight;
            chatMessage.VideoWidth = VideoWidth;
            chatMessage.FileSize = FileSize;

            return chatMessage;
        }

        /// <summary>
        /// Sends this message to other user
        /// </summary>
        public override void ProcessResend()
        {
            Media = new MediaRequest
            {
                Id = ResendId,
                FileName = FilePath,
                FileType = MessageType.Video,
                OperationRequest = MessageOperation.Resend,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        #region IFileObserver members

        /// <summary>
        /// Get video metadata.
        /// </summary>
        /// <param name="info">The metadata of video</param>
        public void OnGetMetadata(FileResponse info)
        {
            duration = info.Duration;

            //Check if file has video tracks to get thumbnail
            if (info.Thumbnail != null)
            {
                using (FileStream fs = new FileStream(info.Thumbnail, FileMode.Open, FileAccess.Read, FileShare.Read))
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

                //Resizing for video container message
                VideoWidth = 200;
                VideoHeight = info.Height * 200 / info.Width;
            }

            if (info.Operation == MessageOperation.Download)
            {
                if (info.ThumbnailBytes != null)
                {
                    SetImageThumbnail(info.ThumbnailBytes);
                }

                VideoWidth = defaultVideoWidth;

                if (info.Height == 0 || info.Width == 0)
                {
                    VideoHeight = defaultVideoHeight;
                }
                //Resizing for video container message just if height and width and greater than zero
                else
                {
                    VideoHeight = info.Height * 200 / info.Width;
                }

                FileName = Path.GetFileName(info.OriginalFileName);

                UID = info.UID;
            }

            //Set video file path
            FilePath = info.FilePath;

            //get video size in bytes
            FileSize = info.Size;

            //Notify that reading metadata has ended
            IsLoading = false;

            OnPropertyChanged(nameof(Duration));

            //Notify that reading operation has started
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

        /// <summary>
        /// Show an error
        /// </summary>
        /// <param name="exception">The error detail</param>
        public void OnError(Exception exception)
        {
            IsReadingFile = false;
            IsLoading = false;
        }

        #endregion

        public void SetImageThumbnail(byte[] thumbnailBytes)
        {
            Thumbnail = new ReadOnlyCollection<byte>(thumbnailBytes);
            OnPropertyChanged(nameof(Thumbnail));
        }
    }
}