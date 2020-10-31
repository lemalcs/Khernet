using System.ServiceModel;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet.org", Name = "EventListenerService", CallbackContract = typeof(IEventListenerCallBack))]
    public interface IEventListener
    {
        [OperationContract]
        void Subscribe(string listenerKey);

        [OperationContract]
        void Unsubscribe(string listenerKey);

        [OperationContract]
        void Echo();
    }
}
