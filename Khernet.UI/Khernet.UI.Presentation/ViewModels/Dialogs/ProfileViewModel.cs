using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.Cache;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for user profile.
    /// </summary>
    public class ProfileViewModel : BaseModel
    {
        #region Properties

        private readonly IMessageManager messageManager;

        /// <summary>
        /// The view model for user.
        /// </summary>
        private UserItemViewModel user;

        /// <summary>
        /// The path of image file.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Indicates if emoji gallery is open.
        /// </summary>
        private bool isEmojiGalleryOpen;

        public string FileName
        {
            get => fileName;
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        public UserItemViewModel User
        {
            get => user;
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        public bool IsEmojiGalleryOpen
        {
            get => isEmojiGalleryOpen;
            set
            {
                if (isEmojiGalleryOpen != value)
                {
                    isEmojiGalleryOpen = value;
                    OnPropertyChanged(nameof(isEmojiGalleryOpen));
                }
            }
        }

        private PagedDialogViewModel pagedDialog;

        #endregion

        #region Commands

        /// <summary>
        /// Command for update user name.
        /// </summary>
        public ICommand UpdateProfileImageCommand { get; private set; }

        /// <summary>
        /// Command for update avatar (image)
        /// </summary>
        public ICommand SaveProfileCommand { get; private set; }

        /// <summary>
        /// Command for edit user name.
        /// </summary>
        public ICommand OpenEditNameCommand { get; private set; }

        /// <summary>
        /// Commando for open emoji gallery.
        /// </summary>
        public ICommand OpenEmojiGalleyCommand { get; private set; }

        /// <summary>
        /// Command for open image files list.
        /// </summary>
        public ICommand OpenImageListCommand { get; private set; }

        /// <summary>
        /// Command for open binary files list.
        /// </summary>
        public ICommand OpenFileListCommand { get; private set; }

        /// <summary>
        /// Command for open video files list.
        /// </summary>
        public ICommand OpenVideoListCommand { get; private set; }

        /// <summary>
        /// Command for open audio files list.
        /// </summary>
        public ICommand OpenAudioListCommand { get; private set; }

        public ICommand OpenProfileImageCommand { get; private set; }

        #endregion

        public ProfileViewModel()
        {
            UpdateProfileImageCommand = new UI.RelayCommand(UpdateProfileImage);
            SaveProfileCommand = new RelayCommand(SaveProfile);
            OpenEditNameCommand = new RelayCommand(EditName);
            OpenEmojiGalleyCommand = new RelayCommand(OpenEmojiGallery);

            OpenImageListCommand = new RelayCommand(OpenImageList);
            OpenFileListCommand = new RelayCommand(OpenFileList);
            OpenVideoListCommand = new RelayCommand(OpenVideoList);
            OpenAudioListCommand = new RelayCommand(OpenAudioList);
            OpenProfileImageCommand = new RelayCommand(OpenProfileImage);
        }

        private async void OpenProfileImage()
        {
            string tempPath = Path.Combine(Configurations.CacheDirectory.FullName, User.Token);

            if (File.Exists(tempPath))
            {
                if (!FileHelper.CompareFileWithMemory(tempPath, user.Avatar.ToArray()))
                {
                    SaveAvatar(tempPath);
                }
            }
            else
            {
                SaveAvatar(tempPath);
            }

            ImageChatMessageViewModel imageModel = new ImageChatMessageViewModel(new ChatMessageListViewModel(), new ModalApplicationDialog())
            {
                FilePath = tempPath,
            };

            await IoCContainer.UI.ShowChildDialog(imageModel);
        }

        /// <summary>
        /// Save avatar to file system.
        /// </summary>
        /// <param name="fileName">The name of avatar file.</param>
        private void SaveAvatar(string fileName)
        {
            if (User.Avatar == null)
                return;

            using (MemoryStream dtStream = new MemoryStream(User.Avatar.ToArray()))
            {
                int chunk = 1048576;
                byte[] buffer = new byte[chunk];

                int readBytes = 0;

                using (FileStream fStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    readBytes = dtStream.Read(buffer, 0, chunk);

                    int actualReadBytes = readBytes;

                    while (readBytes > 0)
                    {
                        fStream.Write(buffer, 0, readBytes);

                        readBytes = dtStream.Read(buffer, 0, chunk);
                        actualReadBytes += readBytes;
                    }
                }
            }
        }

        public ProfileViewModel(IMessageManager messageManager) : this()
        {
            this.messageManager = messageManager ?? throw new ArgumentNullException($"{nameof(IMessageManager)} cannot be null");
        }

        private void OpenEmojiGallery()
        {
            IsEmojiGalleryOpen = true;
        }

        /// <summary>
        /// Edits user name to show on this application only.
        /// </summary>
        private async void EditName()
        {
            try
            {
                FieldEditorViewModel editor = new FieldEditorViewModel(this)
                {
                    FieldName = "Name",
                };
                editor.SourceDataField = User.SourceDisplayName;
                editor.SetDataField(this.User.DisplayName.ToArray());

                pagedDialog.ShowChildDialog(editor);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error while editing name",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }

        private async void UpdateProfileImage()
        {
            //Open file dialog
            string[] imagePath = IoCContainer.UI.ShowOpenFileDialog();

            //Check if an image was chosen
            if (imagePath != null)
            {
                try
                {
                    FileName = imagePath[0];

                    //Save image to database

                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        byte[] content = new byte[fs.Length];

                        fs.Read(content, 0, content.Length);

                        IoCContainer.Get<Messenger>().UpdateAvatar(content);

                        User.SetAvatarThumbnail(content);
                    }
                }
                catch (Exception error)
                {
                    LogDumper.WriteLog(error);
                    await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                    {
                        Message = "Error while updating avatar",
                        Title = "Khernet",
                        ShowAcceptOption = true,
                        AcceptOptionLabel = "OK",
                        ShowCancelOption = false,
                    });
                }
            }
        }

        /// <summary>
        /// Saves profile details of current logged user.
        /// </summary>
        /// <param name="valueContainer">The container for full name.</param>
        private async void SaveProfile(object valueContainer)
        {
            try
            {
                if (!FieldValidator.ValidateUserName(User.Username))
                {
                    await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                    {
                        Message = "Username must be less or equal to 20 characters, contain alphanumeric characters and underscore only",
                        Title = "Khernet",
                        ShowAcceptOption = true,
                        AcceptOptionLabel = "OK",
                        ShowCancelOption = false,
                    });
                    return;
                }

                IDocumentContainer document = (IDocumentContainer)valueContainer;
                IoCContainer.Get<Messenger>().SaveProfile(new Services.Messages.Peer
                {
                    UserName = User.Username,
                    FullName = document.HasDocument() ? document.GetDocument(Media.MessageType.Html) : null,
                    Group = User.Group,
                    Slogan = User.Slogan,
                });

                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Profile saved successfully",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error while saving profile",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }

        /// <summary>
        /// Loads avatar for a user profile.
        /// </summary>
        /// <param name="avatar">The byte array of image.</param>
        public async void ProcessImage(byte[] avatar)
        {
            if (avatar == null)
                return;

            try
            {
                user.SetAvatarThumbnail(avatar);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error while loading avatar",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }

        private void OpenImageList()
        {
            FileListViewModel imageListModel = new FileListViewModel(messageManager);
            imageListModel.User = user;
            imageListModel.ItemsCount = IoCContainer.Get<Messenger>().GetFileCount(user.Token, Services.Messages.ContentType.Image);

            pagedDialog.CurrentViewModel = imageListModel;
            pagedDialog.CurrentPage = ApplicationPage.ImageList;
            pagedDialog.Category = "Images";
        }

        private void OpenFileList()
        {
            FileListViewModel fileListModel = new FileListViewModel(messageManager);
            fileListModel.User = user;
            fileListModel.ItemsCount = IoCContainer.Get<Messenger>().GetFileCount(user.Token, Services.Messages.ContentType.Binary);


            pagedDialog.CurrentViewModel = fileListModel;
            pagedDialog.CurrentPage = ApplicationPage.FileList;
            pagedDialog.Category = "Files";
        }

        private void OpenVideoList()
        {
            FileListViewModel videoListModel = new FileListViewModel(messageManager);
            videoListModel.User = user;
            videoListModel.ItemsCount = IoCContainer.Get<Messenger>().GetFileCount(user.Token, Services.Messages.ContentType.Video);


            pagedDialog.CurrentViewModel = videoListModel;
            pagedDialog.CurrentPage = ApplicationPage.VideoList;
            pagedDialog.Category = "Videos";
        }

        private void OpenAudioList()
        {
            FileListViewModel audioListModel = new FileListViewModel(messageManager);
            audioListModel.User = user;
            audioListModel.ItemsCount = IoCContainer.Get<Messenger>().GetFileCount(user.Token, Services.Messages.ContentType.Audio);


            pagedDialog.CurrentViewModel = audioListModel;
            pagedDialog.CurrentPage = ApplicationPage.AudioList;
            pagedDialog.Category = "Audios";
        }

        /// <summary>
        /// Shows profile in a new dialog.
        /// </summary>
        public async void ShowProfile()
        {
            pagedDialog = new PagedDialogViewModel();

            //Set list of settings as first page
            pagedDialog.CurrentPage = ApplicationPage.ProfileViewer;

            //Title for page
            pagedDialog.Category = "Profile Information";

            // Only request details if peer is not the current logged user
            if (!User.IsSelfUser)
            {
                //Request to update user profile
                IoCContainer.Get<UserManager>().ProcessState(new UserState
                {
                    Token = User.Token,
                    Change = UserChangeType.ProfileChange,
                });

                //Request to update user avatar
                IoCContainer.Get<UserManager>().ProcessState(new UserState
                {
                    Token = User.Token,
                    Change = UserChangeType.AvatarChange,
                });
            }


            //Set the view model for settings list
            pagedDialog.CurrentViewModel = this;


            pagedDialog.SetHomePage(pagedDialog.CurrentPage, pagedDialog.Category, this);

            await IoCContainer.Get<IUIManager>().ShowDialog(pagedDialog);
        }
    }
}
