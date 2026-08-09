using System;
using System.Runtime.Serialization;

namespace Khernet.Services.Messages
{
    [DataContract]
    public enum MessageState
    {
        /// <summary>
        /// The message is not ready to be used.
        /// </summary>
        [EnumMember]
        UnCommited = -1,

        /// <summary>
        /// The message is pending to sent.
        /// </summary>
        [EnumMember]
        Pending = 0,

        /// <summary>
        /// The message was sent or received successfully.
        /// </summary>
        [EnumMember]
        Processed = 1,

        /// <summary>
        /// There was and error while sending or receiving the message.
        /// </summary>
        [EnumMember]
        Error = 2
    }


    [DataContract]
    public abstract class ChatMessage
    {
        /// <summary>
        /// The token of sender of message.
        /// </summary>
        [DataMember]
        public string SenderToken { get; set; }

        /// <summary>
        /// The token of receiver of message.
        /// </summary>
        [DataMember]
        public string ReceiverToken { get; set; }

        /// <summary>
        /// The date that message was sent.
        /// </summary>
        [DataMember]
        public DateTimeOffset SendDate { get; set; }

        /// <summary>
        /// The number of ticks (100 nanoseconds) since January 1, 0001 in the Gregorian calendar.
        /// </summary>
        [DataMember]
        public long TimeId { get; set; }

        /// <summary>
        /// Unique identifier for message, this value will be known by sender and receiver.
        /// </summary>
        [DataMember]
        public string UID { get; set; }

        /// <summary>
        /// The UID of message being replied.
        /// </summary>
        [DataMember]
        public string UIDReply { get; set; }

        /// <summary>
        /// The type of message.
        /// </summary>
        [DataMember]
        public ContentType Type { get; set; }

        /// <summary>
        /// The state of the message.
        /// </summary>
        [DataMember]
        public MessageState State { get; set; }
    }


    [DataContract]
    public class TextMessage : ChatMessage
    {
        [DataMember]
        public byte[] RawContent { get; set; }
    }


    [DataContract]
    public class ConversationMessage : TextMessage
    {
        /// <summary>
        /// Number that identifies the order of chunks when message is splitted.
        /// </summary>
        [DataMember]
        public int Sequential { get; set; }

        /// <summary>
        /// Indicates the total number of chunks for this message.
        /// </summary>
        [DataMember]
        public int TotalChunks { get; set; }
    }


    [DataContract]
    public class InternalConversationMessage : ConversationMessage
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int IdReply { get; set; }
    }

    /// <summary>
    /// Metadata for chat messages.
    /// </summary>
    [DataContract]
    public class MessageItem
    {
        /// <summary>
        /// The id for message.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The peer that sent message.
        /// </summary>
        [DataMember]
        public int IdSenderPeer { get; set; }

        /// <summary>
        /// The type of message.
        /// </summary>
        [DataMember]
        public ContentType Format { get; set; }

        /// <summary>
        /// The date when message was sent or received.
        /// </summary>
        [DataMember]
        public DateTimeOffset RegisterDate { get; set; }

        /// <summary>
        /// The state of chat message.
        /// </summary>
        [DataMember]
        public MessageState State { get; set; }

        /// <summary>
        /// Indicates if this message was read.
        /// </summary>
        [DataMember]
        public bool IsRead { get; set; }

        /// <summary>
        /// The universal identifier of message.
        /// </summary>
        [DataMember]
        public string UID { get; set; }

        /// <summary>
        /// The number of ticks that indicates when message was sent.
        /// </summary>
        [DataMember]
        public long TimeId { get; set; }
    }
}
