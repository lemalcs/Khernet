using System.Runtime.Serialization;

namespace Khernet.Services.Messages
{
    [DataContract]
    public class ServiceInfo
    {
        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string ServiceType { get; set; }
    }
}
