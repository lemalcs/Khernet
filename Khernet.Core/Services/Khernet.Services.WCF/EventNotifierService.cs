using Khernet.Core.Processor;
using Khernet.Services.Common;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false, AddressFilterMode = AddressFilterMode.Any)]
    [ServiceErrorBehavior(typeof(ServiceErrorHandler))]
    [KhernetServiceInspectorBehavior]
    public class EventNotifierService : IEventNotifier
    {
        public void ProcessMessageProcessing(MessageProcessingNotification notification)
        {
            EventNotifier eventNotifier = new EventNotifier();
            eventNotifier.ProcessMessageProcessing(notification);
        }
    }
}
