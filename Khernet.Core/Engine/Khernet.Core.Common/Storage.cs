using FirebirdSql.Data.FirebirdClient;
using Khernet.Core.Utility;
using System;
using System.Data;
using System.IO;

namespace Khernet.Core.Common
{
    public enum StorageType
    {
        Configuration = 0,
        Repository = 1
    }

    public class Storage
    {
        public string EngineAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), "dbin\\fbclient.dll");
            }
        }

        public string ConfigAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();

                //Full path to database that contains application configurations
                return string.Format("{0}.{1}",
                    Path.Combine(Path.GetDirectoryName(assembly.Location), Path.GetFileNameWithoutExtension(assembly.Location)),
                    "dat");
            }
        }

        public string RepoAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), "kdata\\E7FE8601FEAE.KND");
            }
        }

        public string FolderRepoAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), "kdata\\Download");
            }
        }

        public string FileRepoAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), "kdata\\KL732418.KND");
            }
        }

        public string CacheAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), "cache");
            }
        }

        public string MediaToolsAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), "tls");
            }
        }

        public string VLCLibX86Address
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), "libvlc", "win-x86");
            }
        }

        public string BuildConnectionString(StorageType storageType)
        {
            FbConnectionStringBuilder connBuilder = new FbConnectionStringBuilder();
            connBuilder.ServerType = FbServerType.Embedded;
            connBuilder.UserID = "SYSDBA";
            connBuilder.Password = "blank";
            connBuilder.Dialect = 3;
            connBuilder.ClientLibrary = EngineAddress;

            if (storageType == StorageType.Configuration)
            {
                connBuilder.Database = ConfigAddress;
            }
            else
            {
                connBuilder.Database = RepoAddress;
            }

            return connBuilder.ToString();
        }

        public bool VerifyInitialization()
        {
            FbCommand cmd = new FbCommand("VERIFY_INITIALIZATION");
            cmd.CommandType = CommandType.StoredProcedure;

            bool result = false;
            using (cmd.Connection = new FbConnection(BuildConnectionString(StorageType.Configuration)))
            {
                cmd.Connection.Open();
                result = Convert.ToBoolean(cmd.ExecuteScalar());
            }
            return result;
        }

        public bool TryConnectTo(StorageType storageType)
        {
            try
            {
                FbCommand cmd = new FbCommand();
                using (cmd.Connection = new FbConnection(BuildConnectionString(storageType)))
                {
                    cmd.Connection.Open();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string RenameDirectory(string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    string newDirectoryName = directory;
                    string[] directories = newDirectoryName.Split(Path.DirectorySeparatorChar);
                    string directoryname = directories[directories.Length - 1];

                    for (int counter = 0; Directory.Exists(newDirectoryName);)
                    {
                        counter++;
                        newDirectoryName = Path.Combine(
                            Directory.GetParent(directory).FullName,//Full path of file
                            string.Format("{0} ({1})",
                                            directoryname, //Name of file
                                            counter//Counter for new file name
                            ));
                    }
                    Directory.Move(directory, newDirectoryName);

                    return newDirectoryName;
                }
                return null;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        public string GetNewFileFrom(string fileName)
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
                            Path.GetDirectoryName(fileName),//Full path of file
                            string.Format("{0} ({1}){2}",
                                            Path.GetFileNameWithoutExtension(fileName), //Name of file
                                            counter,//Counter for new file name
                                            Path.GetExtension(fileName))//Extension of file
                            );
                    }
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
