using Khernet.Core.Processor;
using Khernet.Services.Common;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false, AddressFilterMode = AddressFilterMode.Any)]
    [ServiceErrorBehavior(typeof(ServiceErrorHandler))]
    [KhernetServiceInspectorBehavior]
    public class GatewayService : IGateway
    {
        /// <summary>
        /// Get information to locate current user.
        /// </summary>
        /// <returns>The information to identify user as same as list of services that exposes.</returns>
        public PeerService GetPeerServiceInfo()
        {
            Gateway service = new Gateway();
            return service.GetPeerServiceInfo();
        }
    }
}
