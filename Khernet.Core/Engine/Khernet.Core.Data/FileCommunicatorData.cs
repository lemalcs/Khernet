using FirebirdSql.Data.FirebirdClient;
using Khernet.Core.Common;
using Khernet.Core.Utility;
using LiteDB;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace Khernet.Core.Data
{
    public class FileCommunicatorData
    {
        private string GetConnectionString()
        {
            Storage st = new Storage();
            return st.BuildConnectionString(StorageType.Repository);
        }

        public byte[] GetSelfAvatar()
        {
            try
            {
                DataTable table = new DataTable();
                FbCommand cmd = new FbCommand("GET_ACCOUNT_AVATAR");
                cmd.CommandType = CommandType.StoredProcedure;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                byte[] data = null;

                if (table.Rows.Count > 0)
                {
                    if (table.Rows[0][0] != null && table.Rows[0][0] != DBNull.Value)
                    {
                        var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                        data = EncryptionHelper.DecryptByteArray((byte[])table.Rows[0][0], keys.Item1, keys.Item2);
                        keys = null;
                    }
                }

                return data;
            }
            catch (Exception error)
            {

                throw error;
            }
        }

        public void SaveFile(string idFile, string fileName, Stream fileData)
        {
            try
            {
                Storage st = new Storage();

                using (LiteDatabase db = new LiteDatabase(st.FileRepoAddress))
                {
                    CryptographyProvider crypto = new CryptographyProvider();

                    string fileNameCode = crypto.GetBase58Check(Encoding.UTF8.GetBytes(fileName));

                    var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);

                    using (LiteFileStream liteFs = db.FileStorage.OpenWrite(idFile, fileNameCode))
                    using (var cryptoStream = crypto.EncryptStream(fileData, keys.Item1, keys.Item2))
                    {
                        int chunk = 1048576;
                        byte[] temporal = new byte[chunk];
                        int read = 0;

                        int actualReadBytes = read;

                        read = cryptoStream.Read(temporal, 0, chunk);

                        while (read > 0)
                        {
                            liteFs.Write(temporal, 0, read);
                            actualReadBytes += read;
                            read = cryptoStream.Read(temporal, 0, chunk);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        public void SaveFileWithoutProgress(string idFile, string fileName, Stream fileData)
        {
            try
            {
                Storage st = new Storage();

                using (LiteDatabase db = new LiteDatabase(st.FileRepoAddress))
                {
                    CryptographyProvider crypto = new CryptographyProvider();
                    string fileNameCode = crypto.GetBase58Check(Encoding.UTF8.GetBytes(fileName));

                    var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);

                    using (var cryptoStream = crypto.EncryptStream(fileData, keys.Item1, keys.Item2))
                    {
                        db.FileStorage.Upload(idFile, fileNameCode, cryptoStream);
                    }
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        public Stream GetFile(string idFile)
        {
            Storage st = new Storage();
            LiteDatabase db = new LiteDatabase(st.FileRepoAddress);

            LiteFileInfo fileInfo = db.FileStorage.FindById(idFile);

            if (fileInfo != null)
            {
                Tuple<byte[], byte[]> keys = null;
                try
                {
                    CryptographyProvider crypto = new CryptographyProvider();

                    keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);

                    LiteFileStream liteFs = fileInfo.OpenRead();
                    var cryp = crypto.DecryptStream(liteFs, keys.Item1, keys.Item2);
                    return cryp;
                }
                catch (Exception exception)
                {
                    LogDumper.WriteLog(exception);
                    throw exception;
                }
                finally
                {
                    keys = null;
                }
            }
            else
                throw new Exception("File not found.");

        }

        public long GetFileSize(string idFile)
        {
            Storage st = new Storage();
            LiteDatabase db = new LiteDatabase(st.FileRepoAddress);

            LiteFileInfo fileInfo = db.FileStorage.FindById(idFile);

            if (fileInfo != null)
            {
                return fileInfo.Length;
            }
            else
                throw new Exception("File not found.");
        }

        public bool ExistsFile(string idFile)
        {
            Storage st = new Storage();
            LiteDatabase db = new LiteDatabase(st.FileRepoAddress);

            return db.FileStorage.Exists(idFile);
        }

        /// <summary>
        /// Delete a file stored on database
        /// </summary>
        /// <param name="idFile">The ID of file</param>
        public void DeleteFile(string idFile)
        {
            Storage st = new Storage();
            LiteDatabase db = new LiteDatabase(st.FileRepoAddress);
            db.FileStorage.Delete(idFile);
        }

        public void SaveThumbnail(int idMessage, byte[] thumbnail)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_THUMBNAIL");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = idMessage;
                cmd.Parameters.Add("@THUMBNAIL", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(thumbnail, keys.Item1, keys.Item2);
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int resultado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetThumbnail(int idMessage)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_THUMBNAIL");
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = idMessage;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i][0] != null && table.Rows[i][0] != DBNull.Value)
                        table.Rows[i][0] = EncryptionHelper.DecryptByteArray(table.Rows[i][0] as byte[], keys.Item1, keys.Item2);
                }
                keys = null;

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int SaveAnimation(int idMessage, string idFile, int width, int height, byte[] animation)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_ANIMATION");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@ID_FILE", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(idFile, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@WIDTH", FbDbType.Integer).Value = width;
                cmd.Parameters.Add("@HEIGHT", FbDbType.Integer).Value = height;
                cmd.Parameters.Add("@THUMBNAIL", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(animation, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = idMessage;
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int resultado = Convert.ToInt32(cmd.ExecuteScalar());
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetAnimationList()
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_ANIMATION_LIST");
                cmd.CommandType = CommandType.StoredProcedure;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetAnimationContent(int id)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_ANIMATION_CONTENT");
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ID", FbDbType.Integer).Value = id;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    //Id file
                    table.Rows[i][1] = EncryptionHelper.DecryptString(table.Rows[i][1].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Byte array of content
                    table.Rows[i][4] = EncryptionHelper.DecryptByteArray(table.Rows[i][4] as byte[], keys.Item1, keys.Item2);
                }
                keys = null;

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetCacheFilePath(int idMessage)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_CACHE_FILE");
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = idMessage;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }


                if (table.Rows.Count > 0)
                {
                    if (table.Rows[0][0] == null || table.Rows[0][0] == DBNull.Value)
                        return null;

                    var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                    byte[] encodedPath = EncryptionHelper.DecryptByteArray(table.Rows[0][0] as byte[], keys.Item1, keys.Item2);
                    keys = null;

                    return Encoding.UTF8.GetString(encodedPath);
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateCacheFilePath(int idMessage, string filePath)
        {
            try
            {
                FbCommand cmd = new FbCommand("UPDATE_CACHE_FILE");
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = idMessage;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@FILE_PATH", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(Encoding.UTF8.GetBytes(filePath), keys.Item1, keys.Item2); ;
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int resultado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetFileList(string userToken, int fileType, int lastIdMessage, int quantity)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_FILE_LIST");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@USER_TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(userToken, Encoding.UTF8, keys.Item1, keys.Item2); ;
                keys = null;

                cmd.Parameters.Add("@FILE_TYPE", FbDbType.Integer).Value = fileType;
                cmd.Parameters.Add("@LAST_ID_MESSAGE", FbDbType.Integer).Value = lastIdMessage;
                cmd.Parameters.Add("@QUANTITY", FbDbType.Integer).Value = quantity;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                if (table.Rows.Count > 0)
                {
                    keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        //State
                        if (table.Rows[i][3] == DBNull.Value)
                            table.Rows[i][3] = 0;

                        //UID
                        if (table.Rows[i][5] != DBNull.Value)
                            table.Rows[i][5] = EncryptionHelper.DecryptString(table.Rows[i][5].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                    }
                }

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Gets the number of files shared between current user and remote peer.
        /// </summary>
        /// <param name="userToken">The type of file.</param>
        /// <param name="fileType">The token of remote peer.</param>
        /// <returns>The number of files.</returns>
        public int GetFileCount(string userToken, int fileType)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_FILE_COUNT");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@USER_TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(userToken, Encoding.UTF8, keys.Item1, keys.Item2); ;
                keys = null;

                cmd.Parameters.Add("@FILE_TYPE", FbDbType.Integer).Value = fileType;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                if (table.Rows.Count > 0)
                {
                    if (table.Rows[0][0] == null || table.Rows[0][0] == DBNull.Value)
                        return 0;

                    return Convert.ToInt32(table.Rows[0][0]);
                }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
