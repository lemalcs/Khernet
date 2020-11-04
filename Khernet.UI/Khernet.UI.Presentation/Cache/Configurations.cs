using System;
using System.IO;
using System.Reflection;

namespace Khernet.UI.Cache
{
    public static class Configurations
    {
        /// <summary>
        /// The directory of entry executable file (.exe)
        /// </summary>
        public static string HomeDirectory
        {
            get
            {
                var currentAssembly = Assembly.GetEntryAssembly();
                var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
                return currentDirectory;
            }
        }

        /// <summary>
        /// The complete path of entry executable.
        /// </summary>
        public static string MainApplicationAssembly
        {
            get
            {
                var currentAssembly = Assembly.GetEntryAssembly();
                var currentName = new FileInfo(currentAssembly.Location).FullName;
                return currentName;
            }
        }

        /// <summary>
        /// The version number of application.
        /// </summary>
        public static string AppVerion
        {
            get
            {
                string currentVersion = Assembly.GetEntryAssembly().FullName.Split(',')[1].Trim().Replace("Version=", "");

                //Take the first 3 numbers from application version
                currentVersion = currentVersion.Substring(0, currentVersion.Length - 2);
                return currentVersion;
            }
        }

        /// <summary>
        /// The directory where VLC library is stored.
        /// </summary>
        public static DirectoryInfo VlcDirectory => new DirectoryInfo(Path.Combine(HomeDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

        /// <summary>
        /// The directory that serves as temporal repository for files.
        /// </summary>
        public static DirectoryInfo CacheDirectory => new DirectoryInfo(System.IO.Path.Combine(HomeDirectory, "cache"));
    }
}
