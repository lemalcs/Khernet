using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.Files;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.IO;
using System.Windows.Input;

namespace Khernet.UI
{
    public class FileChatMessageViewModel : FileMessageItemViewModel, IFileObserver
    {
        #region Properties

        private readonly IMessageManager messageManager;

        public MediaRequest Media { get; set; }

        #endregion

        public ICommand OpenFileCommand { get; private set; }

        public FileChatMessageViewModel(IMessageManager messageManager)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");

            OpenFileCommand = new RelayCommand(OpenFile);
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
            reply.FileName = FileName;
            reply.IconName = "File";
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

        /// <summary>
        /// Process binary from file
        /// </summary>
        /// <param name="fileName">the path of file</param>
        public void ProcessFile(string fileName)
        {
            //Get file name
            FileName = Path.GetFileName(fileName);

            //Get full path of file
            FilePath = fileName;

            //Request upload a file
            Media = new MediaRequest
            {
                FileName = fileName,
                FileType = MessageType.Binary,
                OperationRequest = MessageOperation.Upload,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            //Process file
            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        public void ProcessFile(int idMessage)
        {
            //Request to upload and image retrieved from database
            Media = new MediaRequest
            {
                Id = idMessage,
                FileType = MessageType.Binary,
                OperationRequest = MessageOperation.Download
            };

            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        /// <summary>
        /// Open requested binary file
        /// </summary>
        private void OpenFile()
        {
            try
            {
                FileOperations operations = new FileOperations();
                if (File.Exists(FilePath) && operations.VerifyFileIntegrity(FilePath, FileSize, Id))
                {
                    //Open file with default external program
                    IoCContainer.UI.OpenFile(FilePath);
                }
                else
                {
                    IsFileLoaded = false;
                    ProcessFile(Id);
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
        /// Gets a copy of this message
        /// </summary>
        /// <returns></returns>
        public override ChatMessageItemViewModel Clone()
        {
            FileChatMessageViewModel chatMessage = new FileChatMessageViewModel(messageManager);
            chatMessage.IsSentByMe = true;
            chatMessage.FilePath = FilePath;
            chatMessage.FileName = FileName;
            chatMessage.ResendId = Id;
            chatMessage.FileSize = FileSize;

            return chatMessage;
        }

        #region IMediaObservable members
        public void OnGetMetadata(FileResponse info)
        {
            if (info.Operation == Managers.MessageOperation.Download)
            {
                FileName = info.OriginalFileName;

                //Set file path
                FilePath = info.FilePath;
                UID = info.UID;
            }

            //Get file size in bytes
            FileSize = info.Size;

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
        }

        /// <summary>
        /// Sends this message to other user
        /// </summary>
        public override void ProcessResend()
        {
            //Request upload a file
            Media = new MediaRequest
            {
                Id = ResendId,//Id,
                FileName = FileName,
                FileType = MessageType.Binary,
                OperationRequest = MessageOperation.Resend,
                SenderToken = IoCContainer.Get<IIdentity>().Token,
                ReceiptToken = User.Token,
                UID = UID,
                SendDate = SendDate,
            };

            //Process file
            IoCContainer.Media.ProcessFile(this);

            IsLoading = true;
        }

        #endregion
    }
}
