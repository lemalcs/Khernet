using Khernet.Core.Common;
using Khernet.Core.Resources;
using Khernet.Core.Utility;
using System;
using System.IO;

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

        public void Build()
        {
            if (!ValidateDatabase())
            {
                InstallDatabaseEngine();
            }
            if (!ValidateDatabase())
            {
                CreateDatabases();
            }
            Storage storage = new Storage();
            if (!Directory.Exists(storage.CacheAddress))
                Directory.CreateDirectory(storage.CacheAddress);

            if (!Directory.Exists(storage.MediaToolsAddress))
            {
                Directory.CreateDirectory(storage.MediaToolsAddress);
                UnPackMediaTools();
            }

            if (!Directory.Exists(storage.VLCLibX86Address))
            {
                Directory.CreateDirectory(storage.VLCLibX86Address);
                UnPackVLCLib();
            }
        }

        public bool IsInitialized()
        {
            //Verify if user has been created querying the ACCOUNT table on application database
            Storage storage = new Storage();
            return storage.VerifyInitialization();
        }

        private void InstallDatabaseEngine()
        {
            try
            {
                //Create folder to store FIREBIRD database engine files
                Storage storage = new Storage();
                string firebirdEnginePath = Path.GetDirectoryName(storage.EngineAddress);

                if (!Directory.Exists(firebirdEnginePath))
                {
                    Directory.CreateDirectory(firebirdEnginePath);
                }

                ResourceContainer rc = new ResourceContainer();

                using (MemoryStream memStream = new MemoryStream(rc.GetDataBaseEngine()))
                {
                    Compressor compressor = new Compressor();
                    compressor.UnZipFile(memStream, firebirdEnginePath);
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
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
                using (MemoryStream memStream = new MemoryStream(rc.GetConfigurationFile()))
                {
                    Compressor compressor = new Compressor();
                    compressor.UnZipFile(memStream, Path.GetDirectoryName(storage.ConfigAddress));

                    File.Move(Path.Combine(Path.GetDirectoryName(storage.ConfigAddress), "CONFIG"), storage.ConfigAddress);
                }

                //Rename the existing file if someone is found
                RenameExistingFile(storage.RepoAddress);

                //Decompress ZIP file with application database
                using (MemoryStream memStream = new MemoryStream(rc.GetApplicationDataBase()))
                {
                    Compressor compressor = new Compressor();
                    compressor.UnZipFile(memStream, Path.GetDirectoryName(storage.RepoAddress));

                    File.Move(Path.Combine(Path.GetDirectoryName(storage.RepoAddress), "KH"), storage.RepoAddress);
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        private void UnPackMediaTools()
        {
            ResourceContainer rc = new ResourceContainer();
            using (MemoryStream memStream = new MemoryStream(rc.GetMediaTools()))
            {
                Compressor compressor = new Compressor();
                Storage storage = new Storage();

                compressor.UnZipFile(memStream, storage.MediaToolsAddress);
            }
        }

        private void UnPackVLCLib()
        {
            ResourceContainer rc = new ResourceContainer();
            using (MemoryStream memStream = new MemoryStream(rc.GetVLCLibrary()))
            {
                Compressor compressor = new Compressor();
                Storage storage = new Storage();

                compressor.UnZipFile(memStream, storage.VLCLibX86Address);
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
    }
}
