using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet.org", Name = "EventListenerCallBack")]
    public interface IEventListenerCallBack
    {
        [OperationContract(IsOneWay = true)]
        void ProcessNewMessage(InternalConversationMessage message);

        [OperationContract(IsOneWay = true)]
        void ProcessNewFile(InternalFileMessage fileMessage);

        [OperationContract(IsOneWay = true)]
        void ProcessContactChange(Notification info);

        [OperationContract(IsOneWay = true)]
        void ProcessWritingMessage(string accountToken);

        [OperationContract(IsOneWay = true)]
        void ProcessBeginSendingFile(string accountToken);

        [OperationContract(IsOneWay = true)]
        void ProcessEndSendingFile(string accountToken);

        [OperationContract(IsOneWay = true)]
        void ProcessReadingFile(string token, string idFile, long readBytes);

        [OperationContract(IsOneWay = true)]
        void ProcessMessageSent(string token, int idMessage);
    }
}
