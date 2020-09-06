using Khernet.UI.Resources;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Khernet.UI.Media
{
    /// <summary>
    /// The content of file
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// File is an image
        /// </summary>
        Image = 0,

        /// <summary>
        /// File is a GIF (Graphics Interchangge Format)
        /// </summary>
        GIF = 1,

        /// <summary>
        /// File is a video
        /// </summary>
        Video = 2,

        /// <summary>
        /// File is audio
        /// </summary>
        Audio = 3,

        /// <summary>
        /// Other type of file
        /// </summary>
        Binary = 4,

        /// <summary>
        /// Data inside Stream
        /// </summary>
        StreamData = 5,

        /// <summary>
        /// Text data
        /// </summary>
        Text = 6,

        /// <summary>
        /// Html text message
        /// </summary>
        Html = 7,

        /// <summary>
        /// Markdown text message
        /// </summary>
        Markdown = 8
    }

    /// <summary>
    /// Unit of quantity of bytes
    /// </summary>
    public enum ByteUnit
    {
        /// <summary>
        /// Unit of bytes
        /// </summary>
        Bytes = 0,

        /// <summary>
        /// Kilobyte
        /// </summary>
        KB = 1,

        /// <summary>
        /// Megabyte
        /// </summary>
        MB = 2,

        /// <summary>
        /// Gigabyte
        /// </summary>
        GB = 3,

        /// <summary>
        /// Terabyte
        /// </summary>
        TB = 4
    }
    public class FileHelper
    {
        /// <summary>
        /// The size of 1 kylobyte
        /// </summary>
        public const long KILOBYTE = 1024;

        /// <summary>
        /// The size of 1 megabyte
        /// </summary>
        public const long MEGABYTE = 1048576;

        /// <summary>
        /// The size of 1 gigabyte
        /// </summary>
        public const long GIGABYTE = 1073741824;

        /// <summary>
        /// The size of 1 gigabyte
        /// </summary>
        public const long TERABYTE = 1099511627776;

        /// <summary>
        /// The defualt file type type id cannot be determined.
        /// </summary>
        public const string DefaultFileType = "application/octet-stream";

        /// <summary>
        /// Determines the file type reading the first 256 bytes.
        /// </summary>
        /// <param name="fileName">The path of file</param>
        /// <returns>Rerturns the MIME type of file, if file is empty returns null</returns>
        private static string ScanFileForMimeType(string fileName)
        {
            try
            {
                using (FileStream fs = File.OpenRead(fileName))
                {
                    string mimeout = String.Empty;
                    int MaxContent = (int)new FileInfo(fileName).Length;

                    //Number of bytes to read
                    if (MaxContent > 256)
                        MaxContent = 256;

                    byte[] buf = new byte[MaxContent];

                    try
                    {
                        fs.Read(buf, 0, MaxContent);
                        int result = NativeMethods.FindMimeFromData(IntPtr.Zero, fileName, buf, MaxContent, null, 0, out mimeout, 0);

                        //The was an error reading file
                        if (result != 0)
                            throw Marshal.GetExceptionForHR(result);
                    }
                    catch (Exception ex) { Debug.WriteLine(ex); }

                    return mimeout;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the MIME type from file
        /// </summary>
        /// <param name="fileName">The path of file</param>
        /// <returns></returns>
        public static MessageType GetContentType(string fileName)
        {
            string content = ScanFileForMimeType(fileName);

            if (!string.IsNullOrEmpty(content) && content == MimeTypes.Gif)
                return MessageType.GIF;

            if (!string.IsNullOrEmpty(content) && content.StartsWith("image/"))
                return MessageType.Image;

            if (!string.IsNullOrEmpty(content) && content.StartsWith("video/"))
                return MessageType.Video;

            if (!string.IsNullOrEmpty(content) && content.StartsWith("audio/"))
                return MessageType.Audio;

            return MessageType.Binary;
        }

        /// <summary>
        /// Get length of file in bytes.
        /// </summary>
        /// <param name="fileName">The path of file</param>
        /// <returns></returns>
        public static long GetFileSize(string fileName)
        {
            FileInfo finfo = new FileInfo(fileName);
            return finfo.Length;
        }

        /// <summary>
        /// Get a new file name appending a sequential number to original name
        /// </summary>
        /// <param name="fileName">The path of file</param>
        /// <returns></returns>
        public static string GetNewFileName(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    string newFileName = fileName;
                    for (int counter = 0; File.Exists(newFileName);)
                    {
                        counter++;
                        newFileName = Path.Combine(
                            Path.GetDirectoryName(fileName),//Ruta del archivo
                            string.Format("{0} ({1}){2}",
                                            Path.GetFileNameWithoutExtension(fileName), //nombre del archivo
                                            counter,//Contador para nuevo nombre de archivo
                                            Path.GetExtension(fileName))//extensión del archivo
                            );
                    }
                    return newFileName;
                }
                return fileName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Builds a file path with given extension
        /// </summary>
        /// <param name="fileName">The path of file</param>
        /// <param name="extension">The extension of file</param>
        /// <returns></returns>
        public static string GetFileNameWithExtension(string fileName, string extension)
        {
            try
            {
                return Path.Combine(Path.GetDirectoryName(fileName), string.Format("{0}.{1}", Path.GetFileNameWithoutExtension(fileName), extension));
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Convert number to smallest size based on <see cref="ByteUnit"/> units
        /// </summary>
        /// <param name="size">The value size to convert</param>
        /// <returns>The value converted to supported units based on <see cref="ByteUnit"/> </returns>
        public static double ConvertToSmallestSize(long size)
        {
            if (size <= 0 ||
                size < KILOBYTE ||
                size > TERABYTE)
                return size;

            if (size < MEGABYTE)
            {
                return (double)size / (double)KILOBYTE;
            }
            else if (size < GIGABYTE)
            {
                return (double)size / (double)MEGABYTE;
            }
            return (double)size / (double)GIGABYTE;
        }

        /// <summary>
        /// Get the smallest unit name from byte to terabyte
        /// </summary>
        /// <param name="size">The number of bytes</param>
        /// <returns></returns>
        public static ByteUnit GetSmallestUnit(long size)
        {
            if (size <= 0 ||
               size < KILOBYTE ||
               size > TERABYTE)
                return ByteUnit.Bytes;

            if (size < MEGABYTE)
            {
                return ByteUnit.KB;
            }
            else if (size < GIGABYTE)
            {
                return ByteUnit.MB;
            }
            else if (size < TERABYTE)
            {
                return ByteUnit.GB;
            }
            return ByteUnit.TB;
        }

        public static bool CompareFileWithMemory(string sourceFile, byte[] fileBytes)
        {
            if (File.Exists(sourceFile) && (fileBytes != null && fileBytes.Length > 0))
            {
                FileInfo fileDetail = new FileInfo(sourceFile);
                if (fileBytes.Length == fileDetail.Length)
                {
                    using (MemoryStream dtStream = new MemoryStream(fileBytes))
                    {
                        int chunk = 524288;
                        byte[] buffer = new byte[chunk];

                        int readBytesOriginalFile = 0;
                        using (FileStream fStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            readBytesOriginalFile = dtStream.Read(buffer, 0, chunk);

                            byte[] cacheBuffer = new byte[buffer.Length];

                            int readBytesCacheFile = fStream.Read(cacheBuffer, 0, cacheBuffer.Length);

                            while (readBytesOriginalFile > 0 && readBytesCacheFile > 0)
                            {
                                if (AreEqualArray(buffer, cacheBuffer))
                                {
                                    readBytesOriginalFile = dtStream.Read(buffer, 0, chunk);
                                    readBytesCacheFile = fStream.Read(cacheBuffer, 0, cacheBuffer.Length);
                                }
                                else
                                    return false;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if two byte arrays are equal
        /// </summary>
        /// <param name="array1">The fisrt array to be compared</param>
        /// <param name="array2">The second array to be compared</param>
        /// <returns></returns>
        public static bool AreEqualArray(byte[] array1, byte[] array2)
        {
            if (array1 == null && array2 == null)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            bool areEqual = array1.Length == array2.Length;

            for (int i = 0; i < array1.Length; i++)
            {
                areEqual &= array1[i] == array2[i];
                if (!areEqual)
                    return areEqual;
            }

            return areEqual;
        }
    }
}
