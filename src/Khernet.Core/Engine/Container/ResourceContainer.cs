using System.IO;
using System.Reflection;

namespace Khernet.Core.Resources
{
    public class ResourceContainer
    {
        /// <summary>
        /// Gets a resource that is embedded into assembly.
        /// </summary>
        /// <param name="assetName">The name of resource to get.</param>
        /// <returns>A <see cref="byte"/> array containing the resource.</returns>
        public byte[] GetResource(string assetName)
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
