using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Khernet.UI
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct BITMAPFILEHEADER
    {
        //This variable must be BM for bitmap images
        public static readonly short BM = 0x4D42;

        public short bfType;
        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }


    public static class ImageHelper
    {
        /// <summary>
        /// Gets an <see cref="ImageSource"/> from clipboard.
        /// </summary>
        /// <returns>Image represented by <see cref="ImageSource"/>.</returns>
        public static ImageSource GetImageFromClipboard()
        {
            return GetImageFromStream(Clipboard.GetData(DataFormats.Dib) as MemoryStream);
        }

        /// <summary>
        /// Reads an image from file and returns a <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="filePath">The path of image file.</param>
        /// <returns></returns>
        public static ImageSource GetImageFromFile(string filePath)
        {
            return GetImageFromStream(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        /// <summary>
        /// Reads an image from <see cref="Stream"/> source.
        /// </summary>
        /// <param name="imageStream">The <see cref="Stream"/> that contains the image.</param>
        /// <returns>The <see cref="ImageSource"/> object containing the image.</returns>
        public static ImageSource GetImageFromStream(Stream imageStream)
        {
            //Check if there is an image on clipboard
            if (imageStream == null)
                return null;

            //Get stream bytes
            MemoryStream memStream = GetStreamFromClipboardImage(imageStream as MemoryStream) as MemoryStream;

            //Create an image to use in WPF
            return BitmapFrame.Create(memStream);
        }

        /// <summary>
        /// Gets and stream from an image retrieved from <see cref="Clipboard"/>, this enable to render or save image.
        /// </summary>
        /// <param name="imageStream">The stream containing image.</param>
        /// <returns>The <see cref="Stream"/> containing the image.</returns>
        public static Stream GetStreamFromClipboardImage(Stream imageStream)
        {
            if (imageStream == null)
                return null;

            MemoryStream memStream = imageStream as MemoryStream;

            //Get stream bytes
            byte[] didBuffer = memStream.ToArray();

            //Get bitmap info header
            BITMAPINFOHEADER infoHeader = BinaryStructHelper.ByteArrayToStruct<BITMAPINFOHEADER>(didBuffer);

            int fileHeaderSize = BinaryStructHelper.GetStructSize(typeof(BITMAPFILEHEADER));
            int fileSize = fileHeaderSize + infoHeader.biSize + infoHeader.biSizeImage;
            int infoHeaderSize = infoHeader.biSize;

            //Get bitmap file header according to bitmap info header
            BITMAPFILEHEADER fileHeader = new BITMAPFILEHEADER();
            fileHeader.bfType = BITMAPFILEHEADER.BM;
            fileHeader.bfSize = fileSize;
            fileHeader.bfReserved1 = 0;
            fileHeader.bfReserved2 = 0;
            fileHeader.bfOffBits = fileHeaderSize + infoHeaderSize + infoHeader.biClrUsed * 4;

            //Get bytes from BITMAPFILEHEADER
            byte[] fileHeaderBytes = BinaryStructHelper.StructToByteArray(fileHeader);

            //Build an image in memory using BITMAPINFOHEADER and BITMAPFILEHEADER structs
            MemoryStream memImage = new MemoryStream();
            memImage.Write(fileHeaderBytes, 0, fileHeaderSize);
            memImage.Write(didBuffer, 0, didBuffer.Length);
            memImage.Seek(0, SeekOrigin.Begin);

            return memImage;
        }

        /// <summary>
        /// Saves an image from a stream to the file system.
        /// </summary>
        /// <param name="imageStream">The stream that contains image retrieved from <see cref="Clipboard"/>.</param>
        /// <param name="fileName">The file name to save.</param>
        public static void SaveImageStream(Stream imageStream, string fileName)
        {
            MemoryStream memImage = GetStreamFromClipboardImage(imageStream) as MemoryStream;

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                byte[] data = memImage.ToArray();
                fs.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Gets a thumbnail of given image.
        /// </summary>
        /// <param name="fileName">The path of image file.</param>
        /// <param name="width">The width of thumbnail.</param>
        /// <returns>A <see cref="MemoryStream"/> contains the thumbnail bytes.</returns>
        public static MemoryStream GetImageThumbnail(string fileName, int width = 200)
        {
            MemoryStream fs = new MemoryStream();

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(fileName);
            img.DecodePixelWidth = width;
            img.EndInit();

            BitmapFrame frame = BitmapFrame.Create(img);

            PngBitmapEncoder encoder = new PngBitmapEncoder();

            encoder.Frames.Add(frame);

            encoder.Save(fs);

            return fs;
        }

        /// <summary>
        /// Gets the width and height of an image.
        /// </summary>
        /// <param name="fileName">The path of image.</param>
        /// <returns>The <see cref="Size"/> of image.</returns>
        public static Size GetImageDimensions(string fileName)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(fileName);
            img.EndInit();

            Size size = new Size(img.Width, img.Height);

            return size;
        }
    }
}
