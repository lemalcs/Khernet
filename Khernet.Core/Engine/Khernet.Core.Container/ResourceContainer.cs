using System.IO;
using System.Reflection;

namespace Khernet.Core.Resources
{
    public class ResourceContainer
    {
        /// <summary>
        /// Get the configuration file (firebird database) for application
        /// </summary>
        /// <returns>An arrsy of bytes containing firebird database</returns>
        public byte[] GetConfigurationFile()
        {
            return GetResource("CONFIG_zip");
        }

        /// <summary>
        /// Get the firebird database libraries
        /// </summary>
        /// <returns>An array of bytes of zip file containing firebird libraries</returns>
        public byte[] GetDataBaseEngine()
        {
            return GetResource("FIREBIRD"); //Resources.FIREBIRD;
        }

        /// <summary>
        /// Get the database to store data used by application for example: chat messages, contact list
        /// </summary>
        /// <returns></returns>
        public byte[] GetApplicationDataBase()
        {
            return GetResource("KH_zip"); //Resources.KH_zip;
        }

        /// <summary>
        /// Get ffmpeg libraries to read and write media files
        /// </summary>
        /// <returns>An array of bytes of zip file containing ffmpeg libraries</returns>
        public byte[] GetMediaTools()
        {
            return GetResource("tls_zip"); //Resources.tls_zip;
        }

        /// <summary>
        /// Get VLC libraries to play audio and video files
        /// </summary>
        /// <returns>An array of bytes of zip file containing VLC libraries</returns>
        public byte[] GetVLCLibrary()
        {
            return GetResource("vlc_x86_zip");//Resources.vlc_x86_zip;
        }

        private byte[] GetResource(string assetName)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            using (Stream stream = executingAssembly.GetManifestResourceStream(assetName))
            {
                if (stream == null)
                    return null;

                byte[] assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);

                return assemblyRawBytes;
            }
        }
    }
}
