using Khernet.Core.Utility;
using Khernet.Services.Messages;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace Khernet.Core.Processor.Managers
{
    public class MessageManager : IDisposable
    {
        private Thread penddingThread;
        private volatile bool continueExecution = false;
        ManualResetEvent manualResetEvent;
        Communicator communicator;

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
                    manualResetEvent = new ManualResetEvent(false);

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

        public void RegisterPenddingMessage(string receiptToken, int idMessage)
        {
            communicator.RegisterPenddingMessage(receiptToken, idMessage);
            manualResetEvent.Set();
            manualResetEvent.Reset();
        }

        private void SendPenddingMessages()
        {
            while (continueExecution)
            {
                try
                {
                    Thread.Sleep(5000);

                    Communicator communicator = new Communicator();

                    List<string> usersList = communicator.GetPenddingMessageUsers();

                    if (usersList == null)
                    {
                        manualResetEvent.WaitOne();
                        usersList = communicator.GetPenddingMessageUsers();
                    }

                    List<int> messageList = null;

                    for (int i = 0; i < usersList.Count; i++)
                    {
                        messageList = communicator.GetPenddingMessageOfUser(usersList[i], 1);

                        if (messageList == null)
                            continue;

                        //Try to one send a message to test if peer is connected
                        try
                        {
                            SendPenddingMessage(messageList[0]);
                        }
                        catch (EndpointNotFoundException ex)
                        {
                            LogDumper.WriteLog(ex, "Peer disconnected");

                            //If peer is disconnected then continue with next peer
                            continue;
                        }
                        catch (Exception ex2)
                        {
                            LogDumper.WriteLog(ex2);
                        }

                        //Get the full list of pendding messages
                        messageList = communicator.GetPenddingMessageOfUser(usersList[i], 0);

                        if (messageList == null)
                            continue;

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
                                LogDumper.WriteLog(error, "Error while reading pendding message");
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
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

        public void Stop()
        {
            try
            {
                continueExecution = false;
                if (penddingThread != null && penddingThread.ThreadState != ThreadState.Unstarted)
                {
                    penddingThread.Interrupt();
                    manualResetEvent.Set();

                    if (!penddingThread.Join(TimeSpan.FromSeconds(3)))
                        penddingThread.Abort();
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
            finally
            {
                if (manualResetEvent != null)
                    manualResetEvent.Close();
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
