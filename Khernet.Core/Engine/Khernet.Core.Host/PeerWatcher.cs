using Khernet.Core.Common;
using Khernet.Core.Entity;
using Khernet.Core.Processor;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Common;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Khernet.Core.Host
{
    internal class PeerWatcher : IDisposable
    {
        DiscoveryClient discoveryClient;

        private AnnouncementService announcementService;
        private ServiceHost announcementServiceHost;

        PeerIdentity identity;

        Thread stateMonitor;
        bool continueMonitor = false;

        ManualResetEvent manualReset;

        public PeerWatcher(PeerIdentity peerIdentity)
        {
            identity = peerIdentity;
        }
        public void Start()
        {
            //Set states of users to default: Offline
            Communicator comm = new Communicator();
            comm.ClearPeersState();

            OpenAnnoucementService();
            OpenDiscoveryClient();
            StartStateMonitor();
        }

        private void OpenDiscoveryClient()
        {
            try
            {
                discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());

                KhernetClientBehavior clienteBehavior = new KhernetClientBehavior();
                discoveryClient.Endpoint.Behaviors.Add(clienteBehavior);

                discoveryClient.FindProgressChanged += new EventHandler<FindProgressChangedEventArgs>(discoveryClient_FindProgressChanged);
                discoveryClient.FindCompleted += new EventHandler<FindCompletedEventArgs>(discoveryClient_FindCompleted);

                // Do async discovery
                discoveryClient.FindAsync(new FindCriteria(typeof(ICommunicator)));
                discoveryClient.FindAsync(new FindCriteria(typeof(IFileService)));
                discoveryClient.FindAsync(new FindCriteria(typeof(IEventNotifier)));
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        private void discoveryClient_FindProgressChanged(object sender, FindProgressChangedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                SavePeerAddress(e.EndpointDiscoveryMetadata, identity);
            });
        }

        private static void SavePeerAddress(EndpointDiscoveryMetadata metadata, PeerIdentity peer)
        {
            try
            {
                if (!AddressIsSelf(metadata, peer))
                {
                    AccountManager accountManager = new AccountManager();

                    CryptographyProvider crypto = new CryptographyProvider();
                    byte[] cert = crypto.DecodeBase58Check(metadata.Extensions.Elements(Constants.PeerCertificateTag).FirstOrDefault().Value);

                    X509Certificate certificate = new X509Certificate(cert);

                    string token = certificate.Subject.Substring(3, 34);
                    byte[] publicKey = certificate.GetPublicKey();

                    if (accountManager.ValidateToken(token, publicKey))
                    {
                        //Decode user name with BASE58Check
                        string userName = Encoding.UTF8.GetString(crypto.DecodeBase58Check(metadata.Extensions.Elements(Constants.UserNameTag).FirstOrDefault().Value));

                        //Address is not updated when a timeout error is raised on EventListenerService client side
                        //To reproduce error, generate an error timeout on ProcessContactChange event

                        string address = metadata.Extensions.Elements(Constants.AlternateTag).FirstOrDefault().Value;

                        string tempHost = GetActiveHostName(metadata.Address.Uri.Host, address, metadata.Address.Uri.Port);

                        string tempAddress = string.Format("{0}://{1}:{2}{3}",
                            metadata.Address.Uri.Scheme,
                            tempHost,
                            metadata.Address.Uri.Port,
                            metadata.Address.Uri.LocalPath) ?? metadata.Address.Uri.Host;


                        Communicator comm = new Communicator();
                        comm.SavePeer(
                        userName,
                        token,//Token of user
                        cert,//Public key of user
                        tempAddress,//Address of service
                        metadata.Extensions.Elements(Constants.ServiceIDTag).FirstOrDefault().Value//Type of service
                        );
                    }
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        /// <summary>
        /// Get the active IP address of given host.
        /// </summary>
        /// <param name="address">List of IPv4 and IPv6 addresses</param>
        /// <returns></returns>
        private static string GetActiveHostName(string hostName, string alternateAddresses, int port)
        {
            if (!string.IsNullOrEmpty(hostName) || !string.IsNullOrWhiteSpace(hostName))
            {
                if (NetworkHelper.TryConnectToHost(hostName, port))
                    return hostName;
            }

            if (string.IsNullOrEmpty(alternateAddresses) || string.IsNullOrWhiteSpace(alternateAddresses))
                return null;

            string[] addreessList = null;

            addreessList = alternateAddresses.Split('|');

            CryptographyProvider crypto = new CryptographyProvider();

            for (int i = 0; i < addreessList.Length; i++)
            {
                string[] addr = addreessList[i].Split(':');

                string tempAddr = Encoding.UTF8.GetString(crypto.DecodeBase58Check(addr[1]));

                if (NetworkHelper.TryConnectToIP(tempAddr, port))
                    return tempAddr;
            }

            return null;
        }

        private static void UpdatePeerState(EndpointDiscoveryMetadata metadata, PeerIdentity peer)
        {
            try
            {
                if (!AddressIsSelf(metadata, peer))
                {
                    CryptographyProvider crypto = new CryptographyProvider();

                    byte[] cert = crypto.DecodeBase58Check(metadata.Extensions.Elements(Constants.PeerCertificateTag).FirstOrDefault().Value);

                    X509Certificate certificate = new X509Certificate(cert);

                    AccountManager accountManager = new AccountManager();

                    string token = certificate.Subject.Substring(3, 34);
                    byte[] publicKey = certificate.GetPublicKey();
                    if (accountManager.ValidateToken(token, publicKey))
                    {
                        //Update user state on database
                        Communicator comm = new Communicator();
                        comm.UpdatePeerState(token, PeerState.Offline);//0: Offline
                    }
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        private static bool AddressIsSelf(EndpointDiscoveryMetadata metadata, PeerIdentity peer)
        {
            CryptographyProvider crypto = new CryptographyProvider();
            byte[] cert = crypto.DecodeBase58Check(metadata.Extensions.Elements(Constants.PeerCertificateTag).FirstOrDefault().Value);

            X509Certificate certificate = new X509Certificate(cert);

            if (certificate.Subject.Substring(3, 34) == peer.Token)
                return true;
            return false;
        }

        private void discoveryClient_FindCompleted(object sender, FindCompletedEventArgs e)
        {
            //Client discovery will be closed when this object was disposed
            if (e.UserState != null)
            {
                try
                {
                    List<PeerAddress> peerList = (List<PeerAddress>)e.UserState;
                    if (e.Result.Endpoints.Count > 0)
                    {
                        //Delete from user list with online state that were confirmed during discovery
                        CryptographyProvider crypto = new CryptographyProvider();

                        for (int i = 0; i < e.Result.Endpoints.Count; i++)
                        {
                            if (AddressIsSelf(e.Result.Endpoints[i], identity))//Skip local service
                                continue;

                            byte[] cert = crypto.DecodeBase58Check(e.Result.Endpoints[i].Extensions.Elements(Constants.PeerCertificateTag).FirstOrDefault().Value);

                            X509Certificate certificate = new X509Certificate(cert);

                            string token = certificate.Subject.Substring(3, 34);
                            PeerAddress peerAddr = peerList.Find((p) => { return p.Token == token; });

                            peerList.Remove(peerAddr);
                        }
                    }

                    if (peerList.Count > 0)
                    {
                        //Set user state to offline when they do not appear in discovery results
                        Communicator communicator = new Communicator();
                        peerList.ForEach((peer) =>
                        {
                            communicator.UpdatePeerState(peer.Token, PeerState.Offline);
                        });
                    }

                    LogDumper.WriteInformation("Discovery phase ended");
                }
                catch (Exception error)
                {
                    LogDumper.WriteLog(error);
                }
                finally
                {
                    manualReset.Set();
                }
            }
        }

        private void OpenAnnoucementService()
        {
            try
            {
                //Open announcement service
                announcementService = new AnnouncementService();
                announcementService.OnlineAnnouncementReceived += new EventHandler<AnnouncementEventArgs>(announcementService_OnlineAnnouncementReceived);
                announcementService.OfflineAnnouncementReceived += new EventHandler<AnnouncementEventArgs>(announcementService_OfflineAnnouncementReceived);

                announcementServiceHost = new ServiceHost(announcementService);
                announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());

                announcementServiceHost.Description.Behaviors.Add(new KhernetServiceInspectorBehavior());

                //Announcement service will send messages every 20 seconds when there were error on received messages
                //Error message:
                //El mensaje con To '' no se puede procesar en el destinatario debido a una falta de coincidencia de AddressFilter en EndpointDispatcher.
                //Compruebe que los valores de EndpointAddresses concuerdan en el remitente y el destinatario.

                //This happens when for example the remote application connects to local service with a IP address through NAT
                //Sometime it happens randomly for example when receipt and sender are executing on same machine

                KeyedByTypeCollection<IServiceBehavior> servB = announcementServiceHost.Description.Behaviors;
                foreach (IServiceBehavior beh in servB)
                {
                    if (beh is ServiceBehaviorAttribute)
                    {
                        ((ServiceBehaviorAttribute)beh).AddressFilterMode = AddressFilterMode.Any;
                    }

                }


                announcementServiceHost.BeginOpen(
                    (result) =>
                    {
                        announcementServiceHost.EndOpen(result);
                    },
                        null);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        private void StartStateMonitor()
        {
            stateMonitor = new Thread(new ThreadStart(ProbePeerState));
            continueMonitor = true;
            manualReset = new ManualResetEvent(false);
            stateMonitor.Start();
        }

        private void ProbePeerState()
        {
            Communicator communicator = new Communicator();
            while (continueMonitor)
            {
                try
                {
                    try
                    {
                        List<PeerAddress> tokenList = communicator.GetConnectedPeers(Constants.CommunicatorService);

                        bool existsDisconnected = false;
                        for (int i = 0; i < tokenList.Count; i++)
                        {
                            Uri addr = new Uri(tokenList[i].Address);
                            if (NetworkHelper.IsIPAddress(addr.Host))
                            {
                                if (!NetworkHelper.TryConnectToIP(addr.Host, addr.Port))
                                {
                                    existsDisconnected = true;
                                    break;
                                }
                            }
                            else if (!NetworkHelper.TryConnectToHost(addr.Host, addr.Port))
                            {
                                existsDisconnected = true;
                                break;
                            }
                        }

                        if (existsDisconnected)
                        {
                            discoveryClient.FindAsync(new FindCriteria(typeof(ICommunicator)), tokenList);
                            discoveryClient.FindAsync(new FindCriteria(typeof(IFileService)));
                            discoveryClient.FindAsync(new FindCriteria(typeof(IEventNotifier)));
                            manualReset.WaitOne();
                            manualReset.Reset();
                        }
                    }
                    catch (Exception error)
                    {
                        LogDumper.WriteLog(error);
                    }

                    Thread.Sleep(10000);
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                }
            }
        }

        private void announcementService_OnlineAnnouncementReceived(object sender, AnnouncementEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    //Cause there would exist other services that use announcement, it must be checked that found service
                    //implements the ICommunicator interface (contract)
                    if (MatchService(e.EndpointDiscoveryMetadata) && !AddressIsSelf(e.EndpointDiscoveryMetadata, identity))
                    {
                        //Guardar contacto en la base de datos
                        SavePeerAddress(e.EndpointDiscoveryMetadata, identity);
                    }
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                    throw;
                }
            });
        }

        private void announcementService_OfflineAnnouncementReceived(object sender, AnnouncementEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    //Cause there would exist other services that use announcement, it must be checked that found service
                    //implements the ICommunicator interface (contract)
                    if (MatchService(e.EndpointDiscoveryMetadata) && !AddressIsSelf(e.EndpointDiscoveryMetadata, identity))
                    {
                        UpdatePeerState(e.EndpointDiscoveryMetadata, identity);
                    }
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                    throw;
                }

            });
        }

        private static bool MatchService(EndpointDiscoveryMetadata metaData)
        {
            FindCriteria criteria = new FindCriteria(typeof(ICommunicator));
            bool result = criteria.IsMatch(metaData);

            if (!result)
            {
                criteria = new FindCriteria(typeof(IFileService));
                result = criteria.IsMatch(metaData);
            }
            if (!result)
            {
                criteria = new FindCriteria(typeof(IEventNotifier));
                result = criteria.IsMatch(metaData);
            }
            return result;
        }

        private void CloseDiscoveryClient()
        {
            try
            {
                if (discoveryClient != null)
                {
                    discoveryClient.Close();

                    discoveryClient.FindProgressChanged -= discoveryClient_FindProgressChanged;
                    discoveryClient.FindCompleted -= discoveryClient_FindCompleted;

                    discoveryClient = null;
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        private void StopStateMonitor()
        {
            try
            {
                if (stateMonitor != null && stateMonitor.ThreadState != ThreadState.Unstarted)
                {
                    continueMonitor = false;
                    stateMonitor.Interrupt();
                    if (!stateMonitor.Join(TimeSpan.FromMinutes(1)))
                    {
                        stateMonitor.Abort();
                    }
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        private void CloseAnnoucementService()
        {
            try
            {
                if (announcementServiceHost != null)
                {
                    announcementServiceHost.Close();
                    announcementService.OnlineAnnouncementReceived -= announcementService_OnlineAnnouncementReceived;
                    announcementService.OfflineAnnouncementReceived -= announcementService_OfflineAnnouncementReceived;
                }

            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                StopStateMonitor();
                CloseAnnoucementService();
                CloseDiscoveryClient();
                discoveryClient = null;

                announcementService = null;
                announcementServiceHost = null;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
            finally
            {
                if (manualReset != null)
                    manualReset.Close();
            }
        }

        #region IDisposable Support

        /// <summary>
        /// Variable to detect reentry calls
        /// </summary>
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Cleans resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
