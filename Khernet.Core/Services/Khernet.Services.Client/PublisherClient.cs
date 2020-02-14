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

        public void ProcessNewMessage(InternalConversationMessage message)
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

        public void ProcessNewFile(InternalFileMessage fileMessage)
        {
            try
            {
                using (ServiceClient<IEventListenerCallBack> client = new ServiceClient<IEventListenerCallBack>(GetBinding(), Address))
                {
                    client.serviceContract.ProcessNewFile(fileMessage);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void ProcessContactChange(Notification notification)
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

        public void ProcessBeginSendingFile(string accountToken)
        {
            try
            {
                using (ServiceClient<IEventListenerCallBack> client = new ServiceClient<IEventListenerCallBack>(GetBinding(), Address))
                {
                    client.serviceContract.ProcessBeginSendingFile(accountToken);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ProcessEndSendingFile(string accountToken)
        {
            try
            {
                using (ServiceClient<IEventListenerCallBack> client = new ServiceClient<IEventListenerCallBack>(GetBinding(), Address))
                {
                    client.serviceContract.ProcessEndSendingFile(accountToken);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ProcessReadingFile(string token, string idFile, long readBytes)
        {
            try
            {
                using (ServiceClient<IEventListenerCallBack> client = new ServiceClient<IEventListenerCallBack>(GetBinding(), Address))
                {
                    client.serviceContract.ProcessReadingFile(token, idFile, readBytes);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ProcessWritingMessage(string accountToken)
        {
            try
            {
                using (ServiceClient<IEventListenerCallBack> client = new ServiceClient<IEventListenerCallBack>(GetBinding(), Address))
                {
                    client.serviceContract.ProcessWritingMessage(accountToken);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ProcessMessageSent(string token, int idMessage)
        {
            try
            {
                using (ServiceClient<IEventListenerCallBack> client = new ServiceClient<IEventListenerCallBack>(GetBinding(), Address))
                {
                    client.serviceContract.ProcessMessageSent(token, idMessage);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
