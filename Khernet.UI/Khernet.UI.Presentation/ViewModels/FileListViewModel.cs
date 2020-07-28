using Khernet.Core.Host;
using Khernet.Services.Messages;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
                if (itemsCount != value)
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

                    List<MessageItem> fileList = IoCContainer.Get<Messenger>().GetFileList(
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

                    foreach (MessageItem message in fileList)
                    {
                        LoadMessage(message);
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

        private void LoadMessage(MessageItem messageItem)
        {
            try
            {
                FileMessageItemViewModel fileMessage = null;

                switch (messageItem.Format)
                {
                    case ContentType.Image:
                        fileMessage = new ImageChatMessageViewModel(this, modalDialog);
                        break;

                    case ContentType.Video:
                        fileMessage = new VideoChatMessageViewModel(this, modalDialog);
                        break;

                    case ContentType.Binary:
                        fileMessage = new FileChatMessageViewModel(this, modalDialog);
                        break;

                    case ContentType.Audio:
                        fileMessage = new AudioChatMessageViewModel(this, modalDialog);
                        break;
                }

                fileMessage.DisplayUser = User;
                fileMessage.SenderUserId = messageItem.IdSenderPeer == 0 ? IoCContainer.Get<IIdentity>() : User;
                fileMessage.ReceiverUserId = messageItem.IdSenderPeer == 0 ? User : IoCContainer.Get<IIdentity>();

                if (fileMessage is ImageChatMessageViewModel)
                    ((ImageChatMessageViewModel)fileMessage).LoadMetadata(messageItem);
                else
                    fileMessage.Load(messageItem);

                Items.Add(fileMessage);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}
