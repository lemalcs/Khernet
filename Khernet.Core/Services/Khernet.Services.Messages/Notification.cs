using System.Runtime.Serialization;

namespace Khernet.Services.Messages
{
    /// <summary>
    /// The types of notification.
    /// </summary>
    [DataContract]
    public enum NotificationType : byte
    {
        /// <summary>
        /// Peer has changes its state.
        /// </summary>
        [EnumMember]
        PeerChange = 0,

        /// <summary>
        /// New message has arrived.
        /// </summary>
        [EnumMember]
        NewMessage = 1,

        /// <summary>
        /// Chat message has changes its state (<see cref="MessageState"/>)
        /// </summary>
        [EnumMember]
        MessageChange = 2,

        /// <summary>
        /// Peer is creating a message (<see cref="MessageProcessing"/>)
        /// </summary>
        [EnumMember]
        MessageProcessingChange = 3
    }

    /// <summary>
    /// The stages of message creation.
    /// </summary>
    [DataContract]
    public enum MessageProcessing
    {
        /// <summary>
        /// Peer is writing a text message.
        /// </summary>
        [EnumMember]
        WritingText = 0,

        /// <summary>
        /// Peer is sending a file message
        /// </summary>
        [EnumMember]
        BeginSendingFile = 1,

        /// <summary>
        /// Peer has finished sending a file message.
        /// </summary>
        [EnumMember]
        EndSendingFile = 2,
    }

    /// <summary>
    /// The types of change a peer can have.
    /// </summary>
    [DataContract]
    public enum PeerChangeType
    {
        /// <summary>
        /// Peer has changed its state (<see cref="PeerState"/>)
        /// </summary>
        [EnumMember]
        StateChange = 0,

        /// <summary>
        /// Peer has changed its profile.
        /// </summary>
        [EnumMember]
        ProfileChange = 1,

        /// <summary>
        /// Peer has changed its avatar.
        /// </summary>
        [EnumMember]
        AvatarChange = 2,
    }

    /// <summary>
    /// Base class for notifications.
    /// </summary>
    [DataContract]
    public abstract class BaseNotification
    {
        /// <summary>
        /// The unique id for notification.
        /// </summary>
        [DataMember]
        public string NotificationId { get; set; }
    }

    /// <summary>
    /// Holds information about changes of peer.
    /// </summary>
    [DataContract]
    public class PeerNotification : BaseNotification
    {
        /// <summary>
        /// Token of peer.
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// State of peer.
        /// </summary>
        [DataMember]
        public PeerState State { get; set; }

        /// <summary>
        /// The type of change of peer.
        /// </summary>
        [DataMember]
        public PeerChangeType Change { get; set; }
    }

    /// <summary>
    /// Holds information of new chat message.
    /// </summary>
    [DataContract]
    public class MessageNotification : BaseNotification
    {
        /// <summary>
        /// The Id of chat message.
        /// </summary>
        [DataMember]
        public int MessageId { get; set; }

        /// <summary>
        /// The state of message.
        /// </summary>
        [DataMember]
        public MessageState State { get; set; }

        /// <summary>
        /// The token of peer that sent message.
        /// </summary>
        [DataMember]
        public string SenderToken { get; set; }

        /// <summary>
        /// The format of message.
        /// </summary>
        [DataMember]
        public ContentType Format { get; set; }
    }

    /// <summary>
    /// Holds informations of current state of message.
    /// </summary>
    [DataContract]
    public class MessageStateNotification : BaseNotification
    {
        /// <summary>
        /// The Id of chat message.
        /// </summary>
        [DataMember]
        public int MessageId { get; set; }

        /// <summary>
        /// The state of message.
        /// </summary>
        [DataMember]
        public MessageState State { get; set; }
    }

    /// <summary>
    /// Holds information about current message creation process.
    /// </summary>
    [DataContract]
    public class MessageProcessingNotification : BaseNotification
    {
        /// <summary>
        /// The peer that is creating the message.
        /// </summary>
        [DataMember]
        public string SenderToken { get; set; }

        /// <summary>
        /// The current process of creation.
        /// </summary>
        [DataMember]
        public MessageProcessing Process { get; set; }
    }
}
