using FirebirdSql.Data.FirebirdClient;
using Khernet.Core.Common;
using Khernet.Core.Utility;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace Khernet.Core.Data
{
    public class CommunicatorData
    {
        private static string GetConnectionString()
        {
            Storage st = new Storage();
            return st.BuildConnectionString(StorageType.Repository);
        }

        public void ClearPartialMessages()
        {
            try
            {
                FbCommand cmd = new FbCommand("CLEAR_PARTIAL_MESSAGES");
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

        public DataTable GetPeers()
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_PEERS");
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
                    //Username
                    table.Rows[i][0] = EncryptionHelper.DecryptString(table.Rows[i][0].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Token
                    table.Rows[i][1] = EncryptionHelper.DecryptString(table.Rows[i][1].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Full name
                    if (table.Rows[i][2] != DBNull.Value)
                        table.Rows[i][2] = EncryptionHelper.DecryptByteArray(table.Rows[i][2] as byte[], keys.Item1, keys.Item2);

                    //Display name
                    if (table.Rows[i][3] != DBNull.Value)
                        table.Rows[i][3] = EncryptionHelper.DecryptByteArray(table.Rows[i][3] as byte[], keys.Item1, keys.Item2);

                    //Hexadecimal color of profile
                    if (table.Rows[i][4] != DBNull.Value)
                        table.Rows[i][4] = EncryptionHelper.DecryptString(table.Rows[i][4].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Initials
                    if (table.Rows[i][5] != DBNull.Value)
                        table.Rows[i][5] = EncryptionHelper.DecryptString(table.Rows[i][5].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                }
                keys = null;

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Get the address of a specific service published by peer
        /// </summary>
        /// <param name="token">The peer that publish service</param>
        /// <param name="serviceType">The type of service</param>
        /// <returns></returns>
        public DataTable GetPeerAdress(string token, string serviceType)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_PEER_ADDRESS");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@SRVTYPE", FbDbType.Binary).Value = EncryptionHelper.EncryptString(serviceType, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i][0] != DBNull.Value)
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

        public DataTable GetSelfProfile()
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_ACCOUNT_PROFILE");
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
                    //Token
                    if (!string.IsNullOrEmpty(table.Rows[i][0].ToString()))
                        table.Rows[i][0] = EncryptionHelper.DecryptString(table.Rows[i][0].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Username
                    if (!string.IsNullOrEmpty(table.Rows[i][1].ToString()))
                        table.Rows[i][1] = EncryptionHelper.DecryptString(table.Rows[i][1].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Slogan
                    if (!string.IsNullOrEmpty(table.Rows[i][3].ToString()))
                        table.Rows[i][3] = EncryptionHelper.DecryptString(table.Rows[i][3].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Group name
                    if (!string.IsNullOrEmpty(table.Rows[i][4].ToString()))
                        table.Rows[i][4] = EncryptionHelper.DecryptString(table.Rows[i][4].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Full name
                    if (table.Rows[i][5] != DBNull.Value)
                        table.Rows[i][5] = EncryptionHelper.DecryptByteArray((byte[])table.Rows[i][5], keys.Item1, keys.Item2);
                }
                keys = null;

                return table;
            }
            catch (Exception)
            {

                throw;
            }
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

                byte[] avatar = null;

                if (table.Rows.Count > 0)
                {
                    if (table.Rows[0][0] != null && table.Rows[0][0] != DBNull.Value)
                    {
                        var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                        avatar = EncryptionHelper.DecryptByteArray((byte[])table.Rows[0][0], keys.Item1, keys.Item2);
                        keys = null;
                    }
                }


                return avatar;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetPeerProfile(string token)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_PEER_PROFILE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    //User name
                    table.Rows[i][0] = EncryptionHelper.DecryptString(table.Rows[i][0].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Slogan
                    if (!string.IsNullOrEmpty(table.Rows[i][2].ToString()))
                        table.Rows[i][2] = EncryptionHelper.DecryptString(table.Rows[i][2].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Group name
                    if (!string.IsNullOrEmpty(table.Rows[i][3].ToString()))
                        table.Rows[i][3] = EncryptionHelper.DecryptString(table.Rows[i][3].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Full name
                    if (table.Rows[i][4] != DBNull.Value)
                        table.Rows[i][4] = EncryptionHelper.DecryptByteArray(table.Rows[i][4] as byte[], keys.Item1, keys.Item2);

                    //Display name
                    if (table.Rows[i][5] != DBNull.Value)
                        table.Rows[i][5] = EncryptionHelper.DecryptByteArray(table.Rows[i][5] as byte[], keys.Item1, keys.Item2);

                    //Initials
                    if (table.Rows[i][6] != DBNull.Value)
                        table.Rows[i][6] = EncryptionHelper.DecryptString(table.Rows[i][6].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Hexadecimal color
                    if (table.Rows[i][7] != DBNull.Value)
                        table.Rows[i][7] = EncryptionHelper.DecryptString(table.Rows[i][7].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                }
                keys = null;

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SavePeer(string userName, string token, byte[] certificate, string address, string serviceType, string hexColor = null, string initials = null)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_PEER");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@USERNAME", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(userName, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@CERTIFICATE", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(certificate, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@ADDRESS", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(address, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@SRVTYPE", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(serviceType, Encoding.UTF8, keys.Item1, keys.Item2);

                if (!string.IsNullOrEmpty(hexColor) && !string.IsNullOrWhiteSpace(hexColor))
                    cmd.Parameters.Add("@HEX_COLOR", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(hexColor, Encoding.UTF8, keys.Item1, keys.Item2);

                if (!string.IsNullOrEmpty(initials) && !string.IsNullOrWhiteSpace(initials))
                    cmd.Parameters.Add("@INITIALS", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(initials, Encoding.UTF8, keys.Item1, keys.Item2);

                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SavePeerProfile(string token, string userName, string slogan, string group, sbyte state, byte[] fullName)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_PEER_PROFILE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@USERNAME", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(userName, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@STATE", FbDbType.SmallInt).Value = state;

                if (!string.IsNullOrEmpty(slogan) && !string.IsNullOrWhiteSpace(slogan))
                    cmd.Parameters.Add("@SLOGAN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(slogan, Encoding.UTF8, keys.Item1, keys.Item2);
                else
                    cmd.Parameters.Add("@SLOGAN", FbDbType.VarChar).Value = null;

                if (!string.IsNullOrEmpty(group) && !string.IsNullOrWhiteSpace(group))
                    cmd.Parameters.Add("@GROUPNAME", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(group, Encoding.UTF8, keys.Item1, keys.Item2);
                else
                    cmd.Parameters.Add("@GROUPNAME", FbDbType.VarChar).Value = null;

                if (fullName != null)
                    cmd.Parameters.Add("@FULL_NAME", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(fullName, keys.Item1, keys.Item2);
                else
                    cmd.Parameters.Add("@FULL_NAME", FbDbType.Binary).Value = null;

                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SavePeerDisplayName(string token, byte[] displayName)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_PEER_DISPLAY_NAME");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@AVATAR", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(displayName, keys.Item1, keys.Item2);
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SavePeerAvatar(string token, byte[] displayName)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_PEER_AVATAR");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@DISPLAY_NAME", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(displayName, keys.Item1, keys.Item2);
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveProfile(string userName, string slogan, string group, sbyte state, byte[] fullName)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_PROFILE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);

                cmd.Parameters.Add("@USERNAME", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(userName, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@STATE", FbDbType.SmallInt).Value = state;

                if (!string.IsNullOrEmpty(slogan) && !string.IsNullOrWhiteSpace(slogan))
                    cmd.Parameters.Add("@SLOGAN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(slogan, Encoding.UTF8, keys.Item1, keys.Item2);
                else
                    cmd.Parameters.Add("@SLOGAN", FbDbType.VarChar).Value = null;

                if (!string.IsNullOrEmpty(group) && !string.IsNullOrWhiteSpace(group))
                    cmd.Parameters.Add("@GROUPNAME", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(group, Encoding.UTF8, keys.Item1, keys.Item2);
                else
                    cmd.Parameters.Add("@GROUPNAME", FbDbType.VarChar).Value = null;

                if (fullName != null)
                    cmd.Parameters.Add("@FULL_NAME", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(fullName, keys.Item1, keys.Item2);
                else
                    cmd.Parameters.Add("@FULL_NAME", FbDbType.Binary).Value = null;

                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveAvatar(byte[] avatar)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_AVATAR");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@AVATAR", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(avatar, keys.Item1, keys.Item2);
                keys = null;


                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();
                    int result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void UpdatePeerState(string accountToken, short status)
        {
            try
            {
                FbCommand cmd = new FbCommand("UPDATE_PEER_STATE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(accountToken, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@STATE", FbDbType.SmallInt).Value = status;
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
        public static bool VerifyUserExistence(string token)
        {
            try
            {
                DataTable table = new DataTable();
                FbCommand cmd = new FbCommand("VERIFY_USER_EXISTENCE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                bool result = false;

                if (table.Rows.Count > 0)
                {
                    if (table.Rows.Count > 0 && table.Rows[0][0] != null && table.Rows[0][0] != DBNull.Value)
                    {
                        result = (bool)table.Rows[0][0];
                    }
                }

                return result;
            }
            catch (Exception error)
            {

                throw error;
            }
        }

        public byte[] GetPeerAvatar(string token)
        {
            try
            {
                DataTable table = new DataTable();
                FbCommand cmd = new FbCommand("GET_PEER_AVATAR");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

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
                        keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
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

        public void RegisterNotSentMessage()
        {
            try
            {
                FbCommand cmd = new FbCommand("REGISTER_NOT_SENT_MESSAGES");
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

        public int SaveTextMessage(string senderToken, string receiptToken, DateTimeOffset regDate, byte[] content,
            int contentType, string uid, long timeId, int? idReplyMessage, byte[] thumbnail = null,
            string filePath = null)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_TEXT_MESSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@SENDER_TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(senderToken, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@RECEIPT_TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(receiptToken, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@REG_DATE", FbDbType.Date).Value = regDate;
                cmd.Parameters.Add("@CONTENT", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(content, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@CONTENT_TYPE", FbDbType.Integer).Value = contentType;
                cmd.Parameters.Add("@UID", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(uid, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@TIMEID", FbDbType.BigInt).Value = timeId;

                if (idReplyMessage.HasValue)
                    cmd.Parameters.Add("@ID_REPLY", FbDbType.Integer).Value = idReplyMessage.Value;
                else
                    cmd.Parameters.Add("@ID_REPLY", FbDbType.Integer).Value = null;

                if (thumbnail != null)
                    cmd.Parameters.Add("@THUMBNAIL", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(thumbnail, keys.Item1, keys.Item2);

                if (!string.IsNullOrEmpty(filePath))
                    cmd.Parameters.Add("@FILE_PATH", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(Encoding.UTF8.GetBytes(filePath), keys.Item1, keys.Item2);

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


        public int GetIdMessage(string uid)
        {
            try
            {
                FbCommand cmd = new FbCommand("GET_ID_MESSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@UID_MESSAGE", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(uid, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

                DataTable table = new DataTable();

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                if (table.Rows.Count > 0)
                {
                    return Convert.ToInt32(table.Rows[0][0]);
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long GetTimeIdMessage(int id)
        {
            try
            {
                FbCommand cmd = new FbCommand("GET_TIMEID_MESSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = id;
                keys = null;

                DataTable table = new DataTable();

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                if (table.Rows.Count > 0)
                {
                    return Convert.ToInt64(table.Rows[0][0]);
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetUIDMessage(int idMessage)
        {
            try
            {
                FbCommand cmd = new FbCommand("GET_UID_MESSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = idMessage;
                keys = null;

                DataTable table = new DataTable();

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                if (table.Rows.Count > 0)
                {
                    string uid = null;
                    if (table.Rows[0][0] != DBNull.Value)
                    {
                        keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                        uid = EncryptionHelper.DecryptString(table.Rows[0][0].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                        keys = null;
                    }
                    return uid;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SavePartialTextMessage(string uid, int sequential, byte[] content)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_PARTIAL_MESSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@ID", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(uid, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@SEQ", FbDbType.Integer).Value = sequential;
                cmd.Parameters.Add("@CONTENT", FbDbType.Binary).Value = EncryptionHelper.EncryptByteArray(content, keys.Item1, keys.Item2);
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

        public DataTable GetPartialTextMessage(string idMessage)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_PARTIAL_MESSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@ID", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(idMessage, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    //Decrypt Content
                    if (table.Rows[i][1] != DBNull.Value)
                        table.Rows[i][1] = EncryptionHelper.DecryptByteArray(table.Rows[i][1] as byte[], keys.Item1, keys.Item2);
                }
                keys = null;

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeletePartialTextMessage(string uid)
        {
            try
            {
                FbCommand cmd = new FbCommand("DELETE_PARTIAL_MESSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@ID", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(uid, Encoding.UTF8, keys.Item1, keys.Item2);
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

        public void RegisterPendingMessage(string receiptToken, int idMessage)
        {
            try
            {
                FbCommand cmd = new FbCommand("REGISTER_PENDING_MESSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@RECEIPT_TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(receiptToken, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.SmallInt).Value = idMessage;
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

        public void MarkAsReadMessage(int idMessage)
        {
            try
            {
                FbCommand cmd = new FbCommand("MARK_AS_READ_MESSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = idMessage;
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

        public void BulkMarkAsReadMessage(int lastIdUnreadMessage)
        {
            try
            {
                FbCommand cmd = new FbCommand("BULK_MARK_AS_READ_MESSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@LAST_MESSAGE_ID", FbDbType.Integer).Value = lastIdUnreadMessage;
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

        /// <summary>
        /// Gets the pending message of given user.
        /// </summary>
        /// <param name="receiptToken">The token of receipt user</param>
        /// <param name="quantity">The number of pending message to retrieve, send 0 to get all messages.</param>
        /// <returns>The list of id messages</returns>
        public DataTable GetPendingMessageOfUser(string receiptToken, int quantity)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_PENDING_MESSAGES_OF_USER");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@RECEIPT_TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(receiptToken, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@quantity", FbDbType.Integer).Value = quantity;
                keys = null;

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

        /// <summary>
        /// Gets the list of message to be requested to sender peer.
        /// </summary>
        /// <param name="receiptToken">The token of user that sent message</param>
        /// <param name="quantity">The number of pending message to retrieve, send 0 to get all messages.</param>
        /// <returns>The list of id messages</returns>
        public DataTable GetRequestPendingMessageForUser(string senderToken, int quantity)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_REQUEST_PENDING_MESSAGES");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@SENDER_TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(senderToken, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@quantity", FbDbType.Integer).Value = quantity;
                keys = null;

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

        public DataTable GetMessageContent(int idMessage)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_MESSAGE_CONTENT");
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
                    table.Rows[i][0] = EncryptionHelper.DecryptByteArray(table.Rows[i][0] as byte[], keys.Item1, keys.Item2);
                }

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetMessageDetail(int idMessage)
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
                    //Sender token
                    table.Rows[i][0] = EncryptionHelper.DecryptString(table.Rows[i][0].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Receipt token
                    table.Rows[i][1] = EncryptionHelper.DecryptString(table.Rows[i][1].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //Content
                    table.Rows[i][2] = EncryptionHelper.DecryptByteArray(table.Rows[i][2] as byte[], keys.Item1, keys.Item2);

                    //UID
                    if (table.Rows[i][4] != DBNull.Value)
                        table.Rows[i][4] = EncryptionHelper.DecryptString(table.Rows[i][4].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                    //State
                    if (table.Rows[i][8] == DBNull.Value)
                        table.Rows[i][8] = 0;
                }

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeletePendingMessage(string receiptToken, int idMessage)
        {
            try
            {
                FbCommand cmd = new FbCommand("DELETE_PENDING_MESSSAGE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@RECEIPT_TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(receiptToken, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = idMessage;
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

        public void ClearPeers()
        {
            try
            {
                FbCommand cmd = new FbCommand("CLEAR_PEERS");
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

        public void ClearPeersState()
        {
            try
            {
                FbCommand cmd = new FbCommand("CLEAR_PEER_STATE");
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

        public DataTable GetPeerServAddress(string token, string serviceType)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_PEER_SERV_ADDRESS");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@SERV_TYPE", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(serviceType, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
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

        public DataTable GetConnectedPeers(string serviceType)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_CONNECTED_PEERS");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@SERV_TYPE", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(serviceType, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i][0] = EncryptionHelper.DecryptString(table.Rows[i][0].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                    table.Rows[i][1] = EncryptionHelper.DecryptString(table.Rows[i][1].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                }
                keys = null;

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetPeerCertificate(string token)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_PEER_CERTIFICATE");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

                using (cmd.Connection = new FbConnection(GetConnectionString()))
                {
                    cmd.Connection.Open();

                    FbDataAdapter fda = new FbDataAdapter(cmd);
                    fda.Fill(table);
                }

                keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i][0] = EncryptionHelper.DecryptByteArray((byte[])table.Rows[i][0], keys.Item1, keys.Item2);
                }
                keys = null;

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetUnreadMessages(string senderToken)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_UNREAD_MESSAGES");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@SENDER_TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(senderToken, Encoding.UTF8, keys.Item1, keys.Item2);
                keys = null;

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
                        if (table.Rows[i][4] != DBNull.Value)
                            table.Rows[i][4] = EncryptionHelper.DecryptString(table.Rows[i][4].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                    }
                    keys = null;
                }

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetMessageHeader(int idMessage)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_MESSAGES_HEADER");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = idMessage;
                keys = null;

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
                        if (table.Rows[i][4] == DBNull.Value)
                            table.Rows[i][4] = 0;

                        //UID
                        if (table.Rows[i][5] != DBNull.Value)
                            table.Rows[i][5] = EncryptionHelper.DecryptString(table.Rows[i][5].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                    }
                    keys = null;
                }

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetLastMessages(string senderToken, bool forward, long lastTimeIdMessage, int quantity)
        {
            try
            {
                DataTable table = new DataTable();

                FbCommand cmd = new FbCommand("GET_LAST_MESSAGES");
                cmd.CommandType = CommandType.StoredProcedure;

                var keys = EncryptionHelper.UnpackAESKeys(Obfuscator.Key);
                cmd.Parameters.Add("@SENDER_TOKEN", FbDbType.VarChar).Value = EncryptionHelper.EncryptString(senderToken, Encoding.UTF8, keys.Item1, keys.Item2);
                cmd.Parameters.Add("@LAST_TIMEID_MESSAGE", FbDbType.BigInt).Value = lastTimeIdMessage;
                cmd.Parameters.Add("@FORWARD", FbDbType.Boolean).Value = forward;
                cmd.Parameters.Add("@QUANTITY", FbDbType.Integer).Value = quantity;
                keys = null;

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
                        if (table.Rows[i][4] == DBNull.Value)
                            table.Rows[i][4] = 0;

                        //UID
                        if (table.Rows[i][6] != DBNull.Value)
                            table.Rows[i][6] = EncryptionHelper.DecryptString(table.Rows[i][6].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                    }
                }

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetMessageState(int idMessage, int messageState)
        {
            try
            {
                FbCommand cmd = new FbCommand("SET_MESSAGE_STATE");
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ID_MESSAGE", FbDbType.Integer).Value = idMessage;
                cmd.Parameters.Add("@STATE", FbDbType.Integer).Value = messageState;

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
    }
}
