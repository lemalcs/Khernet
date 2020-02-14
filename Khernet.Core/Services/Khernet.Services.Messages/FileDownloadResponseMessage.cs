using System;
using System.IO;
using System.ServiceModel;

namespace Khernet.Services.Messages
{
    [MessageContract]
    public class FileDownloadResponseMessage : IDisposable
    {
        [MessageBodyMember(Order = 1)]
        public Stream File { get; set; }

        #region Miembros de IDisposable

        public void Dispose()
        {
            if (File != null)
            {
                File.Close();
                File = null;
            }
        }

        #endregion
    }
}
