using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;

namespace Khernet.Core.Processor
{
    public class PeerManager
    {
        public void AddPeer(string hostnameIP, int port)
        {
            GatewayClient client = new GatewayClient(DiscoveryHelper.BuildTcpAddress(hostnameIP, port).AbsoluteUri);
            PeerService peerService = client.GetPeerServiceInfo();
            Communicator communicator = new Communicator();
            CryptographyProvider cryptographyProvider = new CryptographyProvider();
            foreach (ServiceInfo service in peerService.ServiceList)
            {
                communicator.SavePeer(
                    peerService.Username,
                    peerService.Token,
                    cryptographyProvider.DecodeBase58Check(peerService.Certificate),
                    service.Address,
                    service.ServiceType);
            }
        }
    }
}
