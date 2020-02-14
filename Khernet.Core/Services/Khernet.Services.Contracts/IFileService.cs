using Khernet.Services.Messages;
using System.ServiceModel;
//using Umbreon.WCFServices.Common;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet.org", Name = "FileService")]
    public interface IFileService
    {
        //This method only can contains one parameter when it is used with Streaming transfer
        [OperationContract]
        //[FaultContract(typeof(FileServiceError))]
        //[FaultContract(typeof(ErrorInformation))]
        FileDownloadResponseMessage DownloadFile(FileDownloadRequestMessage file);

        //[OperationContract]
        ////[FaultContract(typeof(FileServiceError))]
        ////[FaultContract(typeof(ErrorInformation))]
        //void ProcessFile(ConversationMessage fileMessage);

        [OperationContract]
        ////[FaultContract(typeof(FileServiceError))]
        ////[FaultContract(typeof(ErrorInformation))]
        void UploadFile(FileUploadRequestMessage file);

        [OperationContract]
        //[FaultContract(typeof(FileServiceError))]
        //[FaultContract(typeof(ErrorInformation))]
        FileDownloadResponseMessage GetAvatar();
    }
}
