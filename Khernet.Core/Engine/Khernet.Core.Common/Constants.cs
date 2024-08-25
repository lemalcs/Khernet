namespace Khernet.Core.Common
{
    public static class Constants
    {
        public const string CommunicatorService = "COMMSRV";
        public const string FileService = "FILESRV";
        public const string SubscriberService = "SUSSRV";
        public const string PublisherService = "PUBSRV";
        public const string GatewayService = "GTWSRV";

        public const string TokenTag = "token";
        public const string ServiceIDTag = "serviceID";
        public const string PublickeyTag = "publickey";
        public const string PeerCertificateTag = "peerCertificate";
        public const string UserNameTag = "username";
        public const string AlternateTag = "altAddress";

        public const string Fingerprint = "FINGERPRINT";
        public const string ApplicationKey = "APPKEY";

        public const string ListenerKey = "LISTENKEY";

        public const string EntropyKey = "ENTROPY";
        public const string UserKey = "UKEY";
        public const string PasswordKey = "WKEY";

        public const short NewMessage = 0;
        public const int TextMessage = 0;
        public const int FileMessage = 1;

        public const int IPv4Address = 1;
        public const int IPv6Address = 2;

        /// <summary>
        /// The mode of to get updates for application. Values allowed:
        /// True: get updates online.
        /// False: get updates from a local file.
        /// </summary>
        public const string UpdateSource = "UpdateSource";
    }
}
