using Khernet.Core.Utility;
using System.Security;

namespace Khernet.Core.Common
{
    /// <summary>
    /// Holds the key used to encrypt and decrypt data from database.
    /// </summary>
    public static class Obfuscator
    {
        public static SecureString Key { get; private set; }
        public static void SetKey(SecureString password)
        {
            Key = EncryptionHelper.PackAESKeys(password);
        }
    }
}
