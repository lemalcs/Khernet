using System.Security.Cryptography.X509Certificates;

namespace Khernet.Core.Entity
{
    public class PeerIdentity
    {
        public string UserName { get; private set; }
        public string Token { get; private set; }
        public string PublicKey { get; private set; }

        public X509Certificate2 Certificate { get; private set; }

        public PeerIdentity(string userName, string token, X509Certificate2 certificate)
        {
            UserName = userName;
            Token = token;
            Certificate = certificate;
        }
    }

    public class PeerAddress
    {
        public string Token { get; private set; }
        public string Address { get; set; }

        public PeerAddress(string token, string address)
        {
            Token = token;
            Address = address;
        }
    }
}
