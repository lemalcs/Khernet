using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet.org", Name = "GatewayService")]
    public interface IGateway
    {
        [OperationContract]
        PeerService GetPeerServiceInfo();
    }
}
