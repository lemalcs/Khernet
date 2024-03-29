﻿using Khernet.Core.Common;
using Khernet.Core.Entity;
using Khernet.Core.Processor;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Contracts;
using Khernet.Services.WCF;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Xml.Linq;

namespace Khernet.Core.Host
{
    public class Initializer
    {
        /// <summary>
        /// Identification of current logged user on network.
        /// </summary>
        PeerIdentity identity;

        ServiceHost commHost;
        ServiceHost eventHost;

        PeerWatcher peerWatcher;
        SessionMonitor sessionMonitor;

        public Initializer()
        {
        }

        public Initializer(PeerIdentity peerIdentity)
        {
            identity = peerIdentity;
        }

        public void SetUserIdentity(PeerIdentity peerIdentity)
        {
            identity = peerIdentity;
        }

        public void Init()
        {
            InitializeDatabase();
            InitManagers();
            StartPeerWatcher();
            StartEventListener();
            StartCommunicator();
            InitSessionMonitor();
        }

        /// <summary>
        /// Performs cleaning operations in database.
        /// </summary>
        private void InitializeDatabase()
        {
            Communicator comm = new Communicator();
            comm.ClearPartialMessages();
        }

        private void StartCommunicator()
        {
            try
            {
                //Create service to send and receive chat message
                //This service will be the first that clients will call in network
                commHost = new ServiceHost(typeof(CommunicatorService));

                //Using of TCP binding
                NetTcpBinding binding = new NetTcpBinding();
                binding.TransferMode = TransferMode.Buffered;
                binding.CloseTimeout = TimeSpan.MaxValue;
                binding.ReceiveTimeout = TimeSpan.MaxValue;
                binding.SendTimeout = TimeSpan.MaxValue;
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                binding.Security.Mode = SecurityMode.Message;
                binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;

                Uri commAddress = new Uri(DiscoveryHelper.AvailableTCPBaseAddress.ToString() + Guid.NewGuid());

                //Set certificate to authenticate this service to other services on network
                commHost.Credentials.ServiceCertificate.Certificate = identity.Certificate;

                //Set custom validator for client credentials
                commHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
                commHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new CertificateValidator();

                //Create and endpoint with a port which will be different every time the service is started
                commHost.AddServiceEndpoint(typeof(ICommunicator), binding, commAddress);

                //Add an endpoint to announce the presence of this service on network
                ServiceDiscoveryBehavior announceBehavior = new ServiceDiscoveryBehavior();
                announceBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
                commHost.Description.Behaviors.Add(announceBehavior);

#if DEBUG
                SetupFaultExceptions(commHost.Description.Behaviors, true);
#endif

                //Add discovery support over UDP protocol to find other services similar to CommunicatorService
                commHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());

                //Every peer on network will use a token which is a sequence of numbers and letters
                EndpointDiscoveryBehavior endpointDiscoveryBehavior = new EndpointDiscoveryBehavior();
                endpointDiscoveryBehavior.Extensions.Add(BuildXMLDescription(Constants.CommunicatorService));
                ServiceEndpoint simpleEndPoint = commHost.Description.Endpoints.Find(typeof(ICommunicator));
                simpleEndPoint.Behaviors.Add(endpointDiscoveryBehavior);

                //
                //Create and endpoint for service used to send and receive files
                //

                //Using of TCP binding
                NetTcpBinding fileBinding = new NetTcpBinding();
                fileBinding.TransferMode = TransferMode.Streamed;
                fileBinding.MaxReceivedMessageSize = int.MaxValue;
                fileBinding.MaxBufferSize = 11512000;
                fileBinding.MaxBufferPoolSize = 11500000;
                fileBinding.ReaderQuotas.MaxBytesPerRead = 11512000;
                fileBinding.CloseTimeout = TimeSpan.MaxValue;
                fileBinding.ReceiveTimeout = TimeSpan.MaxValue;
                fileBinding.SendTimeout = TimeSpan.MaxValue;
                fileBinding.Security.Mode = SecurityMode.Transport;
                fileBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                Uri fileAddress = new Uri(DiscoveryHelper.AvailableTCPBaseAddress.ToString() + Guid.NewGuid());

                //Create and endpoint with a port which will be different every time the service is started
                commHost.AddServiceEndpoint(typeof(IFileService), fileBinding, fileAddress);

                EndpointDiscoveryBehavior fileEndpointDiscoveryBehavior = new EndpointDiscoveryBehavior();
                fileEndpointDiscoveryBehavior.Extensions.Add(BuildXMLDescription(Constants.FileService));
                ServiceEndpoint simpleFileEndPoint = commHost.Description.Endpoints.Find(typeof(IFileService));
                simpleFileEndPoint.Behaviors.Add(fileEndpointDiscoveryBehavior);

                commHost.BeginOpen(
                    (result) =>
                    {
                        commHost.EndOpen(result);
                    },
                    null);

                Configuration.SetValue(Constants.CommunicatorService, commAddress.AbsoluteUri);
                Configuration.SetValue(Constants.FileService, fileAddress.AbsoluteUri);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }


        /// <summary>
        /// Enable or disable the inclusion of exceptions in service description.
        /// </summary>
        /// <param name="serviceBehaviors">The service descriptions list.</param>
        /// <param name="enableIncludeExceptions">Indicates whether exceptions must be included.</param>
        private void SetupFaultExceptions(KeyedByTypeCollection<IServiceBehavior> serviceBehaviors, bool enableIncludeExceptions)
        {
            foreach (IServiceBehavior behavior in serviceBehaviors)
            {
                if (behavior is ServiceDebugBehavior)
                {
                    ((ServiceDebugBehavior)behavior).IncludeExceptionDetailInFaults = enableIncludeExceptions;
                }
            }
        }

        private void StartEventListener()
        {
            try
            {
                eventHost = new ServiceHost(typeof(EventListenerService));

                NetNamedPipeBinding binding = new NetNamedPipeBinding();
                binding.TransferMode = TransferMode.Buffered;
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.CloseTimeout = TimeSpan.MaxValue;

                //Address of subscriber service
                //Endpoint to process subscription requests from clients so they can receive notification
                Uri subscriberAddress = new Uri(DiscoveryHelper.AvailableIPCBaseAddress.ToString());
                eventHost.AddServiceEndpoint(typeof(IEventListener), binding, subscriberAddress);

                //Address of publisher service
                //Endpoint to send notifications to subscribers
                Uri publisherAddress = new Uri(DiscoveryHelper.AvailableIPCBaseAddress.ToString());
                eventHost.AddServiceEndpoint(typeof(IEventListenerCallBack), binding, publisherAddress);

#if DEBUG
                SetupFaultExceptions(eventHost.Description.Behaviors, true);
#endif

                eventHost.BeginOpen(
                    (result) =>
                    {
                        eventHost.EndOpen(result);
                    },
                    null);


                Configuration.SetValue(Constants.SubscriberService, subscriberAddress.AbsoluteUri);
                Configuration.SetValue(Constants.PublisherService, publisherAddress.AbsoluteUri);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        private XElement BuildXMLDescription(string serviceID)
        {
            XElement xmlData = new XElement("khernet");
            CryptographyProvider crypto = new CryptographyProvider();

            //Encode user name because it may contain not printable characters
            xmlData.Add(new XElement(Constants.UserNameTag, identity.UserName));

            //Get list of IP v4 addresses
            List<string> ipList = NetworkHelper.GetIPAddresses(Dns.GetHostName(), System.Net.Sockets.ProtocolFamily.InterNetwork);
            string tempAddresses = ConcatenateAddresses(ipList, Constants.IPv4Address);

            //Get list of IP v6 addresses
            ipList = NetworkHelper.GetIPAddresses(Dns.GetHostName(), System.Net.Sockets.ProtocolFamily.InterNetworkV6);
            tempAddresses += ConcatenateAddresses(ipList, Constants.IPv6Address);

            xmlData.Add(new XElement(Constants.AlternateTag, tempAddresses));
            xmlData.Add(new XElement(Constants.ServiceIDTag, serviceID));

            //Encode certificate with BASE58Check
            X509Certificate cert = new X509Certificate(identity.Certificate.RawData);
            xmlData.Add(new XElement(Constants.PeerCertificateTag, crypto.GetBase58Check(cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert))));

            return xmlData;
        }

        /// <summary>
        /// Concatenates a list of IP addresses with the format: [Protocol]:[IP Address].
        /// </summary>
        /// <param name="addressList">Lits of IP address.</param>
        /// <param name="protocol">Type of protocol based on <see cref="Constants.IPv4Address"/>.</param>
        /// <returns></returns>
        private string ConcatenateAddresses(List<string> addressList, int protocol)
        {
            if (addressList == null)
                return null;

            StringBuilder addresses = new StringBuilder();
            CryptographyProvider crypto = new CryptographyProvider();
            for (int i = 0; i < addressList.Count; i++)
            {
                string addr = string.Format("{0}:{1}|", protocol, crypto.GetBase58Check(Encoding.UTF8.GetBytes(addressList[i])));
                if (i + 1 == addressList.Count)
                    addr.Replace("|", "");

                addresses.Append(addr);
            }


            return addresses.ToString();
        }

        /// <summary>
        /// Probe for services that implement ICommunicator contract.
        /// </summary>
        private void StartPeerWatcher()
        {
            try
            {
                peerWatcher = new PeerWatcher(identity);
                peerWatcher.Start();
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        private void InitSessionMonitor()
        {
            try
            {
                sessionMonitor = new SessionMonitor();
                sessionMonitor.SessionClosing += SessionMonitor_SessionClosing;
                sessionMonitor.Start();
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        private void InitManagers()
        {
            Manager.StartFileManager();
            Manager.StartMessageManager();
            Manager.StartNotificationManager();
            Manager.StartTextMessageManager();
        }

        private void SessionMonitor_SessionClosing(object sender, EventArgs eventArgs)
        {
            Stop();
        }

        public void Stop()
        {
            try
            {
                if (peerWatcher != null)
                    peerWatcher.Stop();

                if (sessionMonitor != null)
                    sessionMonitor.Stop();

                StopCommunicatorService();
                StopEventListenerService();
                peerWatcher = null;
                sessionMonitor = null;

                StopManagers();
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
            }
        }

        private void StopCommunicatorService()
        {
            try
            {
                if (commHost != null)
                {
                    commHost.Close();
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
            }
        }

        private void StopEventListenerService()
        {
            try
            {
                if (eventHost != null)
                {
                    eventHost.Close();
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
            }
        }

        private void StopManagers()
        {
            Manager.StopNotificationManager();
            Manager.StopMessageManager();
            Manager.StopFileManager();
            Manager.StopTextMessageManager();
        }
    }
}
