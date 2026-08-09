using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet.org", Name = "FileService")]
    public interface IFileService
    {
        //This method only can contains one parameter when it is used with Streaming transfer.
        [OperationContract]
        FileDownloadResponseMessage DownloadFile(FileDownloadRequestMessage file);

        [OperationContract]
        void UploadFile(FileUploadRequestMessage file);

        [OperationContract]
        FileDownloadResponseMessage GetAvatar();
    }
}
