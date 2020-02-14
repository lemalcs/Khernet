using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet.org", Name = "NotifierService")]
    public interface IEventNotifier
    {
        [OperationContract(IsOneWay = true)]
        void ProcessContactChange(Notification info);

        [OperationContract(IsOneWay = true)]
        void ProcessWritingMessage(string accountToken);
    }
}
