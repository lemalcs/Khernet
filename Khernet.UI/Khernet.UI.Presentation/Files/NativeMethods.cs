using System;
using System.Runtime.InteropServices;

namespace Khernet.UI.Media
{
    /// <summary>
    /// Contains method that calls to windows API.
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
		/// Determines the MIME type from the first 256 bytes of data provided
		/// </summary>
		/// <param name="pBC">null or pointer to bind context</param>
		/// <param name="pwzUrl">File name or URL containing data to examine</param>
		/// <param name="pBuffer">Output buffer</param>
		/// <param name="cbSize">Buffer size</param>
		/// <param name="pwzMimeProposed">Suggested Mime type</param>
		/// <param name="dwMimeFlags">Flags that control how function operates</param>
		/// <param name="ppwzMimeOut">out value containing the Mime type</param>
		/// <param name="dwReserved">Reserved</param>
		/// <returns>S_OK, E_FAIL, E_INVALIDARG or E_OUTOFMEMORY</returns>
		[DllImport("urlmon.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = false)]
        internal static extern int FindMimeFromData(
            IntPtr pBC,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)]
            byte[] pBuffer,
            int cbSize,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwzMimeProposed,
            int dwMimeFlags,
            out string ppwzMimeOut,
            int dwReserved);
    }

    /// <summary>
    /// Known MIME types.
    /// </summary>
    public static class MimeTypes
    {
        /// <summary>
        /// Plain text. Default if data is primarily text and no other type detected.
        /// </summary>
        public const string Plain = "text/plain";

        /// <summary>
        /// HTML. Default if common tags detected and server does not supply image/* type.
        /// </summary>
        public const string Html = "text/html";

        /// <summary>
        /// XML data. Default if data specifies <?xml with an unrecognized DTD.
        /// </summary>
        public const string Xml = "text/xml";

        /// <summary>
        /// Rich Text Format (RTF).
        /// </summary>
        public const string Richtext = "text/richtext";

        /// <summary>
        /// Windows script component.
        /// </summary>
        public const string Scriptlet = "text/scriptlet";

        /// <summary>
        /// Audio Interchange File, Macintosh.
        /// </summary>
        public const string Xaiff = "audio/x-aiff";

        /// <summary>
        /// Audio file, UNIX.
        /// </summary>
        public const string Basic = "audio/basic";

        /// <summary>
        /// MIDI sequence.
        /// </summary>
        public const string Mid = "audio/mid";

        /// <summary>
        /// Pulse Code Modulation (PCM) Wave audio, Windows.
        /// </summary>
        public const string Wav = "audio/wav";

        /// <summary>
        /// Graphics Interchange Format (GIF).
        /// </summary>
        public const string Gif = "image/gif";

        /// <summary>
        /// JPEG image.
        /// </summary>
        public const string Jpeg = "image/jpeg";

        /// <summary>
        /// Default type for JPEG images.
        /// </summary>
        public const string Pjpeg = "image/pjpeg";

        /// <summary>
        /// Portable Network Graphics (PNG).
        /// </summary>
        public const string Png = "image/png";

        /// <summary>
        /// Default type for PNG images.
        /// </summary>
        public const string Xpng = "image/x-png";

        /// <summary>
        /// Tagged Image File Format (TIFF) image.
        /// </summary>
        public const string Tiff = "image/tiff";

        /// <summary>
        /// Bitmap (BMP) image.
        /// </summary>
        public const string Bmp = "image/bmp";

        /// <summary>
        /// Bitmap.
        /// </summary>
        public const string Xxbitmap = "image/x-xbitmap";

        /// <summary>
        /// AOL Johnson-Grace compressed file.
        /// </summary>
        public const string Xjg = "image/x-jg";

        /// <summary>
        /// Enhanced Metafile (EMF).
        /// </summary>
        public const string Xemf = "image/x-emf";

        /// <summary>
        /// Windows Metafile Format (WMF).
        /// </summary>
        public const string Xwmf = "image/x-wmf";

        /// <summary>
        /// Audio-Video Interleaved (AVI) file.
        /// </summary>
        public const string Avi = "video/avi";

        /// <summary>
        /// MPEG stream file.
        /// </summary>
        public const string Mpeg = "video/mpeg";

        /// <summary>
        /// Binary file. Default if data is primarily binary.
        /// </summary>
        public const string Octetstream = "application/octet-stream";

        /// <summary>
        /// PostScript (.ai, .eps, or .ps) file.
        /// </summary>
        public const string Postscript = "application/postscript";

        /// <summary>
        /// Base64-encoded bytes.
        /// </summary>
        public const string Base64 = "application/base64";

        /// <summary>
        /// BinHex for Macintosh.
        /// </summary>
        public const string Macbinhex40 = "application/macbinhex40";

        /// <summary>
        /// Portable Document Format (PDF).
        /// </summary>
        public const string Pdf = "application/pdf";

        /// <summary>
        /// XML data. Must be server-supplied. See also "text/xml" type.
        /// </summary>
        public const string Xml2 = "application/xml";

        /// <summary>
        /// Atom Syndication Format feed.
        /// </summary>
        public const string AtomXml = "application/atom+xml";

        /// <summary>
        /// Really Simple Syndication (RSS) feed.
        /// </summary>
        public const string RssXml = "application/rss+xml";

        /// <summary>
        /// UNIX tar file, Gzipped.
        /// </summary>
        public const string Xcompressed = "application/x-compressed";

        /// <summary>
        /// Compressed archive file.
        /// </summary>
        public const string XzipCompressed = "application/x-zip-compressed";

        /// <summary>
        /// Gzip compressed archive file.
        /// </summary>
        public const string XgzipCompressed = "application/x-gzip-compressed";

        /// <summary>
        /// Java applet.
        /// </summary>
        public const string Java = "application/java";

        /// <summary>
        /// Executable (.exe or .dll) file.
        /// </summary>
        public const string Xmsdownload = "application/x-msdownload";
    }
}
