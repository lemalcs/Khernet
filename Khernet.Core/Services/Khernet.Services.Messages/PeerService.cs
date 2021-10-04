using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Khernet.Services.Messages
{
    [DataContract]
    public class PeerService
    {
        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Certificate { get; set; }

        [DataMember]
        public List<ServiceInfo> ServiceList { get; set; }
    }
}
