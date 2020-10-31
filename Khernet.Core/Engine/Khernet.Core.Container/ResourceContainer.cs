using System.IO;
using System.Reflection;

namespace Khernet.Core.Resources
{
    public class ResourceContainer
    {
        /// <summary>
        /// Get the configuration file (FIREBIRD database) for application.
        /// </summary>
        /// <returns>An array of bytes containing FIREBIRD database.</returns>
        public byte[] GetConfigurationFile()
        {
            return GetResource("CONFIG_zip");
        }

        /// <summary>
        /// Get the FIREBIRD database libraries.
        /// </summary>
        /// <returns>A <see cref="byte"/> array of zip file containing FIREBIRD libraries.</returns>
        public byte[] GetDataBaseEngine()
        {
            return GetResource("FIREBIRD");
        }

        /// <summary>
        /// Get the database to store data used by application for example: chat messages, contact list.
        /// </summary>
        /// <returns>A <see cref="byte"/> array of zip file containing database.</returns>
        public byte[] GetApplicationDataBase()
        {
            return GetResource("KH_zip");
        }

        /// <summary>
        /// Get FFMPEG libraries to read and write media files
        /// </summary>
        /// <returns>A <see cref="byte"/> array of zip file containing FFMPEG libraries</returns>
        public byte[] GetMediaTools()
        {
            return GetResource("tls_zip");
        }

        /// <summary>
        /// Get VLC libraries to play audio and video files.
        /// </summary>
        /// <returns>A <see cref="byte"/> array of zip file containing VLC libraries.</returns>
        public byte[] GetVLCLibrary()
        {
            return GetResource("vlc_x86_zip");
        }

        /// <summary>
        /// Gets a resource that is embedded into assembly.
        /// </summary>
        /// <param name="assetName">The name of resource to get.</param>
        /// <returns>A <see cref="byte"/> array containing the resource.</returns>
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
