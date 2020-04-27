using Khernet.Core.Processor;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false)]
    public class EventListenerService : IEventListener, IEventListenerCallBack
    {
        public void ProcessNewMessage(MessageNotification notification)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessNewMessage(notification);
        }

        public void ProcessContactChange(PeerNotification notification)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessContactChange(notification);
        }

        public void ProcessMessageProcessing(MessageProcessingNotification notification)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessMessageProcessing(notification);
        }

        public void Suscribe(string listenerKey)
        {
            EventListener eventListener = new EventListener();
            eventListener.Suscribe(listenerKey);
        }

        public void Echo()
        {

        }

        public void UnSuscribe(string listenerKey)
        {
            EventListener eventListener = new EventListener();
            eventListener.UnSuscribe(listenerKey);
        }

        public void ProcessMessageStateChanged(MessageStateNotification notification)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessMessageStateChanged(notification);
        }
    }
}
