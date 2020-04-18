using Khernet.Core.Common;
using Khernet.Core.Utility;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Khernet.Core.Host
{
    public class EventListenerClient
    {
        InstanceContext listenerContext;
        DuplexChannelFactory<IEventListener> listenerClient;
        IEventListener listenerService;
        Thread connectionChecker;
        volatile bool continueChecking = false;

        public event MessageArrivedEventHandler MessageArrived;
        public event FileArrivedEventHandler FileArrived;
        public event ContactChangedEventHandler PeerChanged;
        public event WritingMessageEventHandler WritingMessage;
        public event SendingFileEventHandler BeginSendingFile;
        public event SendingFileEventHandler EndSendingFile;
        public event ReadingFileEventHandler ReadingFile;
        public event MessageSentEventHandler MessageSent;

        public void Start()
        {
            Connect();
            StartConnectionChecker();
        }

        /// <summary>
        /// Start process to check if connection is open to internal communication service
        /// </summary>
        private void StartConnectionChecker()
        {
            try
            {
                connectionChecker = new Thread(CheckConnection);
                connectionChecker.Name = "ConnectionChecker";

                continueChecking = true;
                connectionChecker.Start();
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        /// <summary>
        /// Connect to internal communication service
        /// </summary>
        private void Connect()
        {
            try
            {
                if (listenerService != null)
                {
                    listenerClient.Abort();
                }

                listenerContext = new InstanceContext(new EventListenerContext(this));

                NetNamedPipeBinding binding = new NetNamedPipeBinding();
                binding.TransferMode = TransferMode.Buffered;
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.CloseTimeout = TimeSpan.MaxValue;

                EndpointAddress address = new EndpointAddress(Configuration.GetValue(Constants.SuscriberService));

                listenerClient = new DuplexChannelFactory<IEventListener>(listenerContext, binding, address);
                listenerService = listenerClient.CreateChannel();

                listenerService.Suscribe(Configuration.GetValue(Constants.ListenerKey));
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        /// <summary>
        /// Check if connetion to internal communication service is open, 
        /// otherwise try to open a new connection
        /// </summary>
        private void CheckConnection()
        {
            while (continueChecking)
            {
                try
                {
                    if (listenerService != null)
                    {
                        try
                        {
                            listenerService.Echo();
                        }
                        catch (CommunicationException commException)
                        {
                            LogDumper.WriteLog(commException, "CommunicationException ocurred.");
                            
                            //Try to connect again
                            if (continueChecking)
                                Connect();
                        }
                        catch (Exception innerException)
                        {
                            LogDumper.WriteLog(innerException);

                            //Try to connect again
                            if (continueChecking)
                                Connect();
                        }
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                }
            }
        }



        protected void OnMessageArrive(InternalConversationMessage message)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    MessageArrived?.Invoke(this, new Host.MessageArrivedEventArgs(message));
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                    throw;
                }
            });
        }

        protected void OnFileArrive(InternalFileMessage file)
        {
            try
            {
                FileArrived?.BeginInvoke(this, new FileArrivedEventArgs(file), CallBackProgress, "");
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        protected void OnContactChange(Notification notification)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    PeerChanged?.Invoke(this, new ContactChangedEventArgs(notification));
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                    throw exception;
                }
            });
        }

        protected void OnWritingMessage(string accountToken)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    WritingMessage?.Invoke(this, new WritingMessageEventArgs(accountToken));
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                    throw exception;
                }
            });
        }
        protected void OnBeginSendingFile(string accountToken)
        {
            try
            {
                BeginSendingFile?.Invoke(this, new SendingFileEventArgs(accountToken));
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        protected void OnEndSendingFile(string accountToken)
        {
            try
            {
                EndSendingFile?.Invoke(this, new SendingFileEventArgs(accountToken));
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        protected void OnReadingFile(string token, string idFile, long readBytes)
        {
            try
            {
                ReadingFile?.Invoke(this, new ReadingFileEventArgs(token, idFile, readBytes));
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        protected void OnMessageSent(string token, int idMessage)
        {
            try
            {
                MessageSent?.Invoke(this, new MessageSentEventArgs(token, idMessage));
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        private void CallBackProgress(IAsyncResult result)
        {
            AsyncResult res = (AsyncResult)result;
            FileArrivedEventHandler handler = (FileArrivedEventHandler)res.AsyncDelegate;

            handler.EndInvoke(result);
        }

        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
        private class EventListenerContext : IEventListenerCallBack
        {
            EventListenerClient client;

            public EventListenerContext(EventListenerClient listenerClient)
            {
                client = listenerClient;
            }

            public void ProcessNewMessage(InternalConversationMessage message)
            {
                client.OnMessageArrive(message);
            }

            public void ProcessNewFile(InternalFileMessage fileMessage)
            {
                client.OnFileArrive(fileMessage);
            }
            public void ProcessContactChange(Notification info)
            {
                client.OnContactChange(info);
            }

            public void ProcessBeginSendingFile(string accountToken)
            {
                client.OnBeginSendingFile(accountToken);
            }

            public void ProcessEndSendingFile(string accountToken)
            {
                client.OnEndSendingFile(accountToken);
            }

            public void ProcessWritingMessage(string accountToken)
            {
                client.OnWritingMessage(accountToken);
            }

            public void ProcessReadingFile(string token, string idFile, long readBytes)
            {
                client.OnReadingFile(token, idFile, readBytes);
            }

            public void ProcessMessageSent(string token, int idMessage)
            {
                client.OnMessageSent(token, idMessage);
            }
        }

        /// <summary>
        /// Stop client connected to internal communication service
        /// </summary>
        public void Stop()
        {
            try
            {
                continueChecking = false;
                if (listenerClient != null && listenerService != null)
                {
                    if (listenerClient.State == CommunicationState.Opened)
                    {
                        try
                        {
                            listenerService.UnSuscribe(Configuration.GetValue(Constants.ListenerKey));
                            listenerClient.Close();
                        }
                        catch (CommunicationException ex)
                        {
                            LogDumper.WriteLog(ex, "Server not available");
                            listenerClient.Abort();
                        }
                        finally
                        {
                            ICommunicationObject service = (ICommunicationObject)listenerService;
                            if (service.State == CommunicationState.Opened)
                                service.Close();
                            else
                                service.Abort();
                        }
                    }
                }
                
                if (connectionChecker != null)
                {
                    connectionChecker.Interrupt();
                    connectionChecker.Abort();
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception, "Client event listener stopped");
            }
        }

    }
}
