using Khernet.Core.Common;
using Khernet.Core.Resources;
using Khernet.Core.Utility;
using System;
using System.IO;
using IWSH = IWshRuntimeLibrary;

namespace Khernet.Core.Host
{
    public class RegionFactory
    {
        /// <summary>
        /// Validate databases used by application, verifying their existence and successful connection to them.
        /// </summary>
        /// <returns>True if connection to database was successful, otherwise false.</returns>
        private bool ValidateDatabase()
        {
            try
            {
                Storage storage = new Storage();

                if (File.Exists(storage.EngineAddress) && File.Exists(storage.ConfigAddress) && File.Exists(storage.RepoAddress))
                {
                    bool isValidFile = storage.TryConnectTo(StorageType.Configuration);
                    isValidFile &= storage.TryConnectTo(StorageType.Repository);

                    return isValidFile;
                }
                return false;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        private void InstallDataBaseEngine()
        {
            Storage storage = new Storage();
            string enginePath = Path.GetDirectoryName(storage.EngineAddress);

            if (!Directory.Exists(enginePath))
            {
                Directory.CreateDirectory(enginePath);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(enginePath);
            if (directoryInfo.GetFiles().Length != 0)
            {
                return;
            }

            ResourceContainer rc = new ResourceContainer();

            //Decompress ZIP file with database engine
            using (MemoryStream memStream = new MemoryStream(rc.GetResource(Storage.ENGINE_FILE)))
            {
                Compressor compressor = new Compressor();
                compressor.UnZipFile(memStream, Path.GetDirectoryName(storage.EngineAddress));
            }
        }

        public void Build()
        {
            InstallDataBaseEngine();

            if (!ValidateDatabase())
            {
                CreateDatabases();
            }
            Storage storage = new Storage();
            if (!Directory.Exists(storage.CacheAddress))
                Directory.CreateDirectory(storage.CacheAddress);
        }

        public bool IsInitialized()
        {
            Storage storage = new Storage();

            if (!File.Exists(storage.RepoAddress) ||
                !File.Exists(storage.ConfigAddress) ||
                !File.Exists(storage.EngineAddress)
                )
                return false;

            //Verify if user has been created querying the ACCOUNT table on application database
            return storage.VerifyInitialization();
        }

        private void CreateDatabases()
        {
            try
            {
                //Create database for configurations and a database to store data of application
                Storage storage = new Storage();
                string repositoryPath = Path.GetDirectoryName(storage.RepoAddress);

                if (!Directory.Exists(repositoryPath))
                {
                    Directory.CreateDirectory(repositoryPath);
                }

                //Rename the existing file if someone is found
                RenameExistingFile(storage.ConfigAddress);

                ResourceContainer rc = new ResourceContainer();

                //Decompress ZIP file with configuration database
                using (MemoryStream memStream = new MemoryStream(rc.GetResource(Storage.CONFIGURATION_FILE)))
                {
                    Compressor compressor = new Compressor();
                    compressor.UnZipFile(memStream, Path.GetDirectoryName(storage.ConfigAddress));
                    File.Move(Path.Combine(Path.GetDirectoryName(storage.ConfigAddress), Storage.CONFIGURATION_FILE), storage.ConfigAddress);
                }

                //Rename the existing file if someone is found
                RenameExistingFile(storage.RepoAddress);

                //Decompress ZIP file with application database
                using (MemoryStream memStream2 = new MemoryStream(rc.GetResource(Storage.MESSAGE_DB)))
                {
                    Compressor compressor2 = new Compressor();
                    compressor2.UnZipFile(memStream2, Path.GetDirectoryName(storage.RepoAddress));
                    File.Move(Path.Combine(Path.GetDirectoryName(storage.RepoAddress), Storage.MESSAGE_DB), storage.RepoAddress);
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        private string RenameExistingFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    string newFileName = fileName;
                    for (int counter = 0; File.Exists(newFileName);)
                    {
                        counter++;
                        newFileName = Path.Combine(
                            Path.GetDirectoryName(fileName),//Fill path of file
                            string.Format("{0} ({1}){2}",
                                            Path.GetFileNameWithoutExtension(fileName), //Name of file
                                            counter,//Counter for new file name
                                            Path.GetExtension(fileName))//Extension of file
                            );
                    }
                    File.Move(fileName, newFileName);
                    return newFileName;
                }
                return null;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        public void CreateShortcut()
        {
            string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var shell = new IWSH.WshShell();
            var shortCutLinkFilePath = Path.Combine(startupFolderPath, Storage.SHORTCUT_NAME);
            var windowsApplicationShortcut = (IWSH.IWshShortcut)shell.CreateShortcut(shortCutLinkFilePath);
            windowsApplicationShortcut.Description = "You can disable auto-run in Khernet settings.";

            Storage storage = new Storage();
            windowsApplicationShortcut.WorkingDirectory = Path.GetDirectoryName(storage.EntryAssemblyPath);
            windowsApplicationShortcut.TargetPath = storage.EntryAssemblyPath;
            windowsApplicationShortcut.Save();
        }

        public void RemoveShortcut()
        {
            string shortcutPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                Storage.SHORTCUT_NAME
                );
            if (File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
            }
        }
    }
}
