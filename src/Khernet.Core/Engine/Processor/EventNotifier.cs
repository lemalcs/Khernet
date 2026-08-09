using Khernet.Core.Common;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using System;

namespace Khernet.Core.Processor
{
    public class EventNotifier
    {
        public void ProcessContactChange(PeerNotification notification)
        {
            try
            {
                PublisherClient publisher = new PublisherClient(Configuration.GetValue(Constants.PublisherService));
                publisher.ProcessContactChange(notification);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }

        public void ProcessMessageProcessing(MessageProcessingNotification notification)
        {
            try
            {
                PublisherClient publisher = new PublisherClient(Configuration.GetValue(Constants.PublisherService));
                publisher.ProcessMessageProcessing(notification);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }
    }
}
