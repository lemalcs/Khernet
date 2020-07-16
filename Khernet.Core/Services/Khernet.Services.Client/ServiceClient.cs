using Khernet.Core.Utility;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Khernet.Services.Client
{
    internal class ServiceClient<T> : IDisposable
    {
        public T serviceContract;


        internal ServiceClient(string token, string serviceType)
        {
            ChannelFactory<T> factory = new ChannelFactory<T>();

            Uri servAddress = new Uri(ChannelConfiguration.Finder.GetAddress(token, serviceType));

            X509Certificate2 certificate = ChannelConfiguration.Finder.GetCertificate(token);
            factory.Endpoint.Address = new EndpointAddress(servAddress, EndpointIdentity.CreateDnsIdentity(certificate.Subject.Substring(3, 34)));

            factory.Endpoint.Binding = GetBinding(serviceType);

            //Set credentials to auhtenticate server to clients
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            factory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new CertificateValidator();

            //Set credentials to authenticate client to server
            factory.Credentials.ClientCertificate.Certificate = ChannelConfiguration.ClientCertificate;

            serviceContract = factory.CreateChannel();
        }

        internal ServiceClient(Binding bind, string address)
        {
            serviceContract = ChannelFactory<T>.CreateChannel(bind, new EndpointAddress(address));
        }

        private Binding GetBinding(string serviceType)
        {
            Binding bind = null;
            switch (serviceType)
            {
                case ServiceType.CommunicatorService:

                    NetTcpBinding tcpBind = new NetTcpBinding();
                    tcpBind.TransferMode = TransferMode.Buffered;
                    tcpBind.CloseTimeout = TimeSpan.MaxValue;
                    tcpBind.ReceiveTimeout = TimeSpan.MaxValue;
                    tcpBind.SendTimeout = TimeSpan.MaxValue;
                    tcpBind.MaxReceivedMessageSize = int.MaxValue;
                    tcpBind.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                    tcpBind.Security.Mode = SecurityMode.Message;
                    tcpBind.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;

                    bind = tcpBind;
                    break;
                case ServiceType.NotifierService:

                    NetTcpBinding binding = new NetTcpBinding();
                    binding.TransferMode = TransferMode.Buffered;
                    binding.MaxReceivedMessageSize = int.MaxValue;
                    binding.CloseTimeout = TimeSpan.MaxValue;
                    binding.ReceiveTimeout = TimeSpan.MaxValue;
                    binding.SendTimeout = TimeSpan.MaxValue;
                    binding.Security.Mode = SecurityMode.Message;
                    binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;

                    bind = binding;
                    break;
                case ServiceType.FileService:

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


                    bind = fileBinding;
                    break;
                default:
                    NetNamedPipeBinding pipeBind = new NetNamedPipeBinding();
                    pipeBind.TransferMode = TransferMode.Buffered;
                    pipeBind.SendTimeout = TimeSpan.MaxValue;
                    pipeBind.MaxReceivedMessageSize = int.MaxValue;
                    pipeBind.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                    bind = pipeBind;
                    break;
            }

            return bind;
        }

        public void Dispose()
        {
            try
            {
                if (serviceContract != null)
                    ((ICommunicationObject)serviceContract).Close();
            }
            catch (Exception ex)
            {
                LogDumper.WriteLog(ex);
                ((ICommunicationObject)serviceContract).Abort();
            }
        }
    }
}
