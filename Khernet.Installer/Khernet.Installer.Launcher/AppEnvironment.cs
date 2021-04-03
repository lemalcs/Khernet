using System.IO;

namespace Khernet.Installer.Launcher
{
    public class AppEnvironment
    {
        /// <summary>
        /// The path where application folder exists.
        /// </summary>
        string rootPath;

        /// <summary>
        /// The folder where main application exists.
        /// </summary>
        public const string MainDirectory = "app";

        public const string AppName = "Khernet.exe";
        public const string VlcPack = "libvlc_win-x86.zip";
        public const string MediaPack = "media.zip";
        public const string FirebirdPack = "firebird.zip";

        /// <summary>
        /// The full path of main application.
        /// </summary>
        public string AppPath => Path.Combine(rootPath, AppName);

        public AppEnvironment(string parentRootPath)
        {
            this.rootPath = Path.Combine(Directory.GetParent(Path.GetDirectoryName(parentRootPath)).FullName, MainDirectory);
        }

        /// <summary>
        /// Checks whether exists the path of main application.
        /// </summary>
        /// <returns>True if path exists otherwise false.</returns>
        public bool ExistsAppDirectory()
        {
            return Directory.Exists(rootPath);
        }

        /// <summary>
        /// Creates the path for main applications.
        /// </summary>
        /// <returns>The created path.</returns>
        public string CreateAppDirectory()
        {
            if (!Directory.Exists(rootPath))
            {
                DirectoryInfo directory = Directory.CreateDirectory(rootPath);
                return directory.FullName;
            }

            return rootPath;
        }
    }
}
