﻿using Khernet.Core.Common;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Khernet.Core.Processor
{
    public class Gateway
    {
        /// <summary>
        /// Gets the services addresses exposed by current user.
        /// </summary>
        /// <returns>A <see cref="PeerService"/> containing the user addresses.</returns>
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
                ServiceType = Constants.FileService,
            });

            return new PeerService
            {
                Username = username,
                Token = userToken,
                Certificate = certificate,
                ServiceList = services
            };
        }

        /// <summary>
        /// Get the address of gateway of this application.
        /// </summary>
        /// <returns>A string with the complete address of gateway service.</returns>
        public string GetSelfGatewayAddress()
        {
            return Configuration.GetValue(Constants.GatewayService);
        }

        /// <summary>
        /// Gets the gateway address of a specific peer.
        /// </summary>
        /// <param name="token">The token of peer.</param>
        /// <returns>The address of gateway.</returns>
        public string GetPeerGatewayAddress(string token)
        {
            Communicator communicator = new Communicator();
            return communicator.GetPeerAdress(token, Constants.GatewayService);
        }
    }
}
