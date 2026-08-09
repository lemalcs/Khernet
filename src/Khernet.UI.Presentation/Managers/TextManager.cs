using Khernet.Core.Entity;
using Khernet.Core.Host;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows;

namespace Khernet.UI.Managers
{
    /// <summary>
    /// Provides notification to observers about Text operation.
    /// </summary>
    public class TextManager : ITextObservable
    {
        /// <summary>
        /// The observers queue for message sending requests.
        /// </summary>
        private ConcurrentQueue<ITextObserver> sendersList;

        /// <summary>
        /// The observers queue for message receiving requests.
        /// </summary>
        private ConcurrentQueue<ITextObserver> receiversList;

        /// <summary>
        /// The process to send messages.
        /// </summary>
        private Thread messageSender;

        /// <summary>
        /// The process to send messages.
        /// </summary>
        private Thread messageReceiver;

        /// <summary>
        /// Indicates if processor should continue running.
        /// </summary>
        private bool stopProcessing = false;

        /// <summary>
        /// Controls when to start to upload text message.
        /// </summary>
        private AutoResetEvent senderAutoReset;

        /// <summary>
        /// Controls when to start to download text message.
        /// </summary>
        private AutoResetEvent receiverAutoReset;


        public TextManager()
        {
            senderAutoReset = new AutoResetEvent(false);
            receiverAutoReset = new AutoResetEvent(false);
        }

        /// <summary>
        /// Posts and request to process a Text file.
        /// </summary>
        /// <param name="observer">The observer that request Text processing.</param>
        public void ProcessText(ITextObserver observer)
        {
            //Check if observer is null
            if (observer == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(observer)} cannot be null");
            }

            if (observer.Text.OperationRequest == MessageOperation.Upload)
            {
                //Creates the senders list
                if (sendersList == null)
                    sendersList = new ConcurrentQueue<ITextObserver>();

                //Enqueue the request
                sendersList.Enqueue(observer);

                //Start the processor to send messages
                StartSenderProcessor();
            }
            else if (observer.Text.OperationRequest == MessageOperation.Download)
            {
                //Creates the receivers list
                if (receiversList == null)
                    receiversList = new ConcurrentQueue<ITextObserver>();

                //Enqueue the request
                receiversList.Enqueue(observer);

                //Start the processor to receive messages
                StartReceiverProcessor();
            }

        }

        /// <summary>
        /// Starts the processor to send messages, creates a new one if does not exists yet.
        /// </summary>
        private void StartSenderProcessor()
        {
            if (messageSender == null)
            {
                messageSender = new Thread(new ThreadStart(ProcessSendingRequest));
                stopProcessing = false;
                messageSender.Start();
            }

            senderAutoReset.Set();
        }

        /// <summary>
        /// Starts the processor to send messages, creates a new one if does not exists yet.
        /// </summary>
        private void StartReceiverProcessor()
        {
            if (messageReceiver == null)
            {
                messageReceiver = new Thread(new ThreadStart(ProcessReceivingRequest));
                stopProcessing = false;
                messageReceiver.Start();
            }

            receiverAutoReset.Set();
        }

        /// <summary>
        /// Performs operations sending requests.
        /// </summary>
        private void ProcessSendingRequest()
        {
            while (!stopProcessing)
            {
                try
                {
                    //Check if observer queue has elements
                    while (!sendersList.IsEmpty)
                    {
                        //Get the next observer
                        ITextObserver observer;
                        sendersList.TryPeek(out observer);

                        try
                        {
                            //Check if an observer was retrieved
                            if (sendersList.TryPeek(out observer))
                            {
                                SendMessage(observer);
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
                            sendersList.TryDequeue(out observer);
                        }
                    }

                    if (sendersList.IsEmpty)
                        senderAutoReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }


        /// <summary>
        /// Performs operations receiving requests.
        /// </summary>
        private void ProcessReceivingRequest()
        {
            while (!stopProcessing)
            {
                try
                {
                    //Check if observer queue has elements
                    while (!receiversList.IsEmpty)
                    {
                        //Get the next observer
                        ITextObserver observer;
                        receiversList.TryPeek(out observer);

                        try
                        {
                            //Check if an observer was retrieved
                            if (receiversList.TryPeek(out observer))
                            {
                                ReceiveMessage(observer);
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
                            receiversList.TryDequeue(out observer);
                        }
                    }

                    if (receiversList.IsEmpty)
                        receiverAutoReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }

        private void SendMessage(ITextObserver observer)
        {
            ChatMessageState state = ChatMessageState.Pending;
            int idMessage = 0;
            try
            {
                MessageProcessResult result = IoCContainer.Get<Messenger>().SendTextMessage(
                           observer.Text.ChatMessage.SenderUserId.Token,// SenderToken,
                           observer.Text.ChatMessage.ReceiverUserId.Token,// ReceiverToken,
                           observer.Text.Content,
                           observer.Text.IdReplyMessage,
                           (ContentType)((int)observer.Text.FileType),
                           observer.Text.ChatMessage.UID,// UID,
                           observer.Text.ChatMessage.TimeId// TimeId
                           );

                idMessage = result.Id;
                state = (ChatMessageState)((int)result.Result);

            }
            catch (Exception error)
            {
                state = ChatMessageState.Error;
                //Notify about an error to the UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    observer.OnError(error);
                }));
            }
            finally
            {
                ChatMessageProcessResult result = new ChatMessageProcessResult(idMessage, state);
                observer.OnCompleted(result);
            }
        }

        private void ReceiveMessage(ITextObserver observer)
        {
            ChatMessageState state = ChatMessageState.Pending;
            try
            {
                byte[] content = IoCContainer.Get<Messenger>().GetMessageContent(observer.Text.ChatMessage.Id);
                InternalConversationMessage detail = (InternalConversationMessage)IoCContainer.Get<Messenger>().GetMessageDetail(observer.Text.ChatMessage.Id);

                TextResponse response = new TextResponse
                {
                    Content = content,
                    IdReplyMessage = detail.IdReply,
                    Operation = MessageOperation.Download,
                    UID = detail.UID,
                    TimeId = detail.TimeId,
                };
                state = ((ChatMessageState)(int)detail.State);

                observer.OnGetMetadata(response);
            }
            catch (Exception error)
            {
                state = ChatMessageState.Error;
                //Notify about an error to the UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    observer.OnError(error);
                }));
            }
            finally
            {
                ChatMessageProcessResult result = new ChatMessageProcessResult(observer.Text.ChatMessage.Id, state);
                observer.OnCompleted(result);
            }
        }

        /// <summary>
        /// Stops the processor of Text files.
        /// </summary>
        public void StopProcessor()
        {
            try
            {
                if (messageSender != null && messageSender.ThreadState != ThreadState.Unstarted)
                {
                    senderAutoReset.Set();
                    stopProcessing = true;
                    messageSender.Interrupt();
                    messageSender.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                messageSender = null;

                if (senderAutoReset != null)
                    senderAutoReset.Close();

                sendersList = null;
            }

            try
            {
                if (messageReceiver != null && messageReceiver.ThreadState != ThreadState.Unstarted)
                {
                    receiverAutoReset.Set();
                    stopProcessing = true;
                    messageReceiver.Interrupt();
                    messageReceiver.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                messageReceiver = null;

                if (receiverAutoReset != null)
                    receiverAutoReset.Close();

                receiversList = null;
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
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
