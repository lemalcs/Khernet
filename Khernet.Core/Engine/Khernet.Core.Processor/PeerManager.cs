using Khernet.Core.Common;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Khernet.Core.Processor
{
    public class PeerManager
    {
        /// <summary>
        /// Adds the services addresses exposed by peer.
        /// </summary>
        /// <param name="token">The token of peer.</param>
        /// <param name="hostnameIP">The host name of the gateway peer.</param>
        /// <param name="port">The port of the gateway peer.</param>
        public void SearchAndAddPeer(string token, string hostnameIP, int port)
        {
            Uri gatewayAddress = DiscoveryHelper.BuildTcpAddress(hostnameIP, port);
            GatewayClient client = new GatewayClient(token, gatewayAddress.AbsoluteUri);
            PeerService peerService = client.GetPeerServiceInfo();

            Communicator communicator = new Communicator();
            CryptographyProvider cryptographyProvider = new CryptographyProvider();

            // Save gateway address
            communicator.SavePeer(
                   peerService.Username,
                   peerService.Token,
                   cryptographyProvider.DecodeBase58Check(peerService.Certificate),
                   gatewayAddress.AbsoluteUri,
                   Constants.GatewayService);

            // Save communicator and file service addresses
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

        /// <summary>
        /// Add the service addresses exposed by a peer.
        /// </summary>
        /// <param name="peerService">The token of peer.</param>
        public void AddPeer(PeerService peerService)
        {
            Communicator communicator = new Communicator();
            CryptographyProvider cryptographyProvider = new CryptographyProvider();

            // Save communicator and file service addresses
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

        /// <summary>
        /// Gets the X509 certificate of a peer encoded in BASE58Check.
        /// </summary>
        /// <param name="token">The token of peer.</param>
        /// <returns>The X509 certificate encoded in BASE58Check.</returns>
        public string GetEncodedCertificate(string token)
        {
            X509Certificate cert = new PeerFinder().GetCertificate(token);
            if (cert == null)
                return null;

            return (new CryptographyProvider()).GetBase58Check(cert.Export(X509ContentType.Cert));
        }
    }
}
