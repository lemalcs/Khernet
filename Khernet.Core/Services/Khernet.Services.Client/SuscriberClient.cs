using Khernet.Services.Contracts;
using System;
using System.ServiceModel;

namespace Khernet.Services.Client
{
    public class SuscriberClient
    {
        public string Address { get; private set; }

        public SuscriberClient(string address)
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

        public void Suscribe(string listenerKey)
        {
            try
            {
                using (ServiceClient<IEventListener> client = new ServiceClient<IEventListener>(GetBinding(), Address))
                {
                    client.serviceContract.Suscribe(listenerKey);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UnSuscribe(string listenerKey)
        {
            try
            {
                using (ServiceClient<IEventListener> client = new ServiceClient<IEventListener>(GetBinding(), Address))
                {
                    client.serviceContract.UnSuscribe(listenerKey);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
