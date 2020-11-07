using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet.org", Name = "NotifierService")]
    public interface IEventNotifier
    {
        [OperationContract(IsOneWay = true)]
        void ProcessMessageProcessing(MessageProcessingNotification notification);
    }
}
