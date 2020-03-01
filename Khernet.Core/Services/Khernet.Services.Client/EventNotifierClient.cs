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

        public void ProcessContactChange(Notification info)
        {
            try
            {
                using (ServiceClient<IEventNotifier> client = new Services.Client.ServiceClient<IEventNotifier>(Address, ServiceType.NotifierService))
                {
                    client.serviceContract.ProcessContactChange(info);
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
                using (ServiceClient<IEventNotifier> client = new Services.Client.ServiceClient<IEventNotifier>(Address, ServiceType.NotifierService))
                {
                    client.serviceContract.ProcessWritingMessage(accountToken);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
