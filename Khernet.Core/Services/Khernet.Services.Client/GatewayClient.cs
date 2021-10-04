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
        public string Address { get; private set; }

        public GatewayClient(string address)
        {
            Address = address;
        }

        public PeerService GetPeerServiceInfo()
        {
            try
            {
                using (ServiceClient<IGateway> client = new ServiceClient<IGateway>(Address))
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
