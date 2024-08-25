using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Khernet.Core.Utility
{
    public class CryptographyProvider
    {
        byte[] salt = new byte[] {
            112,  98,  67,  123,  46,  190,  184,  94,  202,  16,
            190,  145,  172,  93,  14,  188,  104,  200,  183,  104,
            53,  105,  134,  2,  246,  218,  253,  204,  154,  56,
            203,  239,  34,  35,  17,  154,  166,  182,  238,  39,
            96,  71,  90,  123,  170,  131,  229,  149,  164,  12,
            97,  84,  66,  27,  175,  11,  64,  69,  200,  173,
            203,  22,  18,  42,  247,  129,  12,  60,  1,  36,
            0,  10,  107,  112,  67,  216,  185,  208,  128,  227,
            96,  118,  77,  223,  247,  246,  75,  39,  184,  33,
            250,  95,  199,  243,  83,  16,  246,  142,  183,  113,
            66,  250,  65,  3,  33,  74,  231,  158,  153,  135,
            177,  45,  149,  39,  56,  158,  238,  21,  97,  28,
            247,  37,  45,  178,  80,  224,  16,  181
        };

        public CryptographyProvider()
        {
        }

        public byte[] GenerateKey(SecureString pass, int length, int iterations, byte[] saltData)
        {
            if (saltData == null)
                throw new ArgumentNullException("Salt must not be empty");

            Rfc2898DeriveBytes derive = new Rfc2898DeriveBytes(RetrieveString(pass), saltData);
            derive.IterationCount = iterations;
            return derive.GetBytes(length);
        }

        public byte[] GenerateKey(SecureString pass, int length)
        {
            return GenerateKey(pass, length, 2600, salt);
        }

        public int GenerateRandonNumber()
        {
            RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider();
            byte[] data = new byte[4];
            rg.GetBytes(data);
            int result = BitConverter.ToInt32(data, 0);
            return result;
        }

        public byte[] GenerateRandonNumbers(int quantityNumbers)
        {
            if (quantityNumbers <= 0)
                throw new ArgumentException("Quantity of numbers to generate must be equals or greater than zero");

            RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider();
            byte[] data = new byte[quantityNumbers];
            rg.GetBytes(data);

            return data;
        }

        public byte[] EncryptResource(byte[] source, byte[] Key, byte[] IV)
        {
            try
            {
                RijndaelManaged rm = new RijndaelManaged();

                MemoryStream memData = new MemoryStream();

                CryptoStream crypt = new CryptoStream(
                    memData,
                    rm.CreateEncryptor(Key, IV),
                    CryptoStreamMode.Write);
                crypt.Write(source, 0, source.Length);

                IV = null;
                Key = null;

                crypt.Close();
                memData.Close();

                byte[] encryptedData = memData.ToArray();

                return encryptedData;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public byte[] DecryptResource(byte[] source, byte[] Key, byte[] IV)
        {
            try
            {
                RijndaelManaged rmCryptAlg = new RijndaelManaged();
                MemoryStream msSource = new MemoryStream(source);

                CryptoStream decryptorStream = new CryptoStream(
                    msSource,
                    rmCryptAlg.CreateDecryptor(Key, IV),
                    CryptoStreamMode.Read);

                MemoryStream decryptedStream = new MemoryStream();
                byte[] temp = new byte[1000];
                int readBytes = decryptorStream.Read(temp, 0, temp.Length);
                do
                {
                    decryptedStream.Write(temp, 0, readBytes);
                    readBytes = decryptorStream.Read(temp, 0, temp.Length);
                } while (readBytes > 0);

                IV = null;
                Key = null;

                decryptedStream.Close();
                decryptorStream.Close();
                msSource.Close();
                byte[] decryptedResult = decryptedStream.ToArray();
                return decryptedResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CryptoStream EncryptStream(Stream data, byte[] key, byte[] IV)
        {
            RijndaelManaged rmCryptAlg = new RijndaelManaged();
            CryptoStream crypt = new CryptoStream(data, rmCryptAlg.CreateEncryptor(key, IV), CryptoStreamMode.Read);
            return crypt;
        }

        public CryptoStream DecryptStream(Stream data, byte[] key, byte[] IV)
        {
            RijndaelManaged rmCryptAlg = new RijndaelManaged();
            CryptoStream crypt = new CryptoStream(data, rmCryptAlg.CreateDecryptor(key, IV), CryptoStreamMode.Read);
            return crypt;
        }

        public string GetHash(byte[] data)
        {
            try
            {
                //BitConverter generates groups of 2 hexadecimal digits separated by commas
                return BitConverter.ToString(GetHashDigest(data)).Replace("-", "");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public byte[] GetHashDigest(byte[] data)
        {
            try
            {
                SHA256Managed sha = new SHA256Managed();

                byte[] hash = sha.ComputeHash(data);
                return hash;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetRIPEMD160Hash(byte[] data)
        {
            try
            {
                RIPEMD160 hashBuilder = RIPEMD160.Create();
                byte[] hashResult = hashBuilder.ComputeHash(data);

                return BitConverter.ToString(hashResult).Replace("-", "");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetBase58Check(byte[] data)
        {
            return Base58Check.Base58CheckEncoding.EncodePlain(data);
        }

        public byte[] DecodeBase58Check(string data)
        {
            return Base58Check.Base58CheckEncoding.DecodePlain(data);
        }

        public string RetrieveString(SecureString secString)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Encrypts data with the current logged user credentials.
        /// </summary>
        /// <param name="data">An array that contains the data to encrypt.</param>
        /// <param name="entropy">The entropy to increase the complexity of encryption.</param>
        /// <returns>The encrypted data</returns>
        public byte[] EncryptWithUserKey(byte[] data, byte[] entropy = null)
        {
            byte[] encryptedData = ProtectedData.Protect(
            data,
            entropy,
            DataProtectionScope.CurrentUser);

            return encryptedData;
        }

        /// <summary>
        /// Decrypt data with the current logged user credentials.
        /// </summary>
        /// <param name="data">An array that contains the data to decrypt.</param>
        /// <param name="entropy">The entropy used for encryption.</param>
        /// <returns>The decrypted data.</returns>
        public byte[] DecryptWithUserKey(byte[] data, byte[] entropy = null)
        {
            byte[] encryptedData = ProtectedData.Unprotect(
            data,
            entropy,
            DataProtectionScope.CurrentUser);

            return encryptedData;
        }
    }
}
