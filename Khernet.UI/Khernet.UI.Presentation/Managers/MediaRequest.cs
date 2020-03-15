using Khernet.UI.Media;
using System;
using System.IO;

namespace Khernet.UI.Managers
{
    public abstract class MessageRequest
    {
        /// <summary>
        /// The id of text message
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The token of sender
        /// </summary>
        public string SenderToken { get; set; }

        /// <summary>
        /// The token of receiver
        /// </summary>
        public string ReceiptToken { get; set; }

        /// <summary>
        /// The operation to perfomr over file according to <see cref="MessageOperation"/>
        /// </summary>
        public MessageOperation OperationRequest { get; set; }

        /// <summary>
        /// The contect of file according to <see cref="MessageType"/>
        /// </summary>
        public MessageType FileType { get; set; }

        /// <summary>
        /// The id of message being reply
        /// </summary>
        public int IdReplyMessage { get; set; }

        /// <summary>
        /// The universal identifier of message
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// The date message was sent
        /// </summary>
        public DateTimeOffset SendDate { get; set; }
    }

    public abstract class MessageResponse
    {
        /// <summary>
        /// The date the message was sent
        /// </summary>
        public DateTimeOffset SendDate { get; set; }

        /// <summary>
        /// The id of message that was replied
        /// </summary>
        public int IdReplyMessage { get; set; }

        /// <summary>
        /// The operation done over file
        /// </summary>
        public MessageOperation Operation { get; set; }

        /// <summary>
        /// The state of the message
        /// </summary>
        public ChatMessageState State { get; set; }

        /// <summary>
        /// The universal identifier of message
        /// </summary>
        public string UID { get; set; }
    }


    /// <summary>
    /// Informations to request operations to media files (video, audio)
    /// </summary>
    public class MediaRequest : MessageRequest
    {
        /// <summary>
        /// The path of original file to be processed
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The stream of file
        /// </summary>
        public Stream FileData { get; set; }

        /// <summary>
        /// Indicates whether the file is a GIF (Graphics Interchange Format)
        /// </summary>
        public bool IsGIF { get; set; }
    }

    /// <summary>
    /// Information for media files
    /// </summary>
    public class FileResponse : MessageResponse
    {
        /// <summary>
        /// The path of GIF file converted to AVI format
        /// </summary>
        public string ConvertedFileName { get; set; }

        /// <summary>
        /// The thumbnail path
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// The thumbnail bytes
        /// </summary>
        public byte[] ThumbnailBytes { get; set; }

        /// <summary>
        /// The width of media
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// The height of media
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// The duration media file
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// The original name of file
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// The path of file on local system
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The size of file in bytes
        /// </summary>
        public long Size { get; set; }

    }


    /// <summary>
    /// Operation to do on files
    /// </summary>
    public enum MessageOperation
    {
        /// <summary>
        /// Request to upload message
        /// </summary>
        Upload,

        /// <summary>
        /// Request to download message
        /// </summary>
        Download,

        /// <summary>
        /// Request to open file
        /// </summary>
        Open,

        /// <summary>
        /// Request to resend a message
        /// </summary>
        Resend,

        /// <summary>
        /// The metadata of file message
        /// </summary>
        GetMetadata
    }
}
