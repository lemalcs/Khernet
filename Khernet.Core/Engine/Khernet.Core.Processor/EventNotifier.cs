using Khernet.Core.Common;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using System;

namespace Khernet.Core.Processor
{
    public class EventNotifier
    {
        public void ProcessContactChange(Notification info)
        {
            try
            {
                PublisherClient publisher = new PublisherClient(Configuration.GetValue(Constants.PublisherService));
                publisher.ProcessContactChange(info);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }

        public void ProcessWritingMessage(string accountToken)
        {
            try
            {
                PublisherClient publisher = new PublisherClient(Configuration.GetValue(Constants.PublisherService));
                publisher.ProcessWritingMessage(accountToken);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }
    }
}
