using Khernet.Core.Entity;
using Khernet.Core.Host;
using Khernet.Services.Messages;
using Khernet.UI.Controls;
using Khernet.UI.IoC;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows;

namespace Khernet.UI.Managers
{
    /// <summary>
    /// Provides notification to observers about Text operation
    /// </summary>
    public class TextManager : ITextObservable
    {
        /// <summary>
        /// The observers queue for message sending requests
        /// </summary>
        private ConcurrentQueue<ITextObserver> sendersList;

        /// <summary>
        /// The observers queue for message receiving requests
        /// </summary>
        private ConcurrentQueue<ITextObserver> receiversList;

        /// <summary>
        /// The process to send messages
        /// </summary>
        private Thread messageSender;

        /// <summary>
        /// The process to send messages
        /// </summary>
        private Thread messageReceiver;

        /// <summary>
        /// Indicates if processor should continue running
        /// </summary>
        private bool stopProcessing = false;

        /// <summary>
        /// Controls when to start to upload text message
        /// </summary>
        private ManualResetEvent senderManualReset;

        /// <summary>
        /// Controls when to start to download text message
        /// </summary>
        private ManualResetEvent receiverManualReset;


        public TextManager()
        {
            senderManualReset = new ManualResetEvent(false);
            receiverManualReset = new ManualResetEvent(false);
        }

        /// <summary>
        /// Posts and request to process a Text file
        /// </summary>
        /// <param name="observer">The observer that request Text processing</param>
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
        /// Starts the processor to send messages, creates a new one if does not exists yet
        /// </summary>
        private void StartSenderProcessor()
        {
            if (messageSender == null)
            {
                messageSender = new Thread(new ThreadStart(ProcessSendingRequest));
                stopProcessing = false;
                messageSender.Start();
            }

            senderManualReset.Set();
            senderManualReset.Reset();
        }

        /// <summary>
        /// Starts the processor to send messages, creates a new one if does not exists yet
        /// </summary>
        private void StartReceiverProcessor()
        {
            if (messageReceiver == null)
            {
                messageReceiver = new Thread(new ThreadStart(ProcessReceivingRequest));
                stopProcessing = false;
                messageReceiver.Start();
            }

            receiverManualReset.Set();
            receiverManualReset.Reset();
        }

        /// <summary>
        /// Performs operations sending requests
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
                        catch (Exception)
                        {
                            System.Diagnostics.Debugger.Break();
                            throw;
                        }
                        finally
                        {
                            sendersList.TryDequeue(out observer);
                        }
                    }

                    senderManualReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }


        /// <summary>
        /// Performs operations receiving requests
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
                        catch (Exception)
                        {
                            System.Diagnostics.Debugger.Break();
                            throw;
                        }
                        finally
                        {
                            receiversList.TryDequeue(out observer);
                        }
                    }

                    receiverManualReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }

        private void SendMessage(ITextObserver observer)
        {
            ChatMessageState state = ChatMessageState.Pendding;
            int idMessage = 0;
            try
            {
                MessageProcessResult result = IoCContainer.Get<Messenger>().SendTextMessage(
                           observer.Text.SenderToken,
                           observer.Text.ReceiptToken,
                           observer.Text.Content,
                           observer.Text.IdReplyMessage,
                           (ContentType)((int)observer.Text.FileType),
                           observer.Text.UID);

                idMessage = result.Id;
                state = result.Result == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;

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
            ChatMessageState state = ChatMessageState.Pendding;
            try
            {
                byte[] content = IoCContainer.Get<Messenger>().GetMessageContent(observer.Text.Id);
                InternalConversationMessage detail = (InternalConversationMessage)IoCContainer.Get<Messenger>().GetMessageDetail(observer.Text.Id);

                TextResponse response = new TextResponse
                {
                    Content = content,
                    IdReplyMessage = detail.IdReply,
                    Operation = MessageOperation.Download,
                    UID = detail.UID,
                };
                state = detail.State == MessageState.Processed ? ChatMessageState.Processed : ChatMessageState.Pendding;

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
                ChatMessageProcessResult result = new ChatMessageProcessResult(observer.Text.Id, state);
                observer.OnCompleted(result);
            }
        }

        /// <summary>
        /// Stops the processor of Text files
        /// </summary>
        public void StopProcessor()
        {
            try
            {
                sendersList = null;
                if (messageSender != null && messageSender.ThreadState != ThreadState.Unstarted)
                {
                    senderManualReset.Set();
                    stopProcessing = true;
                    messageSender.Interrupt();

                    //If thread does not stop through 1 minute, abort thread
                    if (!messageSender.Join(TimeSpan.FromMinutes(1)))
                        messageSender.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                messageSender = null;

                if (senderManualReset != null)
                    senderManualReset.Close();
            }

            try
            {
                receiversList = null;
                if (messageReceiver != null && messageReceiver.ThreadState != ThreadState.Unstarted)
                {
                    receiverManualReset.Set();
                    stopProcessing = true;
                    messageReceiver.Interrupt();

                    //If thread does not stop through 1 minute, abort thread
                    if (!messageReceiver.Join(TimeSpan.FromMinutes(1)))
                        messageReceiver.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                messageReceiver = null;

                if (receiverManualReset != null)
                    receiverManualReset.Close();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar llamadas redundantes

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
