using FirebirdSql.Data.FirebirdClient;
using Khernet.Core.Utility;
using System;
using System.Data;
using System.Security;
using System.Text;

namespace Khernet.Core.Common
{
    public class Configuration
    {
        private static SecureString passwd;
        private static SecureString connection;
        private static string configurationPath;

        static Configuration()
        {
            Storage storage = new Storage();
            configurationPath = storage.BuildConnectionString(StorageType.Configuration);
        }

        public static void SetPassword(SecureString passwd)
        {
            Configuration.passwd = EncryptionHelper.PackAESKeys(passwd);
        }

        private static string GetConnectionString()
        {
            return configurationPath;
        }

        public static string GetValue(string key)
        {
            DataTable table = new DataTable();
            CryptographyProvider cp = new CryptographyProvider();

            FbCommand cmd = new FbCommand("GET_VALUE");
            cmd.CommandType = CommandType.StoredProcedure;

            //The key that identify the configuration is stored encrypted in database
            var keys = EncryptionHelper.UnpackAESKeys(passwd);
            cmd.Parameters.Add("@IDKEY", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(key, Encoding.UTF8, keys.Item1, keys.Item2);
            key = null;

            using (cmd.Connection = new FbConnection(GetConnectionString()))
            {
                cmd.Connection.Open();
                FbDataAdapter fda = new FbDataAdapter(cmd);
                fda.Fill(table);
            }

            string configValue = null;

            if (table.Rows.Count > 0)
            {
                byte[] rawValue = (byte[])table.Rows[0][0];
                if (Configuration.passwd != null)
                {
                    keys = EncryptionHelper.UnpackAESKeys(passwd);
                    configValue = Encoding.UTF8.GetString(cp.DecryptResource(rawValue, keys.Item1, keys.Item2));
                }
            }

            return configValue;
        }

        public static void SetValue(string key, string value)
        {
            try
            {
                CryptographyProvider cp = new CryptographyProvider();

                var keys = EncryptionHelper.UnpackAESKeys(passwd);
                byte[] valor = cp.EncryptResource(Encoding.UTF8.GetBytes(value), keys.Item1, keys.Item2);
                value = null;

                FbCommand cmd = new FbCommand("SET_VALUE");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@IDKEY", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(key, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

                cmd.Parameters.Add("@VAL", FbDbType.Binary).Value = valor;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int resultado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                value = null;
            }
        }

        public static void SetPlainValue(string key, byte[] value)
        {
            try
            {
                FbCommand cmd = new FbCommand("SET_VALUE");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@IDKEY", FbDbType.VarChar).Value = key;

                cmd.Parameters.Add("@VAL", FbDbType.Binary).Value = value;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int resultado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                value = null;
            }
        }

        public static byte[] GetPlainValue(string key)
        {
            DataTable table = new DataTable();

            FbCommand cmd = new FbCommand("GET_VALUE");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@IDKEY", FbDbType.VarChar).Value = key;

            using (cmd.Connection = new FbConnection(GetConnectionString()))
            {
                cmd.Connection.Open();
                FbDataAdapter fda = new FbDataAdapter(cmd);
                fda.Fill(table);
            }

            if (table.Rows.Count > 0 && table.Rows[0][0] != DBNull.Value)
            {
                return (byte[])table.Rows[0][0];
            }

            return null;
        }
    }
}
