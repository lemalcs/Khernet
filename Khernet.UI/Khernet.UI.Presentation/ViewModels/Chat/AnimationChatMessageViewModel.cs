using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
    /// </summary>
    public class AnimationChatMessageViewModel : FileMessageItemViewModel, IFileObserver
    {
        #region Properties

        private readonly IMessageManager messageManager;

        /// <summary>
        /// Width of video
        /// </summary>
        private double width;

        /// <summary>
        /// Height of video
        /// </summary>
        private double height;

        /// <summary>
        /// Thumbnail of image
        /// </summary>
        private ReadOnlyCollection<byte> thumbnail;

        /// <summary>
        /// Stores media information such as video, audio or files
        /// </summary>
        public MediaRequest Media { get; set; }

        public double Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public double Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        public ReadOnlyCollection<byte> Thumbnail
        {
            get => thumbnail;
            private set
            {
                if (thumbnail != value)
                {
                    thumbnail = value;
                    OnPropertyChanged(nameof(Thumbnail));
                }
            }
        }


        #endregion

        #region Commands

        /// <summary>
        /// Command for open animation file
        /// </summary>
        public ICommand OpenMediaCommand { get; private set; }


        /// <summary>
        /// Command for save animation for future messages
        /// </summary>
        public ICommand SaveAnimationCommand { get; private set; }

        #endregion

        public AnimationChatMessageViewModel(IMessageManager messageManager)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");

            OpenMediaCommand = new RelayCommand(OpenImage);
            ReplyCommand = new RelayCommand(Reply, IsReadyMessage);
            ResendCommand = new RelayCommand(Resend, IsReadyMessage);
            SaveAnimationCommand = new RelayCommand(SaveAnimation, IsReadyMessage);

            Height = double.PositiveInfinity;
            width = double.PositiveInfinity;

            State = ChatMessageState.Pendding;

            UID = Guid.NewGuid().ToString().Replace("-", "");
        }

        private async void SaveAnimation(object obj)
        {
            string newFile = FilePath;
            try
            {
                string mp4Extension = "avi";

                //Get a new file name with MP4 extension
                newFile = FileHelper.GetFileNameWithExtension(FilePath, mp4Extension);

                if (File.Exists(newFile))
                    newFile = FileHelper.GetNewFileName(newFile);

                SaveFile(newFile);

                //The height of row in GIF gallery
                int rowHeight = 100;

                int newWidth = (int)Width;

                //Get new width according to gallery row height
                if (Height > rowHeight)
                {
                    newWidth = (int)Width * rowHeight / (int)Height;
                }

                //Get new height according to gallery row height
                int newHeight = Height < rowHeight ? (int)Height : rowHeight;

                //Convert AVI video to MP4 format with a smaller size
                MediaHelper.ConvertTo(FilePath, newFile, newWidth, newHeight);

                byte[] animation = null;

                //Read the generated MP4 video file
                using (FileStream fs = new FileStream(newFile, FileMode.Open, FileAccess.Read, FileShare.None))
                using (MemoryStream mem = new MemoryStream())
                {
                    byte[] buffer = new byte[1024];

                    int readBytes = fs.Read(buffer, 0, buffer.Length);

                    while (readBytes > 0)
                    {
                        mem.Write(buffer, 0, readBytes);
                        readBytes = fs.Read(buffer, 0, buffer.Length);
                    }
                    animation = mem.ToArray();
                }

                //Get id file stored in database
                string idFile = IoCContainer.Get<Messenger>().GetFileId(Id);

                //Save MP4 video file to database
                int idAnimation = IoCContainer.Get<Messenger>().SaveAnimation(Id, idFile, newWidth, newHeight, animation);

                if (idAnimation > 0)
                    IoCContainer.Get<ChatMessageListViewModel>().AddAnimationToGallery(idAnimation);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error while saving GIF",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                }); ;
            }
            finally
            {
                try
                {
                    //Delete new generated file
                    if (newFile != FilePath)
                    {
                        if (File.Exists(newFile))
                            File.Delete(newFile);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

        }

        /// <summary>
        /// Requests to send a new message
        /// </summary>
        /// <param name="obj"></param>
        private void Resend(object obj)
        {
            messageManager.ResendMessage(this);
        }

        /// <summary>
        /// Replies a message that was sent
        /// </summary>
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
            reply.SetThumbnail(Thumbnail.ToArray());
            reply.FileName = "GIF";
            reply.Operation = operation;
            reply.Id = Id;

            return reply;
        }

        /// <summary>
        /// Process an animation
        /// </summary>
        /// <param name="fileName"></param>
        public void ProcessAnimation(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
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

                Thumbnail = new ReadOnlyCollection<byte>(mem.ToArray());
            }

            //Requesta upload an animation
            Media = new MediaRequest
            {
                FileName = fileName,
                FileType = MessageType.GIF,
                OperationRequest = MessageOperation.Upload,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            //Process request
            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        public void ProcessAnimation(int idMessage)
        {
            //Request to upload and image retrieved from database
            Media = new MediaRequest
            {
                Id = idMessage,
                FileType = MessageType.GIF,
                OperationRequest = MessageOperation.Download,
            };

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        /// <summary>
        /// Opens an image in its original size within a model dialog
        /// </summary>
        public void OpenImage()
        {
            //Show model dialog for image
            IoCContainer.UI.ShowDialog(this);
        }

        public override ChatMessageItemViewModel Clone()
        {
            AnimationChatMessageViewModel chatMessage = new AnimationChatMessageViewModel(messageManager);
            chatMessage.IsSentByMe = true;
            chatMessage.FilePath = FilePath;
            chatMessage.FileName = FileName;
            chatMessage.IsReadingFile = IsReadingFile;
            chatMessage.IsFileLoaded = IsFileLoaded;
            chatMessage.ResendId = Id;
            chatMessage.Width = Width;
            chatMessage.Height = Height;
            chatMessage.Thumbnail = Thumbnail;

            return chatMessage;
        }

        #region IMediaObservable members
        public void OnGetMetadata(FileResponse info)
        {
            IsLoading = false;

            if (info.Operation == Managers.MessageOperation.Download)
            {
                //Get thumbanil of GIF
                if (info.ThumbnailBytes != null)
                {
                    Thumbnail = new ReadOnlyCollection<byte>(info.ThumbnailBytes);
                }

                UID = info.UID;
            }

            Height = info.Height;
            Width = info.Width;

            FilePath = info.FilePath;
            FileName = info.OriginalFileName;
            FileSize = info.Size;

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

            OnPropertyChanged(nameof(FilePath));
        }

        public void OnError(Exception exception)
        {
            IsReadingFile = false;
            IsLoading = false;
        }

        #endregion

        public override void ProcessResend()
        {
            //Request to upload and image retrieved from database
            Media = new MediaRequest
            {
                Id = ResendId,
                FileName = FileName,
                FileType = MessageType.GIF,
                OperationRequest = MessageOperation.Resend,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        public void SetThumbnail(byte[] thumbnailBytes)
        {
            if (thumbnailBytes == null || thumbnailBytes.Length == 0)
                return;

            Thumbnail = new ReadOnlyCollection<byte>(thumbnailBytes);
        }
    }
}