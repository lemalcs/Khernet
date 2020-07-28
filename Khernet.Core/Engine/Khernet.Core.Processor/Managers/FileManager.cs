using Khernet.Core.Common;
using Khernet.Core.Entity;
using Khernet.Core.Processor.IoC;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Khernet.Core.Processor.Managers
{
    public class FileManager : IDisposable
    {
        private static ConcurrentQueue<int> fileList;

        private Thread processor;
        AutoResetEvent autoReset;
        private bool continueScanning = false;

        FileCommunicator fileCommunicator;
        Communicator communicator;

        public FileManager()
        {
            fileList = new ConcurrentQueue<int>();
            communicator = new Communicator();
        }

        public void Start()
        {
            if (processor == null)
            {
                autoReset = new AutoResetEvent(false);

                processor = new Thread(new ThreadStart(ProcessFiles));
                continueScanning = true;
                fileCommunicator = new FileCommunicator();
                processor.Start();
            }
        }

        public void ProcessFile(int idMessage)
        {
            fileList.Enqueue(idMessage);
            autoReset.Set();
        }

        private void ProcessFiles()
        {
            while (continueScanning)
            {
                FileObserver fileObserver = null;
                ConversationMessage message = null;

                try
                {
                    while (!fileList.IsEmpty)
                    {
                        int idMessage;

                        if (!fileList.TryPeek(out idMessage))
                            continue;

                        try
                        {
                            message = communicator.GetMessageDetail(idMessage);

                            //Check if file is already downloaded and ready to use by this application
                            if (message.State != MessageState.UnCommited)
                                continue;

                            FileInformation info = JSONSerializer<FileInformation>.DeSerialize(communicator.GetMessageContent(idMessage));

                            FileMessage fileMessage = new FileMessage
                            {
                                SenderToken = message.SenderToken,
                                ReceiptToken = message.ReceiptToken,
                                Metadata = info,
                                SendDate = message.SendDate,
                                Type = message.Type,
                                UID = message.UID,
                                UIDReply = message.UIDReply,
                                TimeId=message.TimeId,
                            };

                            fileObserver = new FileObserver(fileMessage);
                            fileObserver.Id = idMessage;

                            IoCContainer.Get<NotificationManager>().ProcessMessageProcessing(new MessageProcessingNotification
                            {
                                Process = MessageProcessing.BeginSendingFile,
                                SenderToken = message.SenderToken,
                            });

                            fileObserver.ReadCompleted += FileObserver_ReadCompleted;
                            fileObserver.ReadFailed += FileObserver_ReadFailed;

                            try
                            {
                                fileCommunicator.RequestFile(fileObserver);
                            }
                            catch (Exception)
                            {
                                List<int> pendingList = communicator.GetRequestPendingMessageForUser(message.SenderToken, 0);
                                if (pendingList == null || (pendingList != null && !pendingList.Contains(idMessage)))
                                    communicator.RegisterPendingMessage(message.SenderToken, idMessage);

                                throw;
                            }

                            SendNotification(idMessage, fileMessage);
                        }
                        catch (ThreadAbortException ex)
                        {
                            LogDumper.WriteLog(ex);
                            return;
                        }
                        catch (Exception ex)
                        {
                            LogDumper.WriteLog(ex);
                        }
                        finally
                        {
                            if (message != null)
                                IoCContainer.Get<NotificationManager>().ProcessMessageProcessing(new MessageProcessingNotification
                                {
                                    Process = MessageProcessing.EndSendingFile,
                                    SenderToken = message.SenderToken,
                                });

                            if (fileObserver != null)
                            {
                                fileObserver.ReadCompleted -= FileObserver_ReadCompleted;
                                fileObserver.ReadFailed -= FileObserver_ReadFailed;
                            }

                            fileList.TryDequeue(out idMessage);
                        }
                    }

                    if (fileList.IsEmpty)
                        autoReset.WaitOne();
                }
                catch (ThreadAbortException exception)
                {
                    LogDumper.WriteLog(exception);
                    return;
                }
                catch (ThreadInterruptedException exception)
                {
                    LogDumper.WriteLog(exception);
                    return;
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                }
            }
        }

        private void FileObserver_ReadFailed(object sender, ReadFailedEventArgs e)
        {
            IoCContainer.Get<NotificationManager>().ProcessMessageProcessing(new MessageProcessingNotification
            {
                Process = MessageProcessing.EndSendingFile,
                SenderToken = ((FileObserver)sender).Data.SenderToken,
            });
        }

        private void FileObserver_ReadCompleted(object sender)
        {
            IoCContainer.Get<NotificationManager>().ProcessMessageProcessing(new MessageProcessingNotification
            {
                Process = MessageProcessing.EndSendingFile,
                SenderToken = ((FileObserver)sender).Data.SenderToken,
            });
        }

        /// <summary>
        /// Send notification to a consumer about a new received file 
        /// </summary>
        /// <param name="idMessage">The id of message</param>
        /// <param name="fileMessage">Metadata of file</param>
        private void SendNotification(int idMessage, FileMessage fileMessage)
        {
            try
            {
                InternalFileMessage internalMessage = new InternalFileMessage();
                internalMessage.SenderToken = fileMessage.SenderToken;
                internalMessage.ReceiptToken = fileMessage.ReceiptToken;
                internalMessage.SendDate = fileMessage.SendDate;
                internalMessage.Metadata = fileMessage.Metadata;
                internalMessage.Type = fileMessage.Type;
                internalMessage.UID = fileMessage.UID;
                internalMessage.UIDReply = fileMessage.UIDReply;
                internalMessage.Id = idMessage;

                PublisherClient publisherClient = new PublisherClient(Configuration.GetValue(Constants.PublisherService));
                publisherClient.ProcessNewMessage(new MessageNotification
                {
                    MessageId = idMessage,
                    SenderToken = fileMessage.SenderToken,
                    State = MessageState.Processed,
                    Format = fileMessage.Type,
                });
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
            }
        }

        public void Stop()
        {
            try
            {
                continueScanning = false;
                if (processor != null && processor.ThreadState != ThreadState.Unstarted)
                {
                    processor.Interrupt();
                    autoReset.Set();
                    processor.Abort();
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
            }
            finally
            {
                if (autoReset != null)
                    autoReset.Close();

                fileList = null;
            }
        }

        #region IDisposable Support

        /// <summary>
        /// Variable to detect reentry calls
        /// </summary>
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Cleans resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
