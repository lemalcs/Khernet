using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System;
using System.ServiceModel;

namespace Khernet.Services.Client
{
    public class PublisherClient
    {
        public string Address { get; private set; }

        public PublisherClient(string address)
        {
            Address = address;
        }
        private static NetNamedPipeBinding GetBinding()
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            binding.TransferMode = TransferMode.Buffered;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.CloseTimeout = TimeSpan.MaxValue;

            return binding;
        }

        public void ProcessNewMessage(MessageNotification message)
        {
            try
            {
                using (ServiceClient<IEventListenerCallBack> client = new ServiceClient<IEventListenerCallBack>(GetBinding(), Address))
                {
                    client.serviceContract.ProcessNewMessage(message);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ProcessContactChange(PeerNotification notification)
        {
            try
            {
                using (ServiceClient<IEventListenerCallBack> client = new ServiceClient<IEventListenerCallBack>(GetBinding(), Address))
                {
                    client.serviceContract.ProcessContactChange(notification);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ProcessMessageProcessing(MessageProcessingNotification notification)
        {
            try
            {
                using (ServiceClient<IEventListenerCallBack> client = new ServiceClient<IEventListenerCallBack>(GetBinding(), Address))
                {
                    client.serviceContract.ProcessMessageProcessing(notification);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ProcessMessageStateChanged(MessageStateNotification notification)
        {
            try
            {
                using (ServiceClient<IEventListenerCallBack> client = new ServiceClient<IEventListenerCallBack>(GetBinding(), Address))
                {
                    client.serviceContract.ProcessMessageStateChanged(notification);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
