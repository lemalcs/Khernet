using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System;

namespace Khernet.Services.Client
{
    public class EventNotifierClient
    {
        public string Address { get; private set; }

        public EventNotifierClient(string address)
        {
            Address = address;
        }

        public void ProcessMessageProcessing(MessageProcessingNotification notification)
        {
            try
            {
                using (ServiceClient<IEventNotifier> client = new Services.Client.ServiceClient<IEventNotifier>(Address, ServiceType.NotifierService))
                {
                    client.serviceContract.ProcessMessageProcessing(notification);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
