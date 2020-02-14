namespace Khernet.Core.Common
{
    public static class Constants
    {
        public const string CommunicatorService = "COMMSRV";
        public const string FileService = "FILESRV";
        public const string NotifierService = "NOTIFSRV";
        public const string SuscriberService = "SUSSRV";
        public const string PublisherService = "PUBSRV";

        public const string TokenTag = "token";
        public const string ServiceIDTag = "serviceID";
        public const string PublickeyTag = "publickey";
        public const string PeerCertificateTag = "peerCertificate";
        public const string UserNameTag = "username";
        public const string AlternateTag = "altAddress";

        public const string Fingerprint = "FINGERPRINT";
        public const string ApplicationKey = "APPKEY";

        public const string ListenerKey = "LISTENKEY";

        public const short NewMessage = 0;
        public const int TextMessage = 0;
        public const int FileMessage = 1;

        public const int IPv4Address = 1;
        public const int IPv6Address = 2;
    }
}
