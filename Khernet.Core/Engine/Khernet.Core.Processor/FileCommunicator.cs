using Khernet.Core.Common;
using Khernet.Core.Data;
using Khernet.Core.Entity;
using Khernet.Core.Processor.IoC;
using Khernet.Core.Processor.Managers;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.ServiceModel;
using System.Text;

namespace Khernet.Core.Processor
{
    public class FileCommunicator
    {
        public FileDownloadResponseMessage GetSelfAvatar()
        {
            try
            {
                FileCommunicatorData fileData = new FileCommunicatorData();

                byte[] data = fileData.GetSelfAvatar();

                if (data == null)
                    data = new byte[0];

                //This may produce memory leaks if number of calls increase
                //because every time a user joins to network, it will request the avatar
                FileDownloadResponseMessage fileResponse = new FileDownloadResponseMessage();
                fileResponse.File = new MemoryStream(data);

                return fileResponse;
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }

        private int? GetIdReply(string uid)
        {
            CommunicatorData commData = new CommunicatorData();

            if (!string.IsNullOrEmpty(uid))
            {
                return commData.GetIdMessage(uid);
            }
            return null;
        }

        public void SaveFile(string userToken, string idFile, string fileName, Stream fileData)
        {
            try
            {
                FileCommunicatorData fileManager = new FileCommunicatorData();

                string tempFile = BuildIdFile(userToken, idFile);

                fileManager.SaveFile(tempFile, fileName, fileData);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }

        public void UploadFile(FileUploadRequestMessage file)
        {
            try
            {
                CommunicatorData commData = new CommunicatorData();

                byte[] info = JSONSerializer<FileInformation>.Serialize(file.FileMetaData.Metadata);

                int? idReply = GetIdReply(file.FileMetaData.UIDReply);


                byte[] thumbnail = null;

                using (DataStream dt = new DataStream(file.File))
                using (MemoryStream fileStream = new MemoryStream())
                {
                    byte[] buffer = new byte[4096];

                    int readBytes = dt.Read(buffer, 0, buffer.Length);

                    while (readBytes > 0)
                    {
                        fileStream.Write(buffer, 0, readBytes);
                        readBytes = dt.Read(buffer, 0, buffer.Length);
                    }

                    buffer = fileStream.ToArray();

                    if (buffer.Length > 0)
                        thumbnail = fileStream.ToArray();
                }

                //Save message of new file to application database
                int idMessage = commData.SaveTextMessage(
                    file.FileMetaData.SenderToken,
                    file.FileMetaData.ReceiptToken,
                    file.FileMetaData.SendDate,
                    info,
                    (int)file.FileMetaData.Type,
                    file.FileMetaData.UID,
                    idReply,
                    thumbnail);


                IoCContainer.Get<FileManager>().ProcessFile(idMessage);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void SaveFileWithoutProgress(string idFile, string fileName, Stream fileData)
        {
            try
            {
                FileCommunicatorData fileManager = new FileCommunicatorData();
                fileManager.SaveFileWithoutProgress(idFile, fileName, fileData);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }

        public MessageProcessResult SendFile(FileObserver fileObserver)
        {
            CommunicatorData commData = new CommunicatorData();
            string tempFile = null;
            int idMessage = -1;

            try
            {
                FileInformation fileMessage = null;
                Communicator comm = new Communicator();

                //Get file stream
                using (DataStream dt = new DataStream(fileObserver.GetFileStream()))
                {
                    fileMessage = new FileInformation
                    {
                        FileName = fileObserver.Data.Metadata.FileName,
                        Size = dt.Length,
                        Width = fileObserver.Data.Metadata.Width,
                        Height = fileObserver.Data.Metadata.Height,
                        Duration = fileObserver.Data.Metadata.Duration,
                        Format = fileObserver.Data.Metadata.Format,
                    };

                    //Serialize metadata of file
                    byte[] fileInfo = JSONSerializer<FileInformation>.Serialize(fileMessage);

                    tempFile = BuildIdFile(fileObserver.Data.ReceiptToken, fileObserver.Data.UID);

                    dt.ReadChanged += (args) =>
                    {
                        fileObserver.OnNext(args.ReadBytes);
                    };

                    //Save text message when a file is sent
                    idMessage = commData.SaveTextMessage(
                        fileObserver.Data.SenderToken,
                        fileObserver.Data.ReceiptToken,
                        fileObserver.Data.SendDate,
                        fileInfo,
                        (int)fileObserver.Data.Type,
                        fileObserver.Data.UID,
                        GetIdReply(fileObserver.Data.UIDReply),
                        fileObserver.Thumbnail,
                        fileObserver.PhysicalFileName);

                    //Save file on local application database
                    SaveFileWithoutProgress(tempFile, fileObserver.PhysicalFileName, dt);

                    commData.SetMessageState(idMessage, (int)MessageState.Pendding);
                }
                fileObserver.OnCompleted();

                FileMessage fMessage = new FileMessage
                {
                    SenderToken = fileObserver.Data.SenderToken,
                    ReceiptToken = fileObserver.Data.ReceiptToken,
                    Type = fileObserver.Data.Type,
                    Metadata = fileMessage,
                    SendDate = fileObserver.Data.SendDate,
                    UID = fileObserver.Data.UID,
                    UIDReply = fileObserver.Data.UIDReply,
                };

                FileUploadRequestMessage fileUploadRequest = new FileUploadRequestMessage();
                fileUploadRequest.FileMetaData = fMessage;

                CommunicatorClient fClient = new CommunicatorClient(fileObserver.Data.ReceiptToken);

                using (MemoryStream thumbnailStream = new MemoryStream(fileObserver.Thumbnail ?? new byte[0]))
                using (DataStream dt = new DataStream(thumbnailStream))
                {
                    dt.ReadChanged += (args) =>
                    {
                        fileObserver.OnNext(args.ReadBytes);
                    };

                    fileUploadRequest.File = dt;

                    fClient.UploadFile(fileUploadRequest);
                }

                commData.SetMessageState(idMessage, (int)MessageState.Processed);
                fileObserver.OnCompleted();

                MessageProcessResult result = new MessageProcessResult(idMessage, MessageState.Processed);

                return result;

            }
            catch (Exception error)
            {
                if (idMessage > 0 && !string.IsNullOrEmpty(tempFile))
                {
                    RegisterPenddingMessage(fileObserver.Data.ReceiptToken, idMessage);
                    MessageProcessResult result = new MessageProcessResult(idMessage, MessageState.Pendding);
                    return result;
                }
                else if (tempFile != null)
                {
                    FileCommunicatorData fData = new FileCommunicatorData();
                    fData.DeleteFile(tempFile);
                }
                fileObserver.OnError(error);

                LogDumper.WriteLog(error);
                throw error;
            }
        }

        private void RegisterPenddingMessage(string receiptToken, int idMessage)
        {
            //Save file on database so this can be send later
            CommunicatorData commData = new CommunicatorData();
            commData.RegisterPenddingMessage(receiptToken, idMessage);

            //Try to send file again
            IoCContainer.Get<MessageManager>().ProcessPenddingMessagesOf(receiptToken);
        }

        /// <summary>
        /// Send a pendding file message to receipt
        /// </summary>
        /// <param name="fileMessage">Message to be sent</param>
        public void SendPenddingFile(InternalConversationMessage conversationMessage)
        {
            try
            {
                CommunicatorData commData = new Data.CommunicatorData();

                DataTable content = commData.GetMessageContent(conversationMessage.Id);

                FileInformation info = null;

                if (content.Rows.Count > 0)
                    info = JSONSerializer<FileInformation>.DeSerialize(content.Rows[0][0] as byte[]);

                if(info.Size>GetFileSize(conversationMessage.Id))
                {
                    throw new Exception("Invalid file size");
                }

                FileMessage fileMessage = new FileMessage
                {
                    SenderToken = conversationMessage.SenderToken,
                    ReceiptToken = conversationMessage.ReceiptToken,
                    SendDate = conversationMessage.SendDate,
                    Type = conversationMessage.Type,
                    UID = conversationMessage.UID,
                    UIDReply = conversationMessage.UIDReply,
                    Metadata = info,
                };

                FileUploadRequestMessage fileUploadRequest = new FileUploadRequestMessage();
                fileUploadRequest.FileMetaData = fileMessage;

                CommunicatorClient fClient = new CommunicatorClient(conversationMessage.ReceiptToken);

                byte[] thumbnail = GetThumbnail(conversationMessage.Id);

                using (MemoryStream thumbnailStream = new MemoryStream(thumbnail ?? new byte[0]))
                using (DataStream dt = new DataStream(thumbnailStream))
                {
                    fileUploadRequest.File = dt;
                    fClient.UploadFile(fileUploadRequest);
                }

                commData.DeletePendingMessage(conversationMessage.ReceiptToken, conversationMessage.Id);

                commData.SetMessageState(conversationMessage.Id, (int)MessageState.Processed);

                IoCContainer.Get<NotificationManager>().ProcessMessageStateChanged(new MessageStateNotification 
                {
                    MessageId=conversationMessage.Id,
                    State=MessageState.Processed,
                });
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }

        public FileDownloadResponseMessage DownloadFile(FileDownloadRequestMessage file)
        {
            try
            {
                FileCommunicatorData fileData = new FileCommunicatorData();

                string idFile = BuildIdFile(file.Description.ReceiptToken, file.Description.UID);

                FileDownloadResponseMessage fileResponse = new FileDownloadResponseMessage();
                fileResponse.File = fileData.GetFile(idFile);

                return fileResponse;
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }

        }

        private string BuildIdFile(string userToken, string idFile)
        {
            string tempId = string.Concat(userToken, idFile);
            CryptographyProvider crypto = new CryptographyProvider();
            return crypto.GetRIPEMD160Hash(Encoding.UTF8.GetBytes(tempId));
        }

        /// <summary>
        /// Request to download a file.
        /// </summary>
        /// <param name="senderToken">Token of user that sent file</param>
        public void RequestFile(FileObserver fileObserver)
        {
            CommunicatorData commData = new CommunicatorData();
            try
            {

                FileDownloadRequestMessage fileRequest = new FileDownloadRequestMessage();
                fileRequest.Description = fileObserver.Data;

                CommunicatorClient commClient = new CommunicatorClient(fileObserver.Data.SenderToken);
                FileDownloadResponseMessage fileResponse = commClient.DownloadFile(fileRequest);

                string idFile = BuildIdFile(fileObserver.Data.SenderToken, fileObserver.Data.UID);

                //Delete previous version of file
                FileCommunicatorData fileCommData = new FileCommunicatorData();
                fileCommData.DeleteFile(idFile);

                int cycles = 0;
                using (DataStream dt = new DataStream(fileResponse.File))
                {
                    dt.ReadChanged += (args) =>
                    {
                        if (args.ReadBytes > cycles + 100000)
                        {
                            cycles = args.ReadBytes;
                            fileObserver.OnNext(args.ReadBytes);
                        }
                    };

                    SaveFile(fileObserver.Data.SenderToken, fileObserver.Data.UID, fileObserver.Data.Metadata.FileName, dt);
                }
                commData.SetMessageState(fileObserver.Id, (int)MessageState.Processed);

                fileObserver.OnCompleted();
            }
            catch (EndpointNotFoundException)
            {
                throw;
            }
            catch (CommunicationObjectAbortedException)
            {
                throw;
            }
            catch(CommunicationObjectFaultedException)
            {
                throw;
            }
            catch(CommunicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogDumper.WriteLog(ex);
                fileObserver.OnError(ex);
                throw;
            }
        }

        public Stream GetFile(int idMessage)
        {
            try
            {
                FileCommunicatorData fileData = new FileCommunicatorData();

                string tempId = GetFileId(idMessage);

                return fileData.GetFile(tempId);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }

        public long GetFileSize(int idMessage)
        {
            try
            {
                FileCommunicatorData fileData = new FileCommunicatorData();

                string tempId = GetFileId(idMessage);

                return fileData.GetFileSize(tempId);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }

        public string GetFileId(int idMessage)
        {
            Communicator comm = new Communicator();
            ConversationMessage message = comm.GetMessageDetail(idMessage);

            Peer p = comm.GetSelfProfile();

            string fileId = null;

            if (p.AccountToken == message.SenderToken)
                fileId = BuildIdFile(message.ReceiptToken, message.UID);
            else
                fileId = BuildIdFile(message.SenderToken, message.UID);

            return fileId;
        }

        public byte[] GetThumbnail(int idMessage)
        {
            FileCommunicatorData fileData = new FileCommunicatorData();

            DataTable result = fileData.GetThumbnail(idMessage);

            if (result.Rows.Count > 0)
            {
                return result.Rows[0][0] as byte[];
            }
            else
                return null;
        }

        public void SaveThumbnail(int idMessage, byte[] thumbnail)
        {
            FileCommunicatorData fileData = new FileCommunicatorData();

            fileData.SaveThumbnail(idMessage, thumbnail);
        }

        public int SaveAnimation(int idMessage, string idFile, int width, int height, byte[] animation)
        {
            FileCommunicatorData fileData = new FileCommunicatorData();

            return fileData.SaveAnimation(idMessage, idFile, width, height, animation);
        }

        public List<int> GetAnimationList()
        {
            FileCommunicatorData fileData = new FileCommunicatorData();

            DataTable data = fileData.GetAnimationList();

            List<int> animationList = new List<int>();

            for (int i = 0; i < data.Rows.Count; i++)
                animationList.Add(Convert.ToInt32(data.Rows[i][0]));

            return animationList;
        }

        public AnimationDetail GetAnimationContent(int id)
        {
            FileCommunicatorData fileData = new FileCommunicatorData();

            DataTable data = fileData.GetAnimationContent(id);

            if (data.Rows.Count == 0)
                return null;

            AnimationDetail animationDetail = new AnimationDetail
            {
                IdMessage = Convert.ToInt32(data.Rows[0][0]),
                IdFile = data.Rows[0][1].ToString(),
                Width = Convert.ToInt32(data.Rows[0][2]),
                Height = Convert.ToInt32(data.Rows[0][3]),
                Content = data.Rows[0][4] as byte[],
            };

            return animationDetail;
        }

        public string GetCacheFilePath(int idMessage)
        {
            FileCommunicatorData fileData = new FileCommunicatorData();
            return fileData.GetCacheFilePath(idMessage);
        }

        public void UpdateCacheFilePath(int idMessage, string filePath)
        {
            FileCommunicatorData fileData = new FileCommunicatorData();
            fileData.UpdateCacheFilePath(idMessage, filePath);
        }

        public Dictionary<int,int> GetFileList(string userToken, ContentType fileType, int lastIdMessage, int quantity)
        {
            FileCommunicatorData fileData = new FileCommunicatorData();

            DataTable data = fileData.GetFileList(userToken, (int)fileType, lastIdMessage, quantity);

            if (data.Rows.Count == 0)
                return null;

            Dictionary<int, int> fileList = new Dictionary<int, int>();

            for (int i = 0; i < data.Rows.Count; i++)
                fileList.Add(Convert.ToInt32(data.Rows[i][0]), Convert.ToInt32(data.Rows[i][1]));

            return fileList;
        }

        /// <summary>
        /// Gte the number of files shared between current user and remote peer.
        /// </summary>
        /// <param name="userToken">The type of file.</param>
        /// <param name="fileType">The token of remote peer.</param>
        /// <returns>The number of files.</returns>
        public int GetFileCount(string userToken, ContentType fileType)
        {
            FileCommunicatorData fileData = new FileCommunicatorData();
            return fileData.GetFileCount(userToken, (int)fileType);
        }
    }
}
