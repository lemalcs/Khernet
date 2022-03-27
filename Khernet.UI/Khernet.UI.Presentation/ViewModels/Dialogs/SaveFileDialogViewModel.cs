using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Khernet.UI
{
    public enum SaveFileResult
    {
        /// <summary>
        /// File is saved successfully.
        /// </summary>
        Saved = 0,

        /// <summary>
        /// File saving was canceled.
        /// </summary>
        Canceled = 1,

        /// <summary>
        /// Saving operation failed.
        /// </summary>
        Failed = 2,

        /// <summary>
        /// Operation is not started yet.
        /// </summary>
        NoStarted = 3,
    }

    /// <summary>
    /// View model to show a progress dialog when saving a file to local file system.
    /// </summary>
    public class SaveFileDialogViewModel : BaseModel
    {
        #region Properties

        /// <summary>
        /// The result of saving file.
        /// </summary>
        private SaveFileResult result;

        /// <summary>
        /// The current written bytes.
        /// </summary>
        private long currentWrittenBytes;

        /// <summary>
        /// The file message that owns the file to save.
        /// </summary>
        private FileMessageItemViewModel fileChatMessage;

        /// <summary>
        /// Indicates whether a cancel request was raised.
        /// </summary>
        private volatile bool cancelSaving;

        /// <summary>
        /// The path of saved file.
        /// </summary>
        private string savedFilePath;

        /// <summary>
        /// The description of result.
        /// </summary>
        private string errorDescription;

        /// <summary>
        /// Indicates a file is being written.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// Blocks the caller until save operation is complete.
        /// Results can be:
        /// - True: file saved successfully.
        /// - False: error while saving file, check <see cref="ErrorDescription"/> for details.
        /// </summary>
        TaskCompletionSource<bool> saveResult;

        /// <summary>
        /// The new name of file that will be saved.
        /// </summary>
        private string newFileName;

        public FileMessageItemViewModel FileChatMessage
        {
            get => fileChatMessage;
            set
            {
                if (fileChatMessage != value)
                {
                    fileChatMessage = value;
                    OnPropertyChanged(nameof(fileChatMessage));
                }
            }
        }

        public long CurrentWrittenBytes
        {
            get => currentWrittenBytes;
            set
            {
                if (currentWrittenBytes != value)
                {
                    currentWrittenBytes = value;
                    OnPropertyChanged(nameof(CurrentWrittenBytes));
                }
            }
        }

        public string ErrorDescription
        {
            get => errorDescription;
            set
            {
                if (errorDescription != value)
                {
                    errorDescription = value;
                    OnPropertyChanged(nameof(ErrorDescription));
                }
            }
        }

        public SaveFileResult Result
        {
            get => result;
            set
            {
                if (result != value)
                {
                    result = value;
                    OnPropertyChanged(nameof(Result));
                }
            }
        }
        public bool IsRunning
        {
            get => isRunning;
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    OnPropertyChanged(nameof(IsRunning));
                }
            }
        }
        public string NewFileName
        {
            get => newFileName;
            set
            {
                if (newFileName != value)
                {
                    newFileName = value;
                    OnPropertyChanged(nameof(NewFileName));
                }
            }
        }

        #endregion


        #region Commands

        /// <summary>
        /// Opens the issue page on browser.
        /// </summary>
        public ICommand OpenFolderCommand { get; private set; }
        public ICommand CancelSavingCommand { get; private set; }

        #endregion

        public SaveFileDialogViewModel()
        {
            OpenFolderCommand = new RelayCommand(OpenFolder);
            CancelSavingCommand = new RelayCommand(CancelSaving);
            Result = SaveFileResult.NoStarted;
            saveResult = new TaskCompletionSource<bool>();
        }

        private void OpenFolder()
        {
            IoCContainer.UI.OpenFolderForFile(savedFilePath);
        }

        /// <summary>
        /// Runs the save file process.
        /// </summary>
        /// <param name="destinationPath">The path where to save file to.</param>
        /// <returns>A <see cref="Task"/> instance.</returns>
        public Task<bool> Execute(string destinationPath)
        {

            var t = Task.Run(() =>
            {
                IsRunning = true;
                if (destinationPath != null)
                {
                    try
                    {
                        NewFileName = Path.GetFileName(destinationPath);
                        using (Stream dtStream = IoCContainer.Get<Messenger>().DownloadLocalFile(fileChatMessage.Id))
                        {
                            int chunk = 1048576;
                            byte[] buffer = new byte[chunk];

                            int readBytes = 0;
                            using (FileStream fStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                            {
                                readBytes = dtStream.Read(buffer, 0, chunk);

                                CurrentWrittenBytes = readBytes;

                                while (readBytes > 0)
                                {
                                    if (cancelSaving)
                                        break;

                                    fStream.Write(buffer, 0, readBytes);

                                    readBytes = dtStream.Read(buffer, 0, chunk);
                                    CurrentWrittenBytes += readBytes;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!cancelSaving)
                        {
                            ErrorDescription = "File is corrupted";
                            Result = SaveFileResult.Failed;
                        };
                        LogDumper.WriteLog(ex);
                    }
                }

                IsRunning = false;
                if (cancelSaving)
                {
                    File.Delete(destinationPath);
                    ErrorDescription = "Save file was canceled";
                    Result = SaveFileResult.Canceled;
                    saveResult.TrySetResult(false);
                }
                else if (string.IsNullOrEmpty(ErrorDescription))
                {
                    savedFilePath = destinationPath;
                    ErrorDescription = "File saved successfully";
                    Result = SaveFileResult.Saved;
                    saveResult.TrySetResult(true);
                }
            });

            return saveResult.Task;

        }

        /// <summary>
        /// Stops saving file.
        /// </summary>
        private void CancelSaving()
        {
            cancelSaving = true;
        }

    }
}
