using Khernet.Core.Host;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Khernet.UI.Managers
{
    public class ChatMessageStateManager : IDisposable
    {
        /// <summary>
        /// The process to manage states
        /// </summary>
        private Thread stateMonitor;

        /// <summary>
        /// Controls when to start to upload text message
        /// </summary>
        private AutoResetEvent stateAutoReset;

        /// <summary>
        /// Indicates if state manager should continue running
        /// </summary>
        private bool stopMonitoring = false;

        /// <summary>
        /// Queue of id messages
        /// </summary>
        private ConcurrentQueue<int> idMessageList;

        public ChatMessageStateManager()
        {
            stateAutoReset = new AutoResetEvent(false);
        }

        public void ProcessState(int idMessage)
        {
            if (idMessageList == null)
                idMessageList = new ConcurrentQueue<int>();

            idMessageList.Enqueue(idMessage);

            StartStateMonitor();
        }

        private void StartStateMonitor()
        {
            if (stateMonitor == null)
            {
                stateMonitor = new Thread(new ThreadStart(ProcessMessageState));
                stateMonitor.Start();
            }

            stateAutoReset.Set();
        }

        private void ProcessMessageState()
        {
            while (!stopMonitoring)
            {
                try
                {
                    while (!idMessageList.IsEmpty)
                    {
                        int idMessage;
                        idMessageList.TryPeek(out idMessage);

                        try
                        {
                            ChatMessage message = IoCContainer.Get<Messenger>().GetMessageDetail(idMessage);
                            if (message != null)
                            {
                                var chatList = IoCContainer.Chat.GetChat(message.ReceiptToken);
                                if (chatList != null)
                                {
                                    var result = chatList.Where((chat, i) => chat.Id == idMessage).FirstOrDefault();
                                    result?.SetChatState(ChatMessageState.Processed);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            System.Diagnostics.Debugger.Break();
                            throw;
                        }
                        finally
                        {
                            idMessageList.TryDequeue(out idMessage);
                        }
                    }

                    if (idMessageList.IsEmpty)
                        stateAutoReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }

        public void StopProcessor()
        {
            try
            {
                idMessageList = null;
                if (stateMonitor != null && stateMonitor.ThreadState != ThreadState.Unstarted)
                {
                    stateAutoReset.Set();
                    stopMonitoring = true;
                    stateMonitor.Interrupt();

                    //If thread does not stop through 1 minute, abort thread
                    if (!stateMonitor.Join(TimeSpan.FromMinutes(1)))
                        stateMonitor.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                stateMonitor = null;

                if (stateAutoReset != null)
                    stateAutoReset.Close();
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
                    StopProcessor();
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
