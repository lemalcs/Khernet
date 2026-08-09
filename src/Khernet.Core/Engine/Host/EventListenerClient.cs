using Khernet.Core.Common;
using Khernet.Core.Utility;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System;
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
        public event ContactChangedEventHandler PeerChanged;
        public event MessageProcessingHandler ProcessingMessage;
        public event MessageStateChangedEventHandler MessageStateChanged;

        public void Start()
        {
            Connect();
            StartConnectionChecker();
        }

        /// <summary>
        /// Start process to check if connection is open to internal communication service.
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
        /// Connect to internal communication service.
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

                EndpointAddress address = new EndpointAddress(Configuration.GetValue(Constants.SubscriberService));

                listenerClient = new DuplexChannelFactory<IEventListener>(listenerContext, binding, address);
                listenerService = listenerClient.CreateChannel();

                listenerService.Subscribe(Configuration.GetValue(Constants.ListenerKey));
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        /// <summary>
        /// Check if connection to internal communication service is open, otherwise try to open a new connection.
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
                            LogDumper.WriteLog(commException, "CommunicationException occurred.");

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



        protected void OnMessageArrive(MessageNotification notification)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    MessageArrived?.Invoke(this, new MessageArrivedEventArgs(notification));
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                    throw;
                }
            });
        }

        protected void OnContactChange(PeerNotification notification)
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

        protected void OnMessageProcessing(MessageProcessingNotification notification)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    ProcessingMessage?.Invoke(this, new MessageProcessingEventArgs(notification));
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                    throw exception;
                }
            });
        }

        protected void OnMessageStateChanged(MessageStateNotification notification)
        {
            try
            {
                MessageStateChanged?.Invoke(this, new MessageStateChangedEventArgs(notification));
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
        private class EventListenerContext : IEventListenerCallBack
        {
            EventListenerClient client;

            public EventListenerContext(EventListenerClient listenerClient)
            {
                client = listenerClient;
            }

            public void ProcessNewMessage(MessageNotification message)
            {
                client.OnMessageArrive(message);
            }

            public void ProcessContactChange(PeerNotification info)
            {
                client.OnContactChange(info);
            }

            public void ProcessMessageProcessing(MessageProcessingNotification notification)
            {
                client.OnMessageProcessing(notification);
            }

            public void ProcessMessageStateChanged(MessageStateNotification notification)
            {
                client.OnMessageStateChanged(notification);
            }
        }

        /// <summary>
        /// Stop client connected to internal communication service.
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
                            listenerService.Unsubscribe(Configuration.GetValue(Constants.ListenerKey));
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
