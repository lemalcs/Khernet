using Khernet.Core.Host;
using Khernet.Services.Messages;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Khernet.UI
{
    public class FileListViewModel : BaseModel, IMessageManager
    {
        private readonly IMessageManager messageManager;

        /// <summary>
        /// Indicates if file list is loading
        /// </summary>
        private bool isLoading;

        /// <summary>
        /// Indicates if file list is empty
        /// </summary>
        private bool isEmpty;

        /// <summary>
        /// Operation to be done on chat message such as reply o resend
        /// </summary>
        public Action Done { get; set; }

        /// <summary>
        /// The file list
        /// </summary>
        private ObservableCollection<FileMessageItemViewModel> items;

        /// <summary>
        /// Represents the objet to display dialogs
        /// </summary>
        private ModalApplicationDialog modalDialog;

        /// <summary>
        /// The number of loaded files
        /// </summary>
        private int itemsCount;

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

        public int ItemsCount 
        { 
            get => itemsCount;
            set 
            { 
                if(itemsCount != value)
                {
                    itemsCount = value;
                    OnPropertyChanged(nameof(ItemsCount));
                }
            }
        }

        /// <summary>
        /// Constructor needed for design time
        /// </summary>
        public FileListViewModel()
        {
            
        }

        public FileListViewModel(IMessageManager messageManager)
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");
            modalDialog = new ModalApplicationDialog();

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
                        return;
                    }

                    int lastIdMessage = 0;

                    if (Items != null && Items.Count > 0)
                        lastIdMessage = Items[Items.Count - 1].Id;

                    int quantity = 20;

                    Dictionary<int,int> fileList = IoCContainer.Get<Messenger>().GetFileList(
                        User.Token, 
                        (ContentType)((int)messageType),
                        lastIdMessage,
                        quantity
                        );

                    if (fileList == null)
                    {
                        IsLoading = false;
                        return;
                    }


                    if (Items == null)
                        Items = new ObservableCollection<FileMessageItemViewModel>();

                    foreach(var idMessage in fileList.Keys)
                    {
                        LoadMessage(idMessage,fileList[idMessage], messageType);
                    }

                    IsEmpty = Items.Count == 0;
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

        private void LoadMessage(int idMessage,int idUser, MessageType messageType)
        {
            try
            {
                UserItemViewModel user = idUser == 0 ? null : User;

                switch (messageType)
                {
                    case MessageType.Image:

                        ImageChatMessageViewModel imageModel = new ImageChatMessageViewModel(this, modalDialog);
                        imageModel.Id = idMessage;
                        imageModel.User = user;
                        imageModel.IsSentByMe = user == null;

                        imageModel.ProcessImage(idMessage);

                        Items.Add(imageModel);

                        break;
                    case MessageType.Video:

                        VideoChatMessageViewModel videoModel = new VideoChatMessageViewModel(this, modalDialog);
                        videoModel.Id = idMessage;
                        videoModel.User = user;
                        videoModel.IsSentByMe = user == null;

                        videoModel.ProcessVideo(idMessage);

                        Items.Add(videoModel);

                        break;

                    case MessageType.Binary:

                        FileChatMessageViewModel fileModel = new FileChatMessageViewModel(this,modalDialog);
                        fileModel.Id = idMessage;
                        fileModel.User = user;
                        fileModel.IsSentByMe = user == null;

                        fileModel.ProcessFile(idMessage);

                        Items.Add(fileModel);

                        break;

                    case MessageType.Audio:

                        AudioChatMessageViewModel audioModel = new AudioChatMessageViewModel(this,modalDialog);
                        audioModel.Id = idMessage;
                        audioModel.User = user;
                        audioModel.IsSentByMe = user == null;

                        audioModel.ProcessAudio(idMessage);

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
