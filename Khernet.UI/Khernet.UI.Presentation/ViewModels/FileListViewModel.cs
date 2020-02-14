using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using Khernet.UI.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Khernet.UI
{
    public class FileListViewModel : BaseModel, IMessageManager
    {
        private readonly IMessageManager messageManager;

        /// <summary>
        /// Indicates if file list is loading
        /// </summary>
        private bool isLoading;

        private bool isEmpty;

        public Action Done { get; set; }

        /// <summary>
        /// The file list
        /// </summary>
        private ObservableCollection<FileMessageItemViewModel> items;

        public ObservableCollection<FileMessageItemViewModel> Items
        {
            get => items;
            private set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }

        /// <summary>
        /// The user to whom files were sent or received
        /// </summary>
        public UserItemViewModel User { get; set; }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public bool IsEmpty
        {
            get => isEmpty;
            set
            {
                if (isEmpty != value)
                {
                    isEmpty = value;
                    OnPropertyChanged(nameof(IsEmpty));
                }
            }
        }

        public FileListViewModel()
        {

        }

        public FileListViewModel(IMessageManager messageManager)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");

            IsLoading = true;
            IsEmpty = false;
        }

        /// <summary>
        /// Loads files asynchronously
        /// </summary>
        /// <param name="messageType">The type of message to load</param>
        public void LoadFiles(MessageType messageType)
        {
            IoCContainer.UI.ExecuteAsync(() =>
            {
                try
                {
                    if (User == null)
                    {
                        IsLoading = false;
                        IsEmpty = true;
                        return;
                    }


                    List<int> fileList = IoCContainer.Get<Messenger>().GetFileList(User.Token, (ContentType)((int)messageType));

                    if (fileList == null)
                    {
                        IsLoading = false;
                        IsEmpty = true;
                        return;
                    }


                    if (Items == null)
                        Items = new ObservableCollection<FileMessageItemViewModel>();

                    for (int i = 0; i < fileList.Count; i++)
                    {
                        LoadMessage(fileList[i], messageType);
                    }

                    IsEmpty = fileList.Count == 0;
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }

        public async void ResendMessage(ChatMessageItemViewModel messageModel, UserItemViewModel receiver = null)
        {
            var pagedVM = new PagedDialogViewModel();

            //Set list of settings as first page
            pagedVM.CurrentPage = ApplicationPage.Resend;

            //Title for page
            pagedVM.Category = "Select a user: ";

            ResendViewModel resend = new ResendViewModel
            {
                Items = IoCContainer.Get<UserListViewModel>().Clone(),
                SelectedUser = null,
                Message = messageModel,
            };

            //Set the view model for settings list
            pagedVM.CurrentViewModel = resend;

            pagedVM.SetHomePage(pagedVM.CurrentPage, pagedVM.Category, pagedVM.CurrentViewModel);

            await IoCContainer.UI.ShowChildDialog(pagedVM);

            if (resend.SelectedUser != null)
            {
                messageManager.ResendMessage(messageModel, resend.SelectedUser);
                Done();
            }
        }

        public void SendReplyMessage(ChatMessageItemViewModel messageModel)
        {
            messageManager.SendReplyMessage(messageModel);
            Done();
        }

        private void LoadMessage(int idMessage, MessageType messageType)
        {
            var chat = IoCContainer.Get<Messenger>().GetMessageDetail(idMessage);

            UserItemViewModel user = chat.SenderToken == IoCContainer.Get<IIdentity>().Token ? null : User;

            try
            {
                switch (messageType)
                {
                    case MessageType.Image:

                        ImageChatMessageViewModel imageModel = new ImageChatMessageViewModel(this);
                        imageModel.Id = idMessage;
                        imageModel.User = user;
                        imageModel.IsSentByMe = user == null;

                        FileInformation info = JSONSerializer<FileInformation>.DeSerialize(
                           IoCContainer.Get<Messenger>().GetMessageContent(idMessage));

                        imageModel.FileName = info.FileName;
                        imageModel.FileSize = info.Size;

                        byte[] thumbNail = IoCContainer.Get<Messenger>().GetThumbnail(idMessage);

                        if (thumbNail != null)
                        {
                            imageModel.SetImageThumbnail(thumbNail);
                        }

                        string outFile = IoCContainer.Get<Messenger>().GetCacheFilePath(idMessage);

                        imageModel.FilePath = outFile;
                        imageModel.OnCompleted(new ChatMessageProcessResult(idMessage, ChatMessageState.Processed));

                        Items.Add(imageModel);

                        break;
                    case MessageType.Video:

                        VideoChatMessageViewModel videoModel = new VideoChatMessageViewModel(this);
                        videoModel.Id = idMessage;
                        videoModel.User = user;
                        videoModel.IsSentByMe = user == null;

                        info = JSONSerializer<FileInformation>.DeSerialize(
                           IoCContainer.Get<Messenger>().GetMessageContent(idMessage));

                        videoModel.FileName = info.FileName;
                        videoModel.FileSize = info.Size;
                        videoModel.Duration = info.Duration;
                        videoModel.SendDate = IoCContainer.Get<Messenger>().GetMessageDetail(idMessage).SendDate;

                        thumbNail = IoCContainer.Get<Messenger>().GetThumbnail(idMessage);

                        if (thumbNail != null)
                        {
                            videoModel.SetImageThumbnail(thumbNail);
                        }

                        outFile = IoCContainer.Get<Messenger>().GetCacheFilePath(idMessage);

                        videoModel.FilePath = outFile;
                        videoModel.OnCompleted(new ChatMessageProcessResult(idMessage, ChatMessageState.Processed));

                        Items.Add(videoModel);

                        break;

                    case MessageType.Binary:

                        FileChatMessageViewModel fileModel = new FileChatMessageViewModel(this);
                        fileModel.Id = idMessage;
                        fileModel.User = user;
                        fileModel.IsSentByMe = user == null;

                        info = JSONSerializer<FileInformation>.DeSerialize(
                            IoCContainer.Get<Messenger>().GetMessageContent(idMessage));

                        outFile = IoCContainer.Get<Messenger>().GetCacheFilePath(idMessage);

                        fileModel.IsFileLoaded = File.Exists(outFile);

                        fileModel.FilePath = outFile;
                        fileModel.FileName = info.FileName;
                        fileModel.FileSize = info.Size;
                        fileModel.SendDate = chat.SendDate;

                        fileModel.OnCompleted(new ChatMessageProcessResult(idMessage, ChatMessageState.Processed));

                        Items.Add(fileModel);

                        break;

                    case MessageType.Audio:

                        AudioChatMessageViewModel audioModel = new AudioChatMessageViewModel(this);
                        audioModel.Id = idMessage;
                        audioModel.User = user;
                        audioModel.IsSentByMe = user == null;

                        info = JSONSerializer<FileInformation>.DeSerialize(
                            IoCContainer.Get<Messenger>().GetMessageContent(idMessage));

                        outFile = IoCContainer.Get<Messenger>().GetCacheFilePath(idMessage);

                        audioModel.IsFileLoaded = File.Exists(outFile);

                        audioModel.FileName = info.FileName;
                        audioModel.FileSize = info.Size;
                        audioModel.Duration = info.Duration;
                        audioModel.SendDate = chat.SendDate;

                        outFile = IoCContainer.Get<Messenger>().GetCacheFilePath(idMessage);

                        audioModel.FilePath = outFile;

                        audioModel.OnCompleted(new ChatMessageProcessResult(idMessage, ChatMessageState.Processed));

                        Items.Add(audioModel);

                        break;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}
