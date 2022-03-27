using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.Files;
using Khernet.UI.IoC;
using System;
using System.IO;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// The state of actual file for file messages.
    /// </summary>
    public enum FileChatState
    {
        /// <summary>
        /// The actual file is ready to use.
        /// </summary>
        Ready,

        /// <summary>
        /// The actual file is damaged or incomplete in local database.
        /// </summary>
        Damaged,

        /// <summary>
        /// The file needs to be downloaded to cache before use it.
        /// </summary>
        NotDownloaded
    }

    public abstract class FileMessageItemViewModel : ChatMessageItemViewModel
    {
        #region Properties

        /// <summary>
        /// The name of file without directory path.
        /// </summary>
        private string fileName;

        /// <summary>
        /// The size of image file.
        /// </summary>
        private long fileSize;

        /// <summary>
        /// Indicates whether a metadata reading operations is running.
        /// </summary>
        private bool isLoading;

        /// <summary>
        /// Indicates if file is being read.
        /// </summary>
        private bool isReadingFile;

        /// <summary>
        /// The current read bytes of image file.
        /// </summary>
        private long currentReadBytes;

        /// <summary>
        /// Indicates that file is ready for use.
        /// </summary>
        private bool isFileLoaded;

        /// <summary>
        /// The id of message that contains the file when resending that message.
        /// </summary>
        public int ResendFileId { get; set; }

        /// <summary>
        /// The path of file on local system.
        /// </summary>
        private string filePath;

        /// <summary>
        /// The state of actual file.
        /// </summary>
        private FileChatState fileState;

        private IApplicationDialog applicationDialog;

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

        public long FileSize
        {
            get
            {
                return fileSize;
            }

            set
            {
                if (fileSize != value)
                {
                    fileSize = value;
                    OnPropertyChanged(nameof(FileSize));
                }
            }
        }

        public bool IsFileLoaded
        {
            get { return isFileLoaded; }
            set
            {
                if (isFileLoaded != value)
                {
                    isFileLoaded = value;
                    OnPropertyChanged(nameof(IsFileLoaded));
                }
            }
        }

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

        public bool IsReadingFile
        {
            get => isReadingFile;
            set
            {
                if (isReadingFile != value)
                {
                    isReadingFile = value;
                    OnPropertyChanged(nameof(IsReadingFile));
                }
            }
        }

        public long CurrentReadBytes
        {
            get => currentReadBytes;
            set
            {
                if (currentReadBytes != value)
                {
                    currentReadBytes = value;
                    OnPropertyChanged(nameof(CurrentReadBytes));
                }
            }
        }

        public string FilePath
        {
            get => filePath;
            set
            {
                if (filePath != value)
                {
                    filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }

        public FileChatState FileState
        {
            get => fileState;
            set
            {
                if (fileState != value)
                {
                    fileState = value;
                    OnPropertyChanged(nameof(FileState));
                }
            }
        }

        #endregion

        #region Commands

        public ICommand SaveCommand { get; protected set; }
        public ICommand ShowInFolderCommand { get; protected set; }

        #endregion

        public FileMessageItemViewModel(IApplicationDialog applicationDialog)
        {
            SaveCommand = new RelayCommand(SaveFile, IsReadyFile);
            ShowInFolderCommand = new RelayCommand(ShowInFolder, IsReadyFile);

            this.applicationDialog = applicationDialog;
        }

        private void ShowInFolder()
        {
            FileOperations operations = new FileOperations();
            if (operations.VerifyFileIntegrity(FilePath, FileSize, Id))
            {
                FileState = FileChatState.Ready;

                //Open file with default external program
                IoCContainer.UI.OpenFolderForFile(FilePath);
            }
            else if (IsFileLoaded && FileState == FileChatState.Ready)
            {
                //Request to download to cache
                FileState = FileChatState.NotDownloaded;
                FilePath = string.Empty;
            }
        }

        /// <summary>
        /// Save the file of this message to local file system.
        /// </summary>
        /// <param name="obj"></param>
        protected async virtual void SaveFile()
        {
            try
            {
                string newFileName = applicationDialog.ShowSaveFileDialog(Path.GetFileName(FileName));

                if (newFileName == null)
                    return;

                SaveFileDialogViewModel saveProgressDialog = new SaveFileDialogViewModel
                {
                    FileChatMessage = this,
                };
                _ = saveProgressDialog.Execute(newFileName);
                await applicationDialog.ShowDialog(saveProgressDialog);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await applicationDialog.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error while saving file.",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }

        /// <summary>
        /// Save the file of this message to local system with a given destination path.
        /// </summary>
        /// <param name="fileName">The path where to save file to.</param>
        public async void SaveFile(string fileName, bool showProgress = true)
        {
            try
            {
                if (fileName == null)
                    return;

                SaveFileDialogViewModel saveProgressDialog = new SaveFileDialogViewModel
                {
                    FileChatMessage = this,
                };

                if (showProgress)
                {
                    _ = saveProgressDialog.Execute(fileName);
                    await applicationDialog.ShowDialog(saveProgressDialog);
                }
                else
                    await saveProgressDialog.Execute(fileName);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await applicationDialog.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error while saving file.",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }

        /// <summary>
        /// Saves a file without showing a progress dialog.
        /// </summary>
        /// <param name="fileName">The path of destination file.</param>
        /// <returns>True if file was saved successfully otherwise false.</returns>
        public bool SaveFileWithShowProgress(string fileName)
        {
            try
            {
                if (fileName == null)
                    return false;

                SaveFileDialogViewModel saveProgressDialog = new SaveFileDialogViewModel
                {
                    FileChatMessage = this,
                };
                return saveProgressDialog.Execute(fileName).Result;
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                return false;
            }
        }

        /// <summary>
        /// Checks if file is saved on database and ready to be read.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if file is ready to use otherwise false.</returns>
        private bool IsReadyFile()
        {
            return IsMessageLoaded;
        }

        public override void Load(MessageItem messageItem)
        {
            if (messageItem == null)
                throw new NullReferenceException($"Parameter {nameof(messageItem)} cannot be null.");

            Id = messageItem.Id;
            UID = messageItem.UID;
            TimeId = messageItem.TimeId;
            SendDate = messageItem.RegisterDate;
            IsSentByMe = messageItem.IdSenderPeer == 0;
            IsRead = messageItem.IsRead;
            State = (ChatMessageState)(int)messageItem.State;
        }

        public abstract void Send(string filePath);
    }
}
