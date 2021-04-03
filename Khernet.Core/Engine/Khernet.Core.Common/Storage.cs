using FirebirdSql.Data.FirebirdClient;
using Khernet.Core.Utility;
using System;
using System.Data;
using System.IO;

namespace Khernet.Core.Common
{
    public enum StorageType
    {
        /// <summary>
        /// Database of application's configurations.
        /// </summary>
        Configuration = 0,

        /// <summary>
        /// Database of applications's data such as messages and user information.
        /// </summary>
        Repository = 1
    }

    public class Storage
    {
        public const string CONFIGURATION_FILE = "config";

        public const string FIREBIRD_LIBRARY = "fbclient.dll";

        public const string MESSAGE_DB = "msgdb";

        public const string FILE_DB = "stgdb";

        private readonly string dataFolder = "data";

        /// <summary>
        /// The path of file system where FIREBIRD engine is located.
        /// </summary>
        public string EngineAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), "firebird", "fbclient.dll");
            }
        }

        /// <summary>
        /// The path of file system where configuration's database is located.
        /// </summary>
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

        /// <summary>
        /// The path of file system where database of file messages is located.
        /// </summary>
        public string RepoAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), this.dataFolder, MESSAGE_DB);
            }
        }

        /// <summary>
        /// The path of file system where database of messages is located.
        /// </summary>
        public string FileRepoAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), this.dataFolder, FILE_DB);
            }
        }

        /// <summary>
        /// The path of file system where cache folder is located.
        /// </summary>
        public string CacheAddress
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                return Path.Combine(Path.GetDirectoryName(assembly.Location), "cache");
            }
        }

        /// <summary>
        /// Gets a connection string to a specific database based on <see cref="StorageType"/> enumeration.
        /// </summary>
        /// <param name="storageType">The type of database.</param>
        /// <returns>The connection string.</returns>
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

        /// <summary>
        /// Checks if user has an account to log in to application.
        /// </summary>
        /// <returns>True if account exists otherwise false.</returns>
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

        /// <summary>
        /// Connects to a database file to verify if it exists and is valid.
        /// </summary>
        /// <param name="storageType">The type of database.</param>
        /// <returns>True if connection success otherwise false.</returns>
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

        /// <summary>
        /// Gets a new file name based on existing file appending a sequential number.
        /// </summary>
        /// <param name="fileName">The path of file.</param>
        /// <returns>If file exists returns a file name with a sequential number appended otherwise returns the same file name.</returns>
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
