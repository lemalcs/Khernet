using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;

namespace Khernet.Services.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class FileService : IFileService
    {
        public FileDownloadResponseMessage DownloadFile(FileDownloadRequestMessage file)
        {
            return null;
        }

        public void UploadFile(FileUploadRequestMessage file)
        {
            
        }
    }
}
