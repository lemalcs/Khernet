using Khernet.Core.Host;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Khernet.UI.Managers
{
    public class MessageStateInfo
    {
        public int Id { get; set; }

        public MessageState State { get; set; }

    }


    public class ChatMessageStateManager : IDisposable
    {
        /// <summary>
        /// The process to manage states.
        /// </summary>
        private Thread stateMonitor;

        /// <summary>
        /// Controls when to start to upload text message.
        /// </summary>
        private AutoResetEvent stateAutoReset;

        /// <summary>
        /// Indicates if state manager should continue running.
        /// </summary>
        private bool stopMonitoring = false;

        /// <summary>
        /// Queue of id messages.
        /// </summary>
        private ConcurrentQueue<MessageStateInfo> idMessageList;

        public ChatMessageStateManager()
        {
            stateAutoReset = new AutoResetEvent(false);
        }

        public void ProcessState(MessageStateInfo messageState)
        {
            if (idMessageList == null)
                idMessageList = new ConcurrentQueue<MessageStateInfo>();

            idMessageList.Enqueue(messageState);

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
                        MessageStateInfo messageState;
                        idMessageList.TryPeek(out messageState);

                        try
                        {
                            ChatMessage message = IoCContainer.Get<Messenger>().GetMessageDetail(messageState.Id);
                            if (message != null)
                            {
                                var chatList = IoCContainer.Chat.GetChat(message.ReceiverToken);
                                if (chatList != null)
                                {
                                    var result = chatList.Where((chat, i) => chat.Id == messageState.Id).FirstOrDefault();
                                    result?.SetChatState((ChatMessageState)(int)message.State);
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
                            idMessageList.TryDequeue(out messageState);
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
                if (stateMonitor != null && stateMonitor.ThreadState != ThreadState.Unstarted)
                {
                    stateAutoReset.Set();
                    stopMonitoring = true;
                    stateMonitor.Interrupt();
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

                idMessageList = null;
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
                    StopProcessor();
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
