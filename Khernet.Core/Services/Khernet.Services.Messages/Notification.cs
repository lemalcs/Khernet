using System;
using System.Runtime.Serialization;

namespace Khernet.Services.Messages
{
    [DataContract]
    public enum NotificationType : byte
    {
        [EnumMember]
        StateChange = 0,

        [EnumMember]
        ProfileChange = 1,

        [EnumMember]
        AvatarChange = 2,

        [EnumMember]
        NewMessage = 3,

        [EnumMember]
        NewFile = 4,

        [EnumMember]
        WritingMessage = 5,

        [EnumMember]
        BeginSendingFile = 6,

        [EnumMember]
        EndSendingFile = 7,

        [EnumMember]
        ReadingFile = 8,

        [EnumMember]
        MessageSent = 9
    }


    [DataContract]
    public class Notification
    {
        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public DateTime ArriveDate { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public NotificationType Type { get; set; }
    }
}
