using FirebirdSql.Data.FirebirdClient;
using Khernet.Core.Common;
using Khernet.Core.Utility;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace Khernet.Core.Data
{
    public class EventListenerData
    {
        private static string GetConnectionString()
        {
            Storage st = new Storage();
            return st.BuildConnectionString(StorageType.Repository);
        }

        public void SaveNotification(string token, byte notificationType, string content)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_NOTIFICATION");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@TYPE_NOT", FbDbType.SmallInt).Value = notificationType;
                cmd.Parameters.Add("@CONTENT", FbDbType.Date).Value = EncryptionHelper.EncryptString(content, Encoding.UTF8, keys.Item1, keys.Item2);
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void DeleteNotification(string token, byte notificationType)
        {
            try
            {
                FbCommand cmd = new FbCommand("DELETE_NOTIFICATION");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@TYPE_NOT", FbDbType.SmallInt).Value = notificationType;
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

        public void ClearNotifications()
        {
            try
            {
                FbCommand cmd = new FbCommand("CLEAR_NOTIFICATIONS");
                cmd.CommandType = CommandType.StoredProcedure;

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

        public DataTable GetNotificationsDetail()
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_NOTIFICATIONS_DETAIL");
                cmd.CommandType = CommandType.StoredProcedure;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i][0] = EncryptionHelper.DecryptString(table.Rows[i][0].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                }
                keys = null;

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetMeesageDetail(int idMessage)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_MESSAGE_DETAIL");
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
                    table.Rows[i][0] = EncryptionHelper.DecryptString(table.Rows[i][0].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                    table.Rows[i][1] = EncryptionHelper.DecryptString(table.Rows[i][1].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                    table.Rows[i][2] = EncryptionHelper.DecryptString(table.Rows[i][2].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                }

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
