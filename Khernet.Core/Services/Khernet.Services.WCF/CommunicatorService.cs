using Khernet.Core.Processor;
using Khernet.Services.Common;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System.ServiceModel;

namespace Khernet.Services.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false, AddressFilterMode = AddressFilterMode.Any)]
    [ServiceErrorBehavior(typeof(ServiceErrorHandler))]
    [KhernetServiceInspectorBehavior]
    public class CommunicatorService : ICommunicator, IFileService
    {
        public FileDownloadResponseMessage GetAvatar()
        {
            FileCommunicator FileCommunicator = new FileCommunicator();
            return FileCommunicator.GetSelfAvatar();
        }

        public Peer GetProfile()
        {
            Communicator comm = new Communicator();
            return comm.GetSelfProfile();
        }

        public FileDownloadResponseMessage DownloadFile(FileDownloadRequestMessage file)
        {
            FileCommunicator FileCommunicator = new FileCommunicator();
            return FileCommunicator.DownloadFile(file);
        }

        public void UploadFile(FileUploadRequestMessage file)
        {
            FileCommunicator FileCommunicator = new FileCommunicator();
            FileCommunicator.UploadFile(file);
        }

        public void ProcessTextMessage(ConversationMessage message)
        {
            Communicator comm = new Communicator();
            comm.ProcessTextMessage(message);
        }
    }
}
