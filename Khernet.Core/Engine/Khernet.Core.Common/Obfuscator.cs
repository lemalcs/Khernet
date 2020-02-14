using Khernet.Core.Utility;
using System.Security;

namespace Khernet.Core.Common
{
    public static class Obfuscator
    {
        public static SecureString Key { get; private set; }
        public static void SetKey(SecureString password)
        {
            Key = EncryptionHelper.PackAESKeys(password);
        }
    }
}
