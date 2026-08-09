using System;
using System.Runtime.Serialization;

namespace Khernet.Services.Messages
{
    [DataContract]
    public class FileInformation
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public long Size { get; set; }

        [DataMember]
        public string Identifier { get; set; }

        [DataMember]
        public string Format { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public TimeSpan Duration { get; set; }
    }
}
