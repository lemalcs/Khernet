using Khernet.Core.Processor.IoC;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;

namespace Khernet.Core.Processor.Managers
{
    /// <summary>
    /// Sends a message to receiver and avoid the delay between saving the message locally and sending it over network.
    /// </summary>
    public class TextMessageManager : IDisposable
    {
        private static ConcurrentQueue<int> messageList;

        private Thread processor;
        AutoResetEvent autoReset;
        private bool continueScanning = false;

        Communicator communicator;

        public TextMessageManager()
        {
            messageList = new ConcurrentQueue<int>();
            communicator = new Communicator();
        }

        public void Start()
        {
            if (processor == null)
            {
                autoReset = new AutoResetEvent(false);

                processor = new Thread(new ThreadStart(ProcessTextMessage));
                continueScanning = true;
                processor.Start();
            }
        }

        public void ProcessMessage(int idMessage)
        {
            messageList.Enqueue(idMessage);
            autoReset.Set();
        }

        private void ProcessTextMessage()
        {
            while (continueScanning)
            {
                try
                {
                    while (!messageList.IsEmpty)
                    {
                        int idMessage;

                        if (!messageList.TryPeek(out idMessage))
                            continue;

                        ConversationMessage message = communicator.GetMessageDetail(idMessage);

                        try
                        {
                            communicator.SendPendingMessage((InternalConversationMessage)message);
                        }
                        catch (EndpointNotFoundException error)
                        {
                            communicator.RegisterPendingMessage(message.ReceiverToken, ((InternalConversationMessage)message).Id);
                            LogDumper.WriteLog(error, $"Could not send message to {message.ReceiverToken} due to: {error.Message}");
                        }
                        catch (ThreadAbortException)
                        {
                            communicator.RegisterPendingMessage(message.ReceiverToken, ((InternalConversationMessage)message).Id);
                            throw;
                        }
                        catch (Exception error)
                        {
                            communicator.SetMessageState(((InternalConversationMessage)message).Id, MessageState.Error);

                            IoCContainer.Get<NotificationManager>().ProcessMessageStateChanged(new MessageStateNotification
                            {
                                MessageId = idMessage,
                                State = MessageState.Error,
                            });

                            LogDumper.WriteLog(error, $"Message not sent to {message.ReceiverToken} due to: {error.Message}");
                        }

                        messageList.TryDequeue(out idMessage);
                    }

                    if (messageList.IsEmpty)
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

                messageList = null;
            }
        }

        #region IDisposable Support

        /// <summary>
        /// Variable to detect reentry calls.
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
        /// Cleans resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
