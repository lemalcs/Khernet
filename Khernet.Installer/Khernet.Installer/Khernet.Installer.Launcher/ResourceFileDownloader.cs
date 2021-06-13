using System.Diagnostics;

namespace Khernet.Installer.Launcher
{
    /// <summary>
    /// Provides access to files located in assemblies.
    /// </summary>
    public class ResourceFileDownloader : IFileDownloader
    {
        string resourceAssemblyPath;
        public ResourceFileDownloader(string resourceAssemblyPath)
        {
            this.resourceAssemblyPath = resourceAssemblyPath;
        }

        /// <summary>
        /// Downloads a resource from a remote assembly.
        /// </summary>
        /// <param name="fileName">The file name to download to.</param>
        /// <param name="destinationPath">The path to save the downloaded file.</param>
        public void Download(string fileName, string destinationPath)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = resourceAssemblyPath;
            processStartInfo.Arguments = $"{fileName} {destinationPath}";

            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
