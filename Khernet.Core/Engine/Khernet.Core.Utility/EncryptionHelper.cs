using System;
using System.Linq;
using System.Security;
using System.Text;

namespace Khernet.Core.Utility
{
    public class EncryptionHelper
    {
        /// <summary>
        /// Generate key and intialization vector from a password
        /// </summary>
        /// <param name="password">Source password</param>
        /// <returns></returns>
        public static SecureString PackAESKeys(SecureString password)
        {
            try
            {
                CryptographyProvider crypto = new CryptographyProvider();
                byte[] keys = new byte[48];
                crypto.GenerateKey(password, 32).CopyTo(keys, 0);
                crypto.GenerateKey(password, 16).CopyTo(keys, 32);

                SecureString keysContainer = new SecureString();
                string tempKey = Convert.ToBase64String(keys);

                keys = null;
                for (int i = 0; i < tempKey.Length; i++)
                {
                    keysContainer.AppendChar(tempKey[i]);
                }
                tempKey = null;

                return keysContainer;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public static Tuple<byte[], byte[]> UnpackAESKeys(SecureString password)
        {
            try
            {
                CryptographyProvider crypto = new CryptographyProvider();

                byte[] tempKey = Convert.FromBase64String(crypto.RetrieveString(password));

                Tuple<byte[], byte[]> keys = new Tuple<byte[], byte[]>(tempKey.Take(32).ToArray(), tempKey.Skip(32).ToArray());
                tempKey = null;

                return keys;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }
        public static string EncryptString(string data, Encoding encoding, byte[] IV, byte[] Key)
        {
            try
            {
                CryptographyProvider crypto = new CryptographyProvider();
                byte[] result = crypto.EncryptResource(encoding.GetBytes(data), IV, Key);
                Key = null;
                IV = null;

                return Convert.ToBase64String(result);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public static byte[] EncryptByteArray(byte[] data, byte[] key, byte[] IV)
        {
            try
            {
                CryptographyProvider crypto = new CryptographyProvider();
                byte[] result = crypto.EncryptResource(data, key, IV);
                key = null;
                IV = null;

                return result;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public static string DecryptString(string data, Encoding encoding, byte[] IV, byte[] Key)
        {
            try
            {
                CryptographyProvider crypto = new CryptographyProvider();
                byte[] result = crypto.DecryptResource(Convert.FromBase64String(data), IV, Key);
                Key = null;
                IV = null;

                return encoding.GetString(result);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public static byte[] DecryptByteArray(byte[] data, byte[] key, byte[] IV)
        {
            try
            {
                CryptographyProvider crypto = new CryptographyProvider();
                byte[] result = crypto.DecryptResource(data, key, IV);
                key = null;
                IV = null;

                return result;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }
    }
}
