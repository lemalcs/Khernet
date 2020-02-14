using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet.org", Name = "CommunicatorService")]
    public interface ICommunicator
    {
        [OperationContract]
        Peer GetProfile();

        [OperationContract(IsOneWay = true)]
        void ProcessTextMessage(ConversationMessage message);
    }
}
