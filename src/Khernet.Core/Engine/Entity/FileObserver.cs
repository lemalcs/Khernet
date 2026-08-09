using Khernet.Services.Messages;
using System;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace Khernet.Core.Entity
{
    public enum FileDirection
    {
        /// <summary>
        /// Indicates that file is being sent.
        /// </summary>
        Send = 0,

        /// <summary>
        /// Indicates that file is being received.
        /// </summary>
        Receive = 1,

        /// <summary>
        /// Indicates that file is being saved in local file system.
        /// </summary>
        Save = 2
    }

    public class ProgressChangedEventArgs : EventArgs
    {
        public long ReadBytes { get; private set; }
        public ProgressChangedEventArgs(long readBytes)
        {
            ReadBytes = readBytes;
        }
    }

    public class ReadFailedEventArgs : EventArgs
    {
        public Exception Error { get; private set; }
        public ReadFailedEventArgs(Exception exception)
        {
            Error = exception;
        }
    }

    public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);
    public delegate void ReadFailedEventHandler(object sender, ReadFailedEventArgs e);
    public delegate void ReadCompletedEventHandler(object sender);
    public class FileObserver : IObserver<long>
    {
        /// <summary>
        /// The id of message.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Metadata of file.
        /// </summary>
        public FileMessage Data { get; private set; }

        /// <summary>
        /// The full path of file on local file system.
        /// </summary>
        public string PhysicalFileName { get; set; }

        /// <summary>
        /// Thumbnail of file if there is one.
        /// </summary>
        public byte[] Thumbnail { get; set; }

        /// <summary>
        /// The operation done over file.
        /// </summary>
        public FileDirection Direction { get; set; }

        /// <summary>
        /// Event fired when file is being read.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Event fired there was an error while reading file.
        /// </summary>
        public event ReadFailedEventHandler ReadFailed;

        /// <summary>
        /// Event fired when file has been read completely.
        /// </summary>
        public event ReadCompletedEventHandler ReadCompleted;

        public FileObserver(FileMessage fileMessage)
        {
            Data = fileMessage;

            if (string.IsNullOrEmpty(fileMessage.Metadata.FileName))
                throw new ArgumentNullException("File name cannot be empty");
        }

        public Stream GetFileStream()
        {
            return File.Open(PhysicalFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        protected void OnProgressChanged(long readBytes)
        {
            ProgressChanged?.BeginInvoke(this, new Entity.ProgressChangedEventArgs(readBytes), CallBackProgress, "");
        }

        private void CallBackProgress(IAsyncResult result)
        {
            AsyncResult res = (AsyncResult)result;
            ProgressChangedEventHandler handler = (ProgressChangedEventHandler)res.AsyncDelegate;

            handler.EndInvoke(result);
        }

        protected void OnReadFailed(Exception exception)
        {
            ReadFailed?.Invoke(this, new ReadFailedEventArgs(exception));
        }

        protected void OnReadCompleted()
        {
            ReadCompleted?.Invoke(this);
        }

        public void OnNext(long value)
        {
            OnProgressChanged(value);
        }

        public void OnError(Exception error)
        {
            OnReadFailed(error);
        }

        public void OnCompleted()
        {
            OnReadCompleted();
        }
    }
}
