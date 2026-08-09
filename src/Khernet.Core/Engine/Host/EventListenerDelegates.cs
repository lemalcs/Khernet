using Khernet.Services.Messages;
using System;

namespace Khernet.Core.Host
{
    public delegate void ContactChangedEventHandler(object sender, ContactChangedEventArgs e);

    public delegate void MessageProcessingHandler(object sender, MessageProcessingEventArgs e);

    public delegate void MessageArrivedEventHandler(object sender, MessageArrivedEventArgs e);

    public delegate void MessageStateChangedEventHandler(object sender, MessageStateChangedEventArgs e);

    public class ContactChangedEventArgs : EventArgs
    {
        public PeerNotification EventInformation { get; private set; }
        public ContactChangedEventArgs(PeerNotification eventInformation)
        {
            EventInformation = eventInformation;
        }
    }

    public class MessageProcessingEventArgs : EventArgs
    {
        public MessageProcessingNotification Notification { get; private set; }
        public MessageProcessingEventArgs(MessageProcessingNotification notification)
        {
            Notification = notification;
        }
    }

    public class MessageStateChangedEventArgs : EventArgs
    {
        public MessageStateNotification Notification { get; private set; }
        public MessageStateChangedEventArgs(MessageStateNotification notification)
        {
            Notification = notification;
        }
    }

    public class MessageArrivedEventArgs : EventArgs
    {
        public MessageNotification Notification { get; private set; }
        public MessageArrivedEventArgs(MessageNotification notification)
        {
            Notification = notification;
        }
    }
}
