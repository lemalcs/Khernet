﻿using System;
using System.IO;
using System.ServiceModel;

namespace Khernet.Services.Messages
{
    [MessageContract]
    public class FileUploadRequestMessage : IDisposable
    {
        [MessageHeader(MustUnderstand = true)]
        public FileMessage FileMetaData { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream File { get; set; }

        #region Members of IDisposable

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
