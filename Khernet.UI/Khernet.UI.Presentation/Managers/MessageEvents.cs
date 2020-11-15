using System;
using System.Diagnostics;

namespace Khernet.UI.Managers
{
    public enum MessageEvent
    {
        /// <summary>
        /// Message is staring to be written.
        /// </summary>
        BeginWriting = 0,

        /// <summary>
        /// Message has finished written.
        /// </summary>
        EndWriting = 1,

        /// <summary>
        /// File is starting to be sent.
        /// </summary>
        BeginSendingFile = 2,

        /// <summary>
        /// File has finished sending.
        /// </summary>
        EndSendingFile = 3
    }

    public class MessageEventData
    {
        /// <summary>
        /// The type action that is being performing when creating a message.
        /// </summary>
        public MessageEvent EventType { get; set; }

        /// <summary>
        /// The peer that is creating the message.
        /// </summary>
        public string SenderPeer { get; set; }

        /// <summary>
        /// The peer that receive the event.
        /// </summary>
        public string ReceiverPeer { get; set; }

        /// <summary>
        /// The date when this event arrived to UI.
        /// </summary>
        public DateTime ArriveDate { get; set; }
    }
}
