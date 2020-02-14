using Khernet.Core.Utility;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System;

namespace Khernet.Services.Client
{
    public class CommunicatorClient
    {
        public string Address { get; private set; }

        public CommunicatorClient(string address)
        {
            Address = address;
        }

        public Peer GetProfile()
        {
            try
            {
                using (ServiceClient<ICommunicator> client = new ServiceClient<ICommunicator>(Address, ServiceType.CommunicatorService))
                {

                    return client.serviceContract.GetProfile();
                }
            }
            catch (Exception ex)
            {
                LogDumper.WriteLog(ex);
                throw ex;
            }
        }

        public void ProcessTextMessage(ConversationMessage message)
        {
            try
            {
                using (ServiceClient<ICommunicator> client = new ServiceClient<ICommunicator>(Address, ServiceType.CommunicatorService))
                {
                    client.serviceContract.ProcessTextMessage(message);
                }
            }
            catch (Exception ex)
            {
                LogDumper.WriteLog(ex);
                throw ex;
            }
        }

        public FileDownloadResponseMessage DownloadFile(FileDownloadRequestMessage file)
        {
            try
            {
                using (ServiceClient<IFileService> client = new ServiceClient<IFileService>(Address, ServiceType.FileService))
                {
                    return client.serviceContract.DownloadFile(file);
                }
            }
            catch (Exception ex)
            {
                LogDumper.WriteLog(ex);
                throw ex;
            }
        }

        public FileDownloadResponseMessage GetAvatar()
        {
            try
            {
                using (ServiceClient<IFileService> client = new ServiceClient<IFileService>(Address, ServiceType.FileService))
                {
                    return client.serviceContract.GetAvatar();
                }
            }
            catch (Exception ex)
            {
                LogDumper.WriteLog(ex);
                throw ex;
            }
        }

        public void UploadFile(FileUploadRequestMessage file)
        {
            try
            {
                using (ServiceClient<IFileService> client = new ServiceClient<IFileService>(Address, ServiceType.FileService))
                {
                    client.serviceContract.UploadFile(file);
                }
            }
            catch (Exception ex)
            {
                LogDumper.WriteLog(ex);
                throw ex;
            }
        }
    }
}
