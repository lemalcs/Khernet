using Khernet.Core.Common;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Khernet.Core.Processor
{
    public class Gateway
    {
        public PeerService GetPeerServiceInfo()
        {
            Peer currentUser = new Communicator().GetSelfProfile();
            string username = currentUser.UserName;
            string userToken = currentUser.AccountToken;
            string commService = Configuration.GetValue(Constants.CommunicatorService);
            string fileService = Configuration.GetValue(Constants.FileService);
            X509Certificate cert = new X509Certificate(ChannelConfiguration.ClientCertificate.RawData);
            string certificate = (new CryptographyProvider()).GetBase58Check(cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert));

            List<ServiceInfo> services = new List<ServiceInfo>();
            services.Add(new ServiceInfo
            {
                Address = commService,
                ServiceType = Constants.CommunicatorService,
            });

            services.Add(new ServiceInfo
            {
                Address = fileService,
                ServiceType = Constants.CommunicatorService,
            });

            return new PeerService
            {
                Username= username,
                Token = userToken,
                Certificate = certificate,
                ServiceList = services
            };
        }

        /// <summary>
        /// Get the address of gateway of this application.
        /// </summary>
        /// <returns>A string with the complete addres of gateway service.</returns>
        public string GetGatewayAddress()
        {
            return Configuration.GetValue(Constants.GatewayService);
        }
    }
}
