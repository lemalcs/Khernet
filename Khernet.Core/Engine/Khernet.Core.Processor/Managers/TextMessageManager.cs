using Khernet.Core.Utility;
using Khernet.Services.Messages;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Khernet.Core.Processor.Managers
{
    /// <summary>
    /// Sends a message to receiver and avoid the delay between saveing the message locally and sendding it over network
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

                        communicator.SendPenddingMessage((InternalConversationMessage)message);

                        messageList.TryDequeue(out idMessage);
                    }
                    autoReset.WaitOne();
                }
                catch (ThreadInterruptedException exception)
                {
                    LogDumper.WriteLog(exception);
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
                    if (!processor.Join(TimeSpan.FromMinutes(2)))
                        processor.Abort();
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
            finally
            {
                if (autoReset != null)
                    autoReset.Close();
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
