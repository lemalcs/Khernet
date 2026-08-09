using System.Runtime.Serialization;

namespace Khernet.Services.Messages
{
    [DataContract]
    public enum ContentType
    {
        [EnumMember]
        Image = 0,

        [EnumMember]
        GIF = 1,

        [EnumMember]
        Video = 2,

        [EnumMember]
        Audio = 3,

        [EnumMember]
        Binary = 4,

        [EnumMember]
        Text = 6,

        [EnumMember]
        Html = 7,

        [EnumMember]
        Markdown = 8,

        [EnumMember]
        Contact = 9
    }


    [DataContract]
    public class FileMessage : ChatMessage
    {
        [DataMember]
        public FileInformation Metadata { get; set; }
    }


    public class InternalFileMessage : FileMessage
    {
        [DataMember]
        public int Id { get; set; }
    }
}
