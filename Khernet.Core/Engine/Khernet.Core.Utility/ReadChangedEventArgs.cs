using System;

namespace Khernet.Core.Utility
{
    public class ReadChangedEventArgs : EventArgs
    {
        public int ReadBytes { get; set; }
        public long Length { get; set; }
    }
}