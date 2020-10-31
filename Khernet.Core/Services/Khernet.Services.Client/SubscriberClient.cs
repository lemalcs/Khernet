using Khernet.Services.Contracts;
using System;
using System.ServiceModel;

namespace Khernet.Services.Client
{
    public class SubscriberClient
    {
        public string Address { get; private set; }

        public SubscriberClient(string address)
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

        public void Subscribe(string listenerKey)
        {
            try
            {
                using (ServiceClient<IEventListener> client = new ServiceClient<IEventListener>(GetBinding(), Address))
                {
                    client.serviceContract.Subscribe(listenerKey);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Unsubscribe(string listenerKey)
        {
            try
            {
                using (ServiceClient<IEventListener> client = new ServiceClient<IEventListener>(GetBinding(), Address))
                {
                    client.serviceContract.Unsubscribe(listenerKey);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
