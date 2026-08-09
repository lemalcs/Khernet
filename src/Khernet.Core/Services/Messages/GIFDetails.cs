using System.Runtime.Serialization;

namespace Khernet.Services.Messages
{
    [DataContract]
    public class AnimationDetail
    {
        [DataMember]
        public int IdMessage { get; set; }

        [DataMember]
        public string IdFile { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        [DataMember]
        public byte[] Content { get; set; }
    }
}
