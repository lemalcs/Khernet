using System;
using System.Runtime.InteropServices;

namespace Khernet.UI.Resources
{
    /// <summary>
    /// Contains method that calls to windows API.
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        public const int SW_SHOW = 5;

        /// <summary>
		/// Determines the MIME type from the first 256 bytes of data provided.
		/// </summary>
		/// <param name="pBC">null or pointer to bind context.</param>
		/// <param name="pwzUrl">File name or URL containing data to examine.</param>
		/// <param name="pBuffer">Output buffer.</param>
		/// <param name="cbSize">Buffer size.</param>
		/// <param name="pwzMimeProposed">Suggested Mime type.</param>
		/// <param name="dwMimeFlags">Flags that control how function operates.</param>
		/// <param name="ppwzMimeOut">out value containing the Mime type.</param>
		/// <param name="dwReserved">Reserved.</param>
		/// <returns>S_OK, E_FAIL, E_INVALIDARG or E_OUTOFMEMORY.</returns>
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

        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="hWnd">A handle to the window that should be activated and brought to the foreground.</param>
        /// <param name="nCmdShow">Controls how the window is to be shown.</param>
        /// <returns>
        /// If the window was previously visible, the return value is nonzero.
        /// If the window was previously hidden, the return value is zero.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Flashes the specified window. It does not change the active state of the window.
        /// </summary>
        /// <param name="pwfi">A pointer to a <see cref="FLASHWINFO"/> structure.</param>
        /// <returns>The return value specifies the window's state before the call to the FlashWindowEx function. 
        /// If the window caption was drawn as active before the call, the return value is nonzero. Otherwise, the return value is zero.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO
    {
        /// <summary>
        /// The size of the structure, in bytes.
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// A handle to the window to be flashed. The window can be either opened or minimized.
        /// </summary>
        public IntPtr hwnd;

        /// <summary>
        /// The flash status based on <see cref="FlashWindow"/>.
        /// </summary>
        public uint dwFlags;

        /// <summary>
        /// The number of times to flash the window.
        /// </summary>
        public uint uCount;

        /// <summary>
        /// The rate at which the window is to be flashed, in milliseconds. If dwTimeout is zero, the function uses the default cursor blink rate.
        /// </summary>
        public uint dwTimeout;
    }

    public enum FlashWindow : uint
    {
        //Stop flashing. The system restores the window to its original state.
        FLASHW_STOP = 0,
        
        //Flash the window caption.
        FLASHW_CAPTION = 1,

        //Flash the taskbar button.
        FLASHW_TRAY = 2,

        //Flash both the window caption and taskbar button.
        //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
        FLASHW_ALL = 3,

        //Flash continuously, until the FLASHW_STOP flag is set.
        FLASHW_TIMER = 4,

        //Flash continuously until the window comes to the foreground.        
        FLASHW_TIMERNOFG = 12,
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
        /// Graphics Interchange Format (GIF)
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
        /// Enhanced Metafile (EMF)
        /// </summary>
        public const string Xemf = "image/x-emf";

        /// <summary>
        /// Windows Metafile Format (WMF)
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
        /// Portable Document Format (PDF)
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
