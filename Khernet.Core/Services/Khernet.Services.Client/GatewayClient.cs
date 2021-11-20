using Khernet.Core.Utility;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khernet.Services.Client
{
    public class GatewayClient
    {
        public Uri Address { get; private set; }
        public string Token { get; private set; }

        public GatewayClient(string token, string address)
        {
            Address = new Uri(address);
            Token = token;
        }

        public PeerService GetPeerServiceInfo()
        {
            try
            {
                using (ServiceClient<IGateway> client = new ServiceClient<IGateway>(Token, Address))
                {

                    return client.serviceContract.GetPeerServiceInfo();
                }
            }
            catch (Exception ex)
            {
                LogDumper.WriteLog(ex);
                throw ex;
            }
        }
    }
}
