using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Cryptography;

namespace Khernet.Core.Utility
{
    public delegate void ReadChangedHandler(ReadChangedEventArgs e);
    public class DataStream : Stream
    {
        private Stream file;
        private int readBytes;

        public event ReadChangedHandler ReadChanged;

        public DataStream(FileStream file)
        {
            this.file = file;
            readBytes = 0;
        }

        public DataStream(Stream file)
        {
            this.file = file;
            readBytes = 0;
        }

        public DataStream(Stream file, SecureString password, bool encrypt)
        {
            RijndaelManaged rm = new RijndaelManaged();
            CryptographyProvider crypProv = new CryptographyProvider();
            byte[] key = crypProv.GenerateKey(password, 32);
            byte[] vector = crypProv.GenerateKey(password, 16);
            CryptoStream crypt = null;
            if (encrypt)
                crypt = new CryptoStream(
                    file,
                    rm.CreateEncryptor(key, vector),
                    CryptoStreamMode.Read);
            else
                crypt = new CryptoStream(
                    file,
                    rm.CreateDecryptor(key, vector),
                    CryptoStreamMode.Read);
            this.file = crypt;
            readBytes = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = file.Read(buffer, offset, count);
            readBytes += result;
            OnReadChanged(new ReadChangedEventArgs { ReadBytes = readBytes });
            return result;
        }

        private void CallBackProgress(IAsyncResult result)
        {
            AsyncResult res = (AsyncResult)result;
            ReadChangedHandler handler = (ReadChangedHandler)res.AsyncDelegate;

            handler.EndInvoke(result);
        }

        protected void OnReadChanged(ReadChangedEventArgs e)
        {
            ReadChanged?.BeginInvoke(e, CallBackProgress, "");
        }

        public override bool CanRead
        {
            get { return file.CanRead; }
        }

        public override bool CanSeek
        {
            get { return file.CanRead; }
        }

        public override bool CanWrite
        {
            get { return file.CanWrite; }
        }

        public override void Flush()
        {
            if (file.CanWrite)
                file.Flush();
        }

        public override long Length
        {
            get { return file.Length; }
        }

        public override long Position
        {
            get
            {
                return readBytes;
            }
            set
            {
                this.Position = value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return file.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            file.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            file.Write(buffer, offset, count);
        }

        public override void Close()
        {
            file.Close();
            base.Close();
        }
    }
}