using Khernet.Core.Processor.IoC;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace Khernet.Core.Processor.Managers
{
    /// <summary>
    /// Sends pendding messages to peers whem them turn online again.
    /// </summary>
    public class MessageManager : IDisposable
    {
        private Thread penddingThread;
        private volatile bool continueExecution = false;
        AutoResetEvent autoResetEvent;
        Communicator communicator;

        private ConcurrentQueue<string> usersList;

        public MessageManager()
        {
            try
            {
                communicator = new Communicator();
                communicator.RegisterNotSentMessage();
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
            }
        }

        public void Start()
        {
            try
            {
                if (penddingThread == null)
                {
                    autoResetEvent = new AutoResetEvent(false);

                    penddingThread = new Thread(SendPenddingMessages);
                    penddingThread.Name = "PenddingSender";

                    continueExecution = true;
                    penddingThread.Start();
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void ProcessPenddingMessagesOf(string receiptToken)
        {
            if (usersList == null)
                usersList = new ConcurrentQueue<string>();

            usersList.Enqueue(receiptToken);

            autoResetEvent.Set();
        }

        private void SendPenddingMessages()
        {
            while (continueExecution)
            {
                try
                {
                    if (usersList == null)
                        autoResetEvent.WaitOne();

                    while (usersList.Count > 0)
                    {
                        string receiptToken;

                        usersList.TryDequeue(out receiptToken);

                        if (string.IsNullOrEmpty(receiptToken))
                            continue;

                        SendPendingMessageOf(receiptToken);

                        RequestPendingMessageOf(receiptToken);
                    }

                    if (usersList.Count == 0)
                        autoResetEvent.WaitOne();
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

        private void SendPendingMessageOf(string receiptToken)
        {
            List<int> messageList = communicator.GetPenddingMessageOfUser(receiptToken, 1);

            if (messageList == null)
                return;

            //Try to one send a message to test if peer is connected
            try
            {
                SendPenddingMessage(messageList[0]);
            }
            catch (EndpointNotFoundException ex)
            {
                LogDumper.WriteLog(ex, "Peer disconnected");

                //If peer is disconnected then continue with next peer
                return;
            }
            catch (Exception ex2)
            {
                SetMessageState(messageList[0], MessageState.Error);

                IoCContainer.Get<NotificationManager>().ProcessMessageStateChanged(new MessageStateNotification
                {
                    MessageId = messageList[0],
                    State = MessageState.Error,
                });

                LogDumper.WriteLog(ex2);
            }

            //Get the full list of pendding messages
            messageList = communicator.GetPenddingMessageOfUser(receiptToken, 0);

            if (messageList == null)
                return;

            //Send pendding messages 
            for (int j = 0; j < messageList.Count; j++)
            {
                try
                {
                    SendPenddingMessage(messageList[j]);
                }
                catch (EndpointNotFoundException error1)
                {
                    LogDumper.WriteLog(error1, "Peer disconnected");

                    //If current peer turns disconnected then continue with next peer
                    break;
                }
                catch (Exception error)
                {
                    SetMessageState(messageList[j], MessageState.Error);
                    IoCContainer.Get<NotificationManager>().ProcessMessageStateChanged(new MessageStateNotification
                    {
                        MessageId = messageList[j],
                        State = MessageState.Error,
                    });
                    LogDumper.WriteLog(error, "Error while reading pendding message");
                }
            }
        }

        private void SendPenddingMessage(int idMessage)
        {
            try
            {
                var message = communicator.GetMessageDetail(idMessage);

                if (message.Type == ContentType.Text ||
                    message.Type == ContentType.Html ||
                    message.Type == ContentType.Markdown)
                {
                    Communicator communicator = new Communicator();
                    communicator.SendPenddingMessage((InternalConversationMessage)message);
                }
                else
                {
                    FileCommunicator fileManager = new FileCommunicator();
                    fileManager.SendPenddingFile((InternalConversationMessage)message);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void RequestPendingMessageOf(string receiptToken)
        {
            List<int> messageList = communicator.GetRequestPendingMessageForUser(receiptToken, 1);

            if (messageList == null)
                return;

            IoCContainer.Get<FileManager>().ProcessFile(messageList[0]);
        }

        private void SetMessageState(int idMessage, MessageState state)
        {
            Communicator communicator = new Communicator();
            communicator.SetMessageState(idMessage, state);
        }

        public void Stop()
        {
            try
            {
                continueExecution = false;
                if (penddingThread != null && penddingThread.ThreadState != ThreadState.Unstarted)
                {
                    penddingThread.Interrupt();
                    autoResetEvent.Set();
                    penddingThread.Abort();
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
            }
            finally
            {
                if (autoResetEvent != null)
                    autoResetEvent.Close();

                usersList = null;
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
