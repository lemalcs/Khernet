using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using System;
using System.IO;
using System.Windows.Input;

namespace Khernet.UI
{
    public abstract class FileMessageItemViewModel : ChatMessageItemViewModel
    {
        #region Properties

        /// <summary>
        /// The name of file without directory path
        /// </summary>
        private string fileName;

        /// <summary>
        /// The size of image file
        /// </summary>
        private long fileSize;

        /// <summary>
        /// Indicates whether a metadata reading operations is running
        /// </summary>
        private bool isLoading;

        /// <summary>
        /// Indicates if file is being read
        /// </summary>
        private bool isReadingFile;

        /// <summary>
        /// The current read bytes of image file
        /// </summary>
        private long currentReadBytes;

        /// <summary>
        /// Indicates that file is ready for use
        /// </summary>
        private bool isFileLoaded;

        /// <summary>
        /// The id of message that contains the file when resending that message
        /// </summary>
        public int ResendId { get; set; }

        /// <summary>
        /// The path of file on local system
        /// </summary>
        private string filePath;

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

        #endregion

        public ICommand SaveCommand { get; protected set; }

        public FileMessageItemViewModel()
        {
            SaveCommand = new RelayCommand(SaveFile, IsReadyFile);
        }

        /// <summary>
        /// Save and image to local file system
        /// </summary>
        /// <param name="obj"></param>
        private async void SaveFile(object obj)
        {
            try
            {
                string newFileName = IoCContainer.UI.ShowSaveFileDialog(Path.GetFileName(FileName));

                SaveFile(newFileName);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
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
        /// Save this file message to local system
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveFile(string fileName)
        {
            if (fileName != null)
            {
                byte[] fileInfo = IoCContainer.Get<Messenger>().GetMessageContent(Id);

                FileMessage info = JSONSerializer<FileMessage>.DeSerialize(fileInfo);

                using (Stream dtStream = IoCContainer.Get<Messenger>().DownloadLocalFile(Id))
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
        }

        /// <summary>
        /// Cheack is file is saved on database and ready to be read
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool IsReadyFile(object obj)
        {
            return IsMessageLoaded;//Id != 0;
        }
    }
}
