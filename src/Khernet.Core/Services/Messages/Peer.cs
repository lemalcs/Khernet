using System.Runtime.Serialization;

namespace Khernet.Services.Messages
{
    [DataContract]
    public enum PeerState : sbyte
    {
        [EnumMember]
        New = -1,

        [EnumMember]
        Offline = 0,

        [EnumMember]
        Idle = 1,

        [EnumMember]
        Online = 2,

        [EnumMember]
        Busy = 3
    }


    [DataContract]
    public class Peer
    {
        [DataMember]
        public string AccountToken { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string HexColor { get; set; }

        [DataMember]
        public string Initials { get; set; }

        [DataMember]
        public string Slogan { get; set; }

        [DataMember]
        public byte[] Avatar { get; set; }

        [DataMember]
        public PeerState State { get; set; }

        [DataMember]
        public string Group { get; set; }

        [DataMember]
        public byte[] FullName { get; set; }

        [DataMember]
        public byte[] DisplayName { get; set; }
    }
}
