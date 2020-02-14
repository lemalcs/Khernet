namespace Khernet.UI.Managers
{
    /// <summary>
    /// Informations to request operations to media files (video, audio)
    /// </summary>
    public class TextRequest : MessageRequest
    {
        /// <summary>
        /// The bytes of text message
        /// </summary>
        public byte[] Content { get; set; }
    }

    /// <summary>
    /// Information for media files
    /// </summary>
    public class TextResponse : MessageResponse
    {
        /// <summary>
        /// The bytes of text message
        /// </summary>
        public byte[] Content { get; set; }
    }
}
