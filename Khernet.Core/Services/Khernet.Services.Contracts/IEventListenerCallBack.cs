using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet.org", Name = "EventListenerCallBack")]
    public interface IEventListenerCallBack
    {
        [OperationContract(IsOneWay = true)]
        void ProcessNewMessage(MessageNotification notification);

        [OperationContract(IsOneWay = true)]
        void ProcessContactChange(PeerNotification info);

        [OperationContract(IsOneWay = true)]
        void ProcessMessageProcessing(MessageProcessingNotification notification);

        [OperationContract(IsOneWay = true)]
        void ProcessMessageStateChanged(MessageStateNotification notification);
    }
}
