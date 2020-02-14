using System;
using System.Security.Cryptography;

namespace Khernet.Core.Utility
{
    public class PublicKeyCryptographyProvider
    {
        private string hashAlgorithm;

        public PublicKeyCryptographyProvider()
        {
            hashAlgorithm = "SHA256";
        }

        public string GenerateXmlKey(int keySize, bool includePrivateKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
                return rsa.ToXmlString(includePrivateKey);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Tuple<string, string> GenerateXmlKey(int keySize)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);

                return new Tuple<string, string>(
                    rsa.ToXmlString(false), //Get public key
                    rsa.ToXmlString(true) //Get private key
                    );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public byte[] EncryptResource(byte[] data, string publicKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicKey);
                byte[] result = rsa.Encrypt(data, true);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public byte[] DecryptResource(byte[] data, string privateKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privateKey);
                byte[] result = rsa.Decrypt(data, true);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public byte[] SignResource(byte[] data, string key)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(key);

                RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(rsa);
                formatter.SetHashAlgorithm(hashAlgorithm);
                CryptographyProvider cp = new CryptographyProvider();
                byte[] hash = cp.GetHashDigest(data);
                byte[] signature = formatter.CreateSignature(hash);
                return signature;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool VerifySignature(byte[] messageHash, byte[] signature, string key)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(key);

                RSAPKCS1SignatureDeformatter deFormatter = new RSAPKCS1SignatureDeformatter(rsa);
                deFormatter.SetHashAlgorithm(hashAlgorithm);
                return deFormatter.VerifySignature(messageHash, signature);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
