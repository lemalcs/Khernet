using Khernet.Services.Messages;
using System;

namespace Khernet.Core.Host
{
    public delegate void ContactChangedEventHandler(object sender, ContactChangedEventArgs e);
    public delegate void WritingMessageEventHandler(object sender, WritingMessageEventArgs e);
    public delegate void SendingFileEventHandler(object sender, SendingFileEventArgs e);
    public delegate void MessageArrivedEventHandler(object sender, MessageArrivedEventArgs e);
    public delegate void FileArrivedEventHandler(object sender, FileArrivedEventArgs e);
    public delegate void ReadingFileEventHandler(object sender, ReadingFileEventArgs e);
    public delegate void MessageSentEventHandler(object sender, MessageSentEventArgs e);

    public class ContactChangedEventArgs : EventArgs
    {
        public Notification EventInformation { get; private set; }
        public ContactChangedEventArgs(Notification eventInformation)
        {
            EventInformation = eventInformation;
        }
    }

    public class WritingMessageEventArgs : EventArgs
    {
        public string AccountToken { get; private set; }
        public WritingMessageEventArgs(string accountToken)
        {
            AccountToken = accountToken;
        }
    }

    public class SendingFileEventArgs : EventArgs
    {
        public string AccountToken { get; private set; }
        public SendingFileEventArgs(string accountToken)
        {
            AccountToken = accountToken;
        }
    }

    public class MessageArrivedEventArgs : EventArgs
    {
        public InternalConversationMessage Message { get; private set; }
        public MessageArrivedEventArgs(InternalConversationMessage message)
        {
            Message = message;
        }
    }

    public class FileArrivedEventArgs : EventArgs
    {
        public InternalFileMessage File { get; private set; }
        public FileArrivedEventArgs(InternalFileMessage file)
        {
            File = file;
        }
    }

    public class ReadingFileEventArgs : EventArgs
    {
        public string Token { get; private set; }
        public string IdFile { get; private set; }
        public string ReadBytes { get; private set; }
        public ReadingFileEventArgs(string token, string idFile, long readBytes)
        {
            Token = token;
            IdFile = idFile;
            ReadBytes = ReadBytes;
        }
    }

    public class MessageSentEventArgs : EventArgs
    {
        public string Token { get; private set; }
        public int IdMessage { get; private set; }

        public MessageSentEventArgs(string token, int idMessage)
        {
            Token = token;
            IdMessage = idMessage;
        }
    }
}
