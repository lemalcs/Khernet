using FirebirdSql.Data.FirebirdClient;
using Khernet.Core.Common;
using System;
using System.Data;

namespace Khernet.Core.Data
{
    public class AccountManagerData
    {
        private string GetConnectionString()
        {
            Storage st = new Storage();
            return st.BuildConnectionString(StorageType.Repository);
        }

        public void SaveAccount(string username, string token, byte[] certificate)
        {
            try
            {
                FbCommand cmd = new FbCommand("SAVE_ACCOUNT");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@USER_NAME", FbDbType.VarChar).Value = username;
                cmd.Parameters.Add("@TOKEN", FbDbType.VarChar).Value = token;
                cmd.Parameters.Add("@CERT", FbDbType.Binary).Value = certificate;

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

        public DataTable GetToken()
        {
            try
            {
                DataTable table = new DataTable();
                FbDataAdapter fda = new FbDataAdapter("GET_ACCOUNT_TOKEN", GetConnectionString());
                fda.SelectCommand.CommandType = CommandType.StoredProcedure;

                fda.Fill(table);

                return table;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void SetAccountState(sbyte state)
        {
            try
            {
                FbCommand cmd = new FbCommand("SET_ACCOUNT_STATE");
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@STATE", FbDbType.SmallInt).Value = state;

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
