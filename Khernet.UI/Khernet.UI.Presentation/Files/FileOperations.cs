using Khernet.Core.Entity;
using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.Cache;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.IO;
using System.Windows;

namespace Khernet.UI.Files
{
    public class FileOperations
    {
        public void ProcessImage(IFileObserver observer)
        {
            Stream dtStream = null;
            ChatMessageState state = ChatMessageState.Pendding;
            int idMessage = 0;
            try
            {
                if (observer.Media.OperationRequest == MessageOperation.Upload)
                {
                    //Stores the file name located in cache directory
                    string outFile = null;

                    string fileName = null;

                    //Check if image comes from a stream
                    if (string.IsNullOrEmpty(observer.Media.FileName))
                    {
                        string imageFile = Guid.NewGuid().ToString();
                        outFile = Path.Combine(Configurations.CacheDirectory.FullName, imageFile);
                        ImageHelper.SaveImageStream(observer.Media.FileData, outFile);

                        fileName = imageFile;
                    }
                    //Image comes from a file
                    else
                    {
                        fileName = Path.GetFileName(observer.Media.FileName);

                        //Get file name that will be copied to cache directory
                        outFile = Path.Combine(Configurations.CacheDirectory.FullName, fileName);

                        //Copy file to cache directory
                        File.Copy(observer.Media.FileName, outFile, true);
                    }

                    FileResponse metadata = new FileResponse();
                    metadata.FilePath = outFile;
                    metadata.OriginalFileName = fileName;
                    metadata.Operation = MessageOperation.Upload;
                    metadata.Size = FileHelper.GetFileSize(outFile);

                    using (MemoryStream mem = ImageHelper.GetImageThumbnail(outFile))
                    {
                        metadata.ThumbnailBytes = mem.ToArray();
                    }

                    MessageProcessResult result = UploadFile(observer, metadata);
                    state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;
                    idMessage = result.Id;
                }
                else if (observer.Media.OperationRequest == MessageOperation.GetMetadata)
                {
                    idMessage = observer.Media.Id;

                    FileResponse response = GetFileMetadata(observer);
                    response.Operation = MessageOperation.GetMetadata;

                    state = response.State;

                    observer.OnGetMetadata(response);
                }
                else if (observer.Media.OperationRequest == MessageOperation.Download)
                {
                    idMessage = observer.Media.Id;

                    FileResponse response = new FileResponse
                    {
                        FilePath = GetCacheFile(observer)
                    };
                    observer.OnGetMetadata(response);
                }
                else if (observer.Media.OperationRequest == MessageOperation.Resend)
                {
                    FileResponse response = GetLocalFile(observer);
                    response.Operation = MessageOperation.Download;

                    MessageProcessResult result = UploadFile(observer, response);
                    state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;
                    idMessage = result.Id;

                    observer.OnGetMetadata(response);
                }
            }
            catch (Exception error)
            {
                state = ChatMessageState.Error;

                //Notify about an error to the UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    observer.OnError(error);
                }));

            }
            finally
            {
                if (dtStream != null)
                {
                    dtStream.Close();
                }

                ChatMessageProcessResult result = new ChatMessageProcessResult(idMessage, state);
                observer.OnCompleted(result);
            }
        }

        /// <summary>
        /// Send or receives GIF files
        /// </summary>
        /// <param name="observer">The observer to notify about performed operations</param>
        public async void ProcessGIF(IFileObserver observer)
        {
            Stream dtStream = null;

            string aviExtension = ".avi";
            ChatMessageState state = ChatMessageState.Pendding;
            int idMessage = 0;

            try
            {
                if (observer.Media.OperationRequest == MessageOperation.Upload)
                {
                    //Get new file name for AVI format
                    string outFile = Path.Combine(Configurations.CacheDirectory.FullName, Path.GetFileNameWithoutExtension(observer.Media.FileName) + aviExtension);

                    if (File.Exists(outFile))
                    {
                        outFile = FileHelper.GetNewFileName(outFile);
                    }

                    //Convert video format from GIF to AVI format
                    MediaHelper.ConvertTo(observer.Media.FileName, outFile, MediaFormat.AVI);

                    //Get media metadata
                    FileResponse metadata = new FileResponse();
                    metadata.FilePath = outFile;
                    metadata.OriginalFileName = Path.GetFileName(outFile);

                    string thumbnailFile = Path.Combine(Configurations.CacheDirectory.FullName, Path.GetFileNameWithoutExtension(observer.Media.FileName) + ".jpg");

                    if (File.Exists(thumbnailFile))
                    {
                        thumbnailFile = FileHelper.GetNewFileName(thumbnailFile);
                    }

                    MediaHelper.GetVideoThumbnail(observer.Media.FileName, thumbnailFile, TimeSpan.Zero);
                    metadata.Thumbnail = thumbnailFile;

                    using (MemoryStream mem = ImageHelper.GetImageThumbnail(thumbnailFile))
                    {
                        metadata.ThumbnailBytes = mem.ToArray();
                    }

                    Size videoSize = await MediaHelper.GetVideoSize(observer.Media.FileName);
                    metadata.Width = videoSize.Width;
                    metadata.Height = videoSize.Height;
                    metadata.Size = FileHelper.GetFileSize(outFile);

                    MessageProcessResult result = UploadFile(observer, metadata);
                    state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;
                    idMessage = result.Id;
                }
                else if (observer.Media.OperationRequest == MessageOperation.Download)
                {
                    idMessage = observer.Media.Id;

                    FileResponse response = GetFileMetadata(observer);
                    response.Operation = MessageOperation.Download;

                    state = response.State;

                    observer.OnGetMetadata(response);

                    if (response.State == ChatMessageState.Pendding || response.State == ChatMessageState.Processed)
                    {
                        response.FilePath = GetCacheFile(observer);
                        observer.OnGetMetadata(response);
                    }
                }
                else if (observer.Media.OperationRequest == MessageOperation.Resend)
                {
                    observer.Media.FileName = Path.GetFileNameWithoutExtension(observer.Media.FileName) + aviExtension;

                    FileResponse response = GetLocalFile(observer);
                    response.Operation = MessageOperation.Resend;

                    MessageProcessResult result = UploadFile(observer, response);
                    state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;
                    idMessage = result.Id;

                    observer.OnGetMetadata(response);
                }
            }
            catch (Exception error)
            {
                state = ChatMessageState.Error;

                //Notify about an error to the UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    observer.OnError(error);
                }));

            }
            finally
            {
                if (dtStream != null)
                {
                    dtStream.Close();
                }

                ChatMessageProcessResult result = new ChatMessageProcessResult(idMessage, state);
                observer.OnCompleted(result);
            }
        }

        /// <summary>
        /// Send or receives video files
        /// </summary>
        /// <param name="observer">The observer who listens to notifications about performed operations</param>
        public async void ProcessVideo(IFileObserver observer)
        {
            Stream dtStream = null;
            ChatMessageState state = ChatMessageState.Pendding;
            int idMessage = 0;

            try
            {
                if (observer.Media.OperationRequest == MessageOperation.Upload)
                {
                    //Get duration of video
                    TimeSpan duration = await MediaHelper.GetVideoDuration(observer.Media.FileName);

                    //Get media metadata
                    FileResponse metadata = new FileResponse();
                    metadata.Duration = duration;

                    //If duration of video is greater than five seconds get an thumbnail of second five.
                    TimeSpan startTime = TimeSpan.Zero;
                    if (duration > TimeSpan.FromSeconds(5))
                        startTime = TimeSpan.FromSeconds(5);

                    //Verify if file has video tracks
                    bool hasVideoTracks = await MediaHelper.HasVideo(observer.Media.FileName);

                    if (hasVideoTracks)
                    {
                        //Build thmubnail file 
                        string thumbnailFile = Path.Combine(Configurations.CacheDirectory.FullName, Path.GetFileNameWithoutExtension(observer.Media.FileName) + ".jpg");

                        if (File.Exists(thumbnailFile))
                        {
                            thumbnailFile = FileHelper.GetNewFileName(thumbnailFile);
                        }

                        MediaHelper.GetVideoThumbnail(observer.Media.FileName, thumbnailFile, startTime);
                        metadata.Thumbnail = thumbnailFile;

                        using (MemoryStream mem = ImageHelper.GetImageThumbnail(thumbnailFile))
                        {
                            metadata.ThumbnailBytes = mem.ToArray();
                        }

                        Size videoSize = await MediaHelper.GetVideoSize(observer.Media.FileName);
                        metadata.Width = videoSize.Width;
                        metadata.Height = videoSize.Height;
                    }


                    //Get the orignal name of video file
                    metadata.OriginalFileName = Path.GetFileName(observer.Media.FileName);
                    metadata.FilePath = observer.Media.FileName;

                    //Get total number of bytes of file
                    metadata.Size = FileHelper.GetFileSize(observer.Media.FileName);

                    MessageProcessResult result = UploadFile(observer, metadata);
                    state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;
                    idMessage = result.Id;
                }
                else if (observer.Media.OperationRequest == MessageOperation.GetMetadata)
                {
                    idMessage = observer.Media.Id;

                    FileResponse response = GetFileMetadata(observer);
                    response.Operation = MessageOperation.GetMetadata;

                    state = response.State;

                    observer.OnGetMetadata(response);
                }
                else if (observer.Media.OperationRequest == MessageOperation.Download)
                {
                    idMessage = observer.Media.Id;

                    FileResponse response = new FileResponse
                    {
                        FilePath = GetCacheFile(observer)
                    };

                    observer.OnGetMetadata(response);
                }
                else if (observer.Media.OperationRequest == MessageOperation.Resend)
                {
                    FileResponse response = GetLocalFile(observer);
                    response.Operation = MessageOperation.Download;

                    MessageProcessResult result = UploadFile(observer, response);
                    state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;
                    idMessage = result.Id;

                    observer.OnGetMetadata(response);
                }
            }
            catch (Exception error)
            {
                state = ChatMessageState.Error;

                //Notify about an error to the UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    observer.OnError(error);
                }));
            }
            finally
            {
                if (dtStream != null)
                {
                    dtStream.Close();
                }

                ChatMessageProcessResult result = new ChatMessageProcessResult(idMessage, state);
                observer.OnCompleted(result);
            }
        }

        /// <summary>
        /// Sends o receives audio files
        /// </summary>
        /// <param name="observer">The observer who listens to notifications about performed operations</param>
        public async void ProcessAudio(IFileObserver observer)
        {
            Stream dtStream = null;
            ChatMessageState state = ChatMessageState.Pendding;
            int idMessage = 0;

            try
            {
                if (observer.Media.OperationRequest == MessageOperation.Upload)
                {
                    //Get duration of audio
                    TimeSpan duration = await MediaHelper.GetVideoDuration(observer.Media.FileName);

                    //Get media metadata
                    FileResponse metadata = new FileResponse();
                    metadata.Duration = duration;

                    //Get original path of video file
                    metadata.OriginalFileName = Path.GetFileName(observer.Media.FileName);
                    metadata.FilePath = observer.Media.FileName;
                    metadata.Size = FileHelper.GetFileSize(observer.Media.FileName);

                    metadata.Operation = MessageOperation.GetMetadata;
                    //observer.OnGetMetadata(metadata);

                    MessageProcessResult result = UploadFile(observer, metadata);


                    state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;
                    idMessage = result.Id;
                }
                else if (observer.Media.OperationRequest == MessageOperation.GetMetadata)
                {
                    idMessage = observer.Media.Id;

                    FileResponse response = GetFileMetadata(observer);
                    response.Operation = MessageOperation.GetMetadata;

                    state = response.State;

                    observer.OnGetMetadata(response);
                }
                else if (observer.Media.OperationRequest == MessageOperation.Download)
                {
                    idMessage = observer.Media.Id;

                    FileResponse response = new FileResponse
                    {
                        FilePath = GetCacheFile(observer)
                    };
                    observer.OnGetMetadata(response);
                }
                else if (observer.Media.OperationRequest == MessageOperation.Resend)
                {
                    FileResponse response = GetLocalFile(observer);
                    response.Operation = MessageOperation.Download;

                    MessageProcessResult result = UploadFile(observer, response);
                    state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;
                    idMessage = result.Id;

                    observer.OnGetMetadata(response);
                }
            }
            catch (Exception error)
            {
                state = ChatMessageState.Error;

                //Notify about an error to the UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    observer.OnError(error);
                }));
            }
            finally
            {
                if (dtStream != null)
                {
                    dtStream.Close();
                }

                ChatMessageProcessResult result = new ChatMessageProcessResult(idMessage, state);
                observer.OnCompleted(result);
            }
        }

        /// <summary>
        /// Sends and receives any type of files
        /// </summary>
        /// <param name="observer">The observer who listens to notifications about performed operations</param>
        public void ProcessFile(IFileObserver observer)
        {
            Stream dtStream = null;
            ChatMessageState state = ChatMessageState.Pendding;
            int idMessage = 0;
            try
            {
                if (observer.Media.OperationRequest == MessageOperation.Upload)
                {
                    FileResponse metadata = new FileResponse();

                    //Set new file name
                    metadata.OriginalFileName = Path.GetFileName(observer.Media.FileName);
                    metadata.FilePath = observer.Media.FileName;

                    //Get file size in bytes
                    metadata.Size = FileHelper.GetFileSize(observer.Media.FileName);

                    MessageProcessResult result = UploadFile(observer, metadata);
                    state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;
                    idMessage = result.Id;
                }
                else if (observer.Media.OperationRequest == MessageOperation.GetMetadata)
                {
                    idMessage = observer.Media.Id;

                    FileResponse response = GetFileMetadata(observer);
                    response.Operation = MessageOperation.GetMetadata;

                    state = response.State;

                    observer.OnGetMetadata(response);
                }
                else if (observer.Media.OperationRequest == MessageOperation.Download)
                {
                    idMessage = observer.Media.Id;

                    FileResponse response =new FileResponse 
                    {
                       FilePath = GetCacheFile(observer)
                    };
                    observer.OnGetMetadata(response);
                }
                else if (observer.Media.OperationRequest == MessageOperation.Resend)
                {
                    FileResponse response = GetLocalFile(observer);
                    response.Operation = MessageOperation.Download;

                    MessageProcessResult result = UploadFile(observer, response);
                    state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;
                    idMessage = result.Id;

                    observer.OnGetMetadata(response);
                }
            }
            catch (Exception error)
            {
                state = ChatMessageState.Error;

                //Notify about an error to the UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    observer.OnError(error);
                }));
            }
            finally
            {
                if (dtStream != null)
                {
                    dtStream.Close();
                }

                ChatMessageProcessResult result = new ChatMessageProcessResult(idMessage, state);
                observer.OnCompleted(result);
            }
        }

        /// <summary>
        /// Uploads a file to destination peer.
        /// </summary>
        /// <param name="observer">The sender that will be notified about operations performed over file</param>
        /// <param name="response">Holds information about metadata of file</param>
        /// <param name="fileName">The path of file to send</param>
        /// <returns>The id of file</returns>
        private MessageProcessResult UploadFile(IFileObserver observer, FileResponse response)
        {
            response.UID = observer.Media.UID;

            //Notify the sender about metadata of file
            observer.OnGetMetadata(response);

            //Prepare information about file
            FileMessage fileMessage = new FileMessage();
            fileMessage.SenderToken = observer.Media.SenderToken;
            fileMessage.ReceiptToken = observer.Media.ReceiptToken;
            fileMessage.SendDate = observer.Media.SendDate;

            FileInformation info = new FileInformation
            {
                FileName = response.OriginalFileName,
                Format = Enum.GetName(typeof(MessageType), observer.Media.FileType),
                Duration = response.Duration,
                Width = response.Width,
                Height = response.Height,
                Size = response.Size,
            };
            fileMessage.Metadata = info;
            fileMessage.Type = (ContentType)((int)observer.Media.FileType);
            fileMessage.UID = observer.Media.UID;

            //Create the observer that will notify progress about uploading
            FileObserver fileObserver = new FileObserver(fileMessage);
            fileObserver.PhysicalFileName = response.FilePath;
            fileObserver.Direction = FileDirection.Send;
            fileObserver.Thumbnail = response.ThumbnailBytes;

            fileObserver.ProgressChanged += (objSender, args) =>
            {
                observer.OnProcessing(args.ReadBytes);
            };

            return IoCContainer.Get<Messenger>().SendFile(fileObserver);
        }

        /// <summary>
        /// Get the metadata of requested file.
        /// </summary>
        /// <param name="observer">The observer that will be notified about operations performed over file</param>
        /// <returns>A <see cref="FileResponse"/> object with metadata</returns>
        private FileResponse GetFileMetadata(IFileObserver observer)
        {
            //Get file information
            byte[] fileInfo = IoCContainer.Get<Messenger>().GetMessageContent(observer.Media.Id);

            FileInformation info = JSONSerializer<FileInformation>.DeSerialize(fileInfo);

            ConversationMessage message = (ConversationMessage)IoCContainer.Get<Messenger>().GetMessageDetail(observer.Media.Id);

            //Set processed bytes to zero before save file
            observer.OnProcessing(0);

            FileResponse response = new FileResponse();

            response.OriginalFileName = info.FileName;
            response.Duration = info.Duration;
            response.Width = info.Width;
            response.Height = info.Height;
            response.Size = info.Size;
            response.State = (ChatMessageState)((int)message.State);
            response.ThumbnailBytes = IoCContainer.Get<Messenger>().GetThumbnail(observer.Media.Id);
            response.UID = message.UID;
            response.FilePath= IoCContainer.Get<Messenger>().GetCacheFilePath(observer.Media.Id);

            return response;
        }

        /// <summary>
        /// Get the file from application local storage which is saved in cache folder.
        /// </summary>
        /// <param name="observer">The observer that will be notified about operations performed over file</param>
        /// <returns>The path of physical file</returns>
        private string GetCacheFile(IFileObserver observer)
        {
            ConversationMessage message = (ConversationMessage)IoCContainer.Get<Messenger>().GetMessageDetail(observer.Media.Id);

            //Check if file is stored correctly in local database otherwise do not download file
            if (message.State == MessageState.Error || message.State == MessageState.UnCommited)
                return null;

            //Get file information
            byte[] fileInfo = IoCContainer.Get<Messenger>().GetMessageContent(observer.Media.Id);

            FileInformation info = JSONSerializer<FileInformation>.DeSerialize(fileInfo);


            FileMessage fileMessage = new FileMessage
            {
                SenderToken = message.SenderToken,
                ReceiptToken = message.ReceiptToken,
                Metadata = info,
                UID = message.UID,
            };

            FileObserver fileObserver = new FileObserver(fileMessage);

            //Build name of file for local file system
            string outFile = IoCContainer.Get<Messenger>().GetCacheFilePath(observer.Media.Id);

            if (string.IsNullOrEmpty(outFile) || !Directory.Exists(outFile))
                outFile = Path.Combine(Configurations.CacheDirectory.FullName, Path.GetFileName(fileObserver.Data.Metadata.FileName));

            //If file name already exists then build a new file name
            if (!VerifyFileIntegrity(outFile, info.Size, observer))
            {
                observer.OnProcessing(0);

                outFile = FileHelper.GetNewFileName(outFile);

                fileObserver.Direction = FileDirection.Save;
                fileObserver.PhysicalFileName = outFile;

                //Save file to local file system
                using (Stream dtStream = IoCContainer.Get<Messenger>().DownloadLocalFile(observer.Media.Id))
                {
                    int chunk = 1048576;
                    byte[] buffer = new byte[chunk];

                    int readBytes = 0;
                    using (FileStream fStream = new FileStream(outFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        readBytes = dtStream.Read(buffer, 0, chunk);

                        int actualReadBytes = readBytes;

                        while (readBytes > 0)
                        {
                            fStream.Write(buffer, 0, readBytes);

                            observer.OnProcessing(actualReadBytes);

                            readBytes = dtStream.Read(buffer, 0, chunk);
                            actualReadBytes += readBytes;
                        }
                    }
                }
            }

            IoCContainer.Get<Messenger>().UpdateCacheFilePath(observer.Media.Id, outFile);

            return outFile;
        }

        /// <summary>
        /// Saves file of database in cache folder
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        private FileResponse GetLocalFile(IFileObserver observer)
        {
            //Build name of file for local file system
            string outFile = Path.Combine(Configurations.CacheDirectory.FullName, Path.GetFileName(observer.Media.FileName));

            //If file name already exists then build a new file name
            if (File.Exists(outFile))
            {
                outFile = FileHelper.GetNewFileName(outFile);
            }

            //Save file to local file system
            using (Stream dtStream = IoCContainer.Get<Messenger>().DownloadLocalFile(observer.Media.Id))
            {
                int chunk = 1048576;
                byte[] buffer = new byte[chunk];

                int readBytes = 0;
                using (FileStream fStream = new FileStream(outFile, FileMode.Create, FileAccess.Write))
                {
                    readBytes = dtStream.Read(buffer, 0, chunk);

                    int actualReadBytes = readBytes;

                    while (readBytes > 0)
                    {
                        fStream.Write(buffer, 0, readBytes);

                        observer.OnProcessing(actualReadBytes);

                        readBytes = dtStream.Read(buffer, 0, chunk);
                        actualReadBytes += readBytes;
                    }
                }
            }

            byte[] thumbnail = IoCContainer.Get<Messenger>().GetThumbnail(observer.Media.Id);

            byte[] fileInfo = IoCContainer.Get<Messenger>().GetMessageContent(observer.Media.Id);

            FileInformation info = JSONSerializer<FileInformation>.DeSerialize(fileInfo);

            FileResponse response = new FileResponse();
            response.FilePath = outFile;
            response.ThumbnailBytes = thumbnail;
            response.OriginalFileName = info.FileName;
            response.Width = info.Width;
            response.Height = info.Height;
            response.Duration = info.Duration;
            response.Size = info.Size;
            response.UID = observer.Media.UID;

            return response;
        }

        /// <summary>
        /// Checks if the file located in cache is the same file that is stored on database
        /// </summary>
        /// <param name="cacheFile">The path of cache file</param>
        /// <param name="originalFileSize">The size of original file</param>
        /// <param name="idMessage">The id of message thats owns the file</param>
        /// <returns>True if both file in database and file in cache are equal, otherwise false</returns>
        public bool VerifyFileIntegrity(string cacheFile, long originalFileSize, int idMessage)
        {
            try
            {
                if (File.Exists(cacheFile))
                {
                    FileInfo fileDetail = new FileInfo(cacheFile);
                    if (originalFileSize == fileDetail.Length)
                    {
                        using (Stream dtStream = IoCContainer.Get<Messenger>().DownloadLocalFile(idMessage))
                        {
                            int chunk = 1048576;
                            byte[] buffer = new byte[chunk];

                            int readBytesOriginalFile = 0;
                            using (FileStream fStream = new FileStream(cacheFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                readBytesOriginalFile = dtStream.Read(buffer, 0, chunk);

                                byte[] cacheBuffer = new byte[buffer.Length];

                                int readBytesCacheFile = fStream.Read(cacheBuffer, 0, cacheBuffer.Length);

                                while (readBytesOriginalFile > 0 && readBytesCacheFile > 0)
                                {
                                    if (FileHelper.AreEqualArray(buffer, cacheBuffer))
                                    {
                                        readBytesOriginalFile = dtStream.Read(buffer, 0, chunk);
                                        readBytesCacheFile = fStream.Read(cacheBuffer, 0, cacheBuffer.Length);
                                    }
                                    else
                                        return false;
                                }
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Checks if the file located in cache is the same file that is stored on database
        /// </summary>
        /// <param name="cacheFile">The path of cache file</param>
        /// <param name="originalFileSize">The size of original file</param>
        /// <param name="observer">The observer to be notified about progress</param>
        /// <returns>True if both file in database and file in cache are equal, otherwise false</returns>
        public bool VerifyFileIntegrity(string cacheFile, long originalFileSize, IFileObserver observer)
        {
            try
            {
                observer.OnProcessing(0);
                if (File.Exists(cacheFile))
                {
                    FileInfo fileDetail = new FileInfo(cacheFile);
                    if (originalFileSize == fileDetail.Length)
                    {
                        using (Stream dtStream = IoCContainer.Get<Messenger>().DownloadLocalFile(observer.Media.Id))
                        {
                            int chunk = 1048576;
                            byte[] buffer = new byte[chunk];

                            int readBytesOriginalFile = 0;
                            using (FileStream fStream = new FileStream(cacheFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                readBytesOriginalFile = dtStream.Read(buffer, 0, chunk);

                                byte[] cacheBuffer = new byte[buffer.Length];

                                int readBytesCacheFile = fStream.Read(cacheBuffer, 0, cacheBuffer.Length);

                                int actualReadBytes = readBytesCacheFile;

                                while (readBytesOriginalFile > 0 && readBytesCacheFile > 0)
                                {
                                    if (FileHelper.AreEqualArray(buffer, cacheBuffer))
                                    {
                                        readBytesOriginalFile = dtStream.Read(buffer, 0, chunk);
                                        readBytesCacheFile = fStream.Read(cacheBuffer, 0, cacheBuffer.Length);
                                        actualReadBytes += readBytesCacheFile;

                                        observer.OnProcessing(actualReadBytes);
                                    }
                                    else
                                        return false;
                                }
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
