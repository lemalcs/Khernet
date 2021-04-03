namespace Khernet.Installer.Launcher
{
    public interface IFileDownloader
    {
        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <param name="fileName">The file to download to.</param>
        /// <param name="destinationPath">The path to save the downloaded file.</param>
        void Download(string fileName, string destinationPath);
    }
}
