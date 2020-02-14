using Khernet.Core.Processor;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false)]
    public class EventListenerService : IEventListener, IEventListenerCallBack
    {
        public void ProcessNewMessage(InternalConversationMessage message)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessNewMessage(message);
        }
        public void ProcessNewFile(InternalFileMessage fileMessage)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessNewFile(fileMessage);
        }

        public void ProcessContactChange(Notification info)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessContactChange(info);
        }

        public void ProcessWritingMessage(string accountToken)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessWritingMessage(accountToken);
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

        public void ProcessBeginSendingFile(string accountToken)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessBeginSendingFile(accountToken);
        }

        public void ProcessEndSendingFile(string accountToken)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessEndSendingFile(accountToken);
        }

        public void ProcessReadingFile(string token, string idFile, long readBytes)
        {
            EventListener eventListener = new EventListener();
            eventListener.ProcessReadingFile(token, idFile, readBytes);
        }

        public void ProcessMessageSent(string token, int idMessage)
        {
            EventListener eventListener = new EventListener();
        }
    }
}
