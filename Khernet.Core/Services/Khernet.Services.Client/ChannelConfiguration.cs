using System.Security.Cryptography.X509Certificates;

namespace Khernet.Services.Client
{
    public class ServiceType
    {
        public const string CommunicatorService = "COMMSRV";
        public const string FileService = "FILESRV";
        public const string NotifierService = "NOTIFSRV";
        public const string SubscriberService = "SUSSRV";
        public const string PublisherService = "PUBSRV";
    }

    public interface IPeerFinder
    {
        string GetAddress(string token, string endPointType);
        X509Certificate2 GetCertificate(string token);
        bool ValidateToken(string token, byte[] publicKey);
    }
    public class ChannelConfiguration
    {
        public static IPeerFinder Finder { get; private set; }

        public static void SetFinder(IPeerFinder peerFinder)
        {
            Finder = peerFinder;
        }
        public static X509Certificate2 ClientCertificate { get; private set; }

        public static void SetClientCertificate(X509Certificate2 certificate)
        {
            ClientCertificate = certificate;
        }
    }
}
