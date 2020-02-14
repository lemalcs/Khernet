using System.ServiceModel;

namespace Khernet.Services.Messages
{
    [MessageContract]
    public class FileDownloadRequestMessage
    {
        /// <summary>
        /// The information about requested file
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public FileMessage Description { get; set; }
    }
}
