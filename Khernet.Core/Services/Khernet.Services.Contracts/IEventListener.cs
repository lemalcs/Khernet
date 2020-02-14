using System.ServiceModel;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet.org", Name = "EventListenerService", CallbackContract = typeof(IEventListenerCallBack))]
    public interface IEventListener
    {
        [OperationContract]
        void Suscribe(string listenerKey);

        [OperationContract]
        void UnSuscribe(string listenerKey);

        [OperationContract]
        void Echo();
    }
}
