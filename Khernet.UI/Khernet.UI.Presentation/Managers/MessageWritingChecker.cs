using Khernet.UI.IoC;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Khernet.UI.Managers
{
    /// <summary>
    /// Checks the status of writing event for text messages.
    /// </summary>
    public class MessageWritingChecker : IDisposable
    {
        #region Properties
        /// <summary>
        /// Message event pool.
        /// </summary>
        private ConcurrentQueue<MessageEventData> messageEventPool;

        /// <summary>
        /// Temporal pool of message events.
        /// </summary>
        private ConcurrentQueue<MessageEventData> temporalPool;

        /// <summary>
        /// Pool of pending requests to process message events.
        /// </summary>
        private ConcurrentQueue<MessageEventData> pendingEventPool;

        /// <summary>
        /// The process to send message events.
        /// </summary>
        private Thread eventProcessor;

        /// <summary>
        /// Indicates if processor should continue running.
        /// </summary>
        private bool stopProcessing = false;

        /// <summary>
        /// Controls when to start to process message events.
        /// </summary>
        private AutoResetEvent processorAutoReset;

        #endregion

        public MessageWritingChecker()
        {
            processorAutoReset = new AutoResetEvent(false);
        }

        #region Methods

        private void StartEventMessageProcessor()
        {
            if (messageEventPool == null)
                messageEventPool = new ConcurrentQueue<MessageEventData>();

            if (temporalPool == null)
                temporalPool = new ConcurrentQueue<MessageEventData>();

            if (pendingEventPool == null)
                pendingEventPool = new ConcurrentQueue<MessageEventData>();

            if (eventProcessor == null)
            {
                eventProcessor = new Thread(new ThreadStart(ProcessStatusChecker));
                stopProcessing = false;
                eventProcessor.Start();
            }

            processorAutoReset.Set();
        }

        /// <summary>
        /// Requests to process a message event.
        /// </summary>
        /// <param name="messageEvent">The message event to process to.</param>
        public void BeginCheckEvent(MessageEventData messageEvent)
        {
            if (pendingEventPool == null)
                pendingEventPool = new ConcurrentQueue<MessageEventData>();

            pendingEventPool.Enqueue(messageEvent);

            StartEventMessageProcessor();
        }

        /// <summary>
        /// Process the message events.
        /// </summary>
        private void ProcessStatusChecker()
        {
            while (!stopProcessing)
            {
                try
                {
                    while (!messageEventPool.IsEmpty || !temporalPool.IsEmpty || !pendingEventPool.IsEmpty)
                    {
                        try
                        {
                            ConcurrentQueue<MessageEventData> primaryPool = messageEventPool;
                            ConcurrentQueue<MessageEventData> secondaryPool = temporalPool;

                            //Always try to use the pool that has items
                            if (primaryPool.IsEmpty)
                            {
                                primaryPool = temporalPool;
                                secondaryPool = messageEventPool;
                            }

                            MessageEventData senderEvent;
                            pendingEventPool.TryDequeue(out senderEvent);

                            MessageEventData currentEvent;
                            bool foundEvent = false;

                            while (!primaryPool.IsEmpty)
                            {
                                primaryPool.TryDequeue(out currentEvent);

                                //Update arrive date if new writing event has been received
                                //This avoid to hide the writing indicator
                                if ((!foundEvent && senderEvent != null) &&
                                    (currentEvent.SenderPeer == senderEvent.SenderPeer
                                    && currentEvent.EventType == senderEvent.EventType))
                                {
                                    currentEvent.ArriveDate = DateTime.Now;
                                    foundEvent = true;
                                    secondaryPool.Enqueue(currentEvent);
                                }
                                //If no new writing event has been received then send an end writing event
                                //This allow to hide the writing indicator
                                else if ((DateTime.Now - currentEvent.ArriveDate).TotalSeconds > 4)
                                {
                                    currentEvent.EventType = MessageEvent.EndWriting;
                                    IoCContainer.Get<MessageProcessingEventManager>().ProcessMessageEvent(currentEvent);
                                }
                                else
                                    //Add the event to check status later
                                    //This keeps the writing indicator showing
                                    secondaryPool.Enqueue(currentEvent);
                            }

                            if (!foundEvent && senderEvent != null)
                                secondaryPool.Enqueue(senderEvent);

                            Thread.Sleep(100);

                        }
                        catch (ThreadAbortException)
                        {
                            return;
                        }
                        catch (ThreadInterruptedException)
                        {
                            return;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    if (messageEventPool.IsEmpty & temporalPool.IsEmpty & pendingEventPool.IsEmpty)
                        processorAutoReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// Stops the processor ans release resources.
        /// </summary>
        public void StopProcessor()
        {
            try
            {
                if (eventProcessor != null && eventProcessor.ThreadState != ThreadState.Unstarted)
                {
                    processorAutoReset.Set();
                    stopProcessing = true;
                    eventProcessor.Interrupt();
                    eventProcessor.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                eventProcessor = null;

                if (processorAutoReset != null)
                    processorAutoReset.Close();

                messageEventPool = null;
                temporalPool = null;
                pendingEventPool = null;
            }
        }

        #endregion

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
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
