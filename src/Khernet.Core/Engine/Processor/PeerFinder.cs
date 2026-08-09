using Khernet.Core.Data;
using Khernet.Services.Client;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Khernet.Core.Processor
{
    public class PeerFinder : IPeerFinder
    {
        public string GetAddress(string token, string endPointType)
        {
            CommunicatorData commData = new Data.CommunicatorData();
            DataTable data = commData.GetPeerServAddress(token, endPointType);
            if (data.Rows.Count > 0)
            {
                return data.Rows[0][0].ToString();
            }
            return null;
        }

        public X509Certificate2 GetCertificate(string token)
        {
            CommunicatorData commData = new Data.CommunicatorData();
            DataTable data = commData.GetPeerCertificate(token);
            if (data.Rows.Count > 0)
            {
                X509Certificate2 certificate = new X509Certificate2((byte[])data.Rows[0][0]);
                return certificate;
            }
            return null;
        }

        public bool ValidateToken(string token, byte[] publicKey)
        {
            AccountManager accountManager = new Processor.AccountManager();
            return accountManager.ValidateToken(token, publicKey);
        }
    }
}
