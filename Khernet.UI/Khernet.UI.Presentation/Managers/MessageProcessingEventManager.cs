using Khernet.Core.Host;
using Khernet.UI.IoC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Khernet.UI.Managers
{
    /// <summary>
    /// Provides notification to listeners about message events.
    /// </summary>
    public class MessageProcessingEventManager : IMessageEventObservable
    {
        #region Properties
        /// <summary>
        /// The queue for message events to be sent.
        /// </summary>
        private ConcurrentQueue<MessageEventData> eventSenderList;

        /// <summary>
        /// The queue for message events sent by other peers.
        /// </summary>
        private ConcurrentQueue<MessageEventData> eventReceiverList;

        /// <summary>
        /// The list of subscribers whom listen message events.
        /// </summary>
        private List<IMessageEventObserver> subscribersList;

        /// <summary>
        /// The process to send message events.
        /// </summary>
        private Thread eventSender;

        /// <summary>
        /// The process to process message events.
        /// </summary>
        private Thread eventReceiver;

        /// <summary>
        /// Indicates if processor should continue running.
        /// </summary>
        private bool stopProcessing = false;

        /// <summary>
        /// Controls when to start to send message events.
        /// </summary>
        private AutoResetEvent senderAutoReset;

        /// <summary>
        /// Controls when to start to process message events.
        /// </summary>
        private AutoResetEvent receiverAutoReset;

        #endregion

        public MessageProcessingEventManager()
        {
            senderAutoReset = new AutoResetEvent(false);
            receiverAutoReset = new AutoResetEvent(false);
        }

        #region Methods

        /// <summary>
        /// Send a message event to peer.
        /// </summary>
        /// <param name="observer">The event data.</param>
        public void SendMessageEvent(MessageEventData messageEvent)
        {
            //Check if observer is null
            if (messageEvent == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(messageEvent)} cannot be null");
            }

            if (eventSenderList == null)
                eventSenderList = new ConcurrentQueue<MessageEventData>();

            eventSenderList.Enqueue(messageEvent);
            StartSenderProcessor();
        }

        /// <summary>
        /// Starts the processor to send message events, creates a new one if does not exists yet.
        /// </summary>
        private void StartSenderProcessor()
        {
            if (eventSender == null)
            {
                eventSender = new Thread(new ThreadStart(ProcessSendingEvent));
                stopProcessing = false;
                eventSender.Start();
            }

            senderAutoReset.Set();
        }

        /// <summary>
        /// Perform the actual operation of sending message events.
        /// </summary>
        private void ProcessSendingEvent()
        {
            while (!stopProcessing)
            {
                try
                {
                    //Check if observer queue has elements
                    while (eventSenderList.Count > 0)
                    {
                        //Get the next observer
                        MessageEventData senderEvent;
                        eventSenderList.TryPeek(out senderEvent);

                        try
                        {
                            //Check if an observer was retrieved
                            if (senderEvent != null)
                            {
                                IoCContainer.Get<Messenger>().SendWrtitingMessage(senderEvent.SenderPeer, senderEvent.ReceiverPeer);
                            }
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
                        finally
                        {
                            eventSenderList.TryDequeue(out senderEvent);
                        }
                    }

                    if (eventSenderList.Count == 0)
                        senderAutoReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }


        /// <summary>
        /// Process a message event sent by other peer.
        /// </summary>
        /// <param name="messageEvent">The event data.</param>
        public void ProcessMessageEvent(MessageEventData messageEvent)
        {
            //Check if observer is null
            if (messageEvent == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(messageEvent)} cannot be null");
            }

            if (eventReceiverList == null)
                eventReceiverList = new ConcurrentQueue<MessageEventData>();

            eventReceiverList.Enqueue(messageEvent);
            StartReceiverProcessor();
        }

        /// <summary>
        /// Starts the processor to process message events, creates a new one if does not exists yet.
        /// </summary>
        private void StartReceiverProcessor()
        {
            if (eventReceiver == null)
            {
                eventReceiver = new Thread(new ThreadStart(ProcessReceivingEvent));
                stopProcessing = false;
                eventReceiver.Start();
            }

            receiverAutoReset.Set();
        }


        /// <summary>
        /// Performs the actual operations of processing message events.
        /// </summary>
        private void ProcessReceivingEvent()
        {
            while (!stopProcessing)
            {
                try
                {
                    //Check if observer queue has elements
                    while (eventReceiverList.Count > 0)
                    {
                        //Get the next observer
                        MessageEventData receiverEvent;
                        eventReceiverList.TryPeek(out receiverEvent);

                        try
                        {
                            //Check if an observer was retrieved
                            if (receiverEvent != null)
                            {
                                IMessageEventObserver observer = subscribersList.Find((s) => s.Token == receiverEvent.SenderPeer);
                                if (observer != null)
                                {
                                    observer.OnUpdate(receiverEvent);

                                    if (receiverEvent.EventType == MessageEvent.BeginWriting)
                                        IoCContainer.Get<MessageWritingChecker>().BeginCheckEvent(receiverEvent);
                                }
                            }
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
                        finally
                        {
                            eventReceiverList.TryDequeue(out receiverEvent);
                        }
                    }

                    if (eventReceiverList.Count == 0)
                        receiverAutoReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// Stops the processor and release resources.
        /// </summary>
        public void StopProcessor()
        {
            try
            {
                if (eventSender != null && eventSender.ThreadState != ThreadState.Unstarted)
                {
                    senderAutoReset.Set();
                    stopProcessing = true;
                    eventSender.Interrupt();
                    eventSender.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                eventSender = null;

                if (senderAutoReset != null)
                    senderAutoReset.Close();

                eventSenderList = null;
            }

            try
            {
                if (eventReceiver != null && eventReceiver.ThreadState != ThreadState.Unstarted)
                {
                    receiverAutoReset.Set();
                    stopProcessing = true;
                    eventReceiver.Interrupt();
                    eventReceiver.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                eventReceiver = null;

                if (receiverAutoReset != null)
                    receiverAutoReset.Close();

                eventReceiverList = null;
                subscribersList.Clear();
                subscribersList = null;
            }
        }

        #endregion 

        #region IMessageEventObservable members

        /// <summary>
        /// Subscribe a listener of message events.
        /// </summary>
        /// <param name="eventObserver">The listener to subscribe to.</param>z
        public void Subscribe(IMessageEventObserver eventObserver)
        {
            if (subscribersList == null)
                subscribersList = new List<IMessageEventObserver>();

            subscribersList.Add(eventObserver);
        }

        /// <summary>
        /// Unsubscribes the listener of message events.
        /// </summary>
        /// <param name="eventObserver">The listener to unsubscribe to.</param>
        public void Unsubscribe(IMessageEventObserver eventObserver)
        {
            if (subscribersList == null)
                return;

            subscribersList.Remove(eventObserver);
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
