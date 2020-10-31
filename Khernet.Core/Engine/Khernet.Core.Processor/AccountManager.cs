using Khernet.Core.Common;
using Khernet.Core.Data;
using Khernet.Core.Entity;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using X509Certificate2 = System.Security.Cryptography.X509Certificates.X509Certificate2;
using X509ContentType = System.Security.Cryptography.X509Certificates.X509ContentType;
using X509KeyStorageFlags = System.Security.Cryptography.X509Certificates.X509KeyStorageFlags;

namespace Khernet.Core.Processor
{
    public class AccountManager
    {
        public void Create(string userName, SecureString pass)
        {
            //Validate user name
            //Validate password
            //Generate a key pair using RSA algorithm
            //Generate a symmetric key using AES-256 from user password
            //Encrypt RSA keys with generated key
            //Generate the address using public key
            //Save on database the following: user name, RAS keys, address and AES-256 key

            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
                throw new Exception("Invalid user name");

            if (pass == null || pass.Length == 0)
                throw new ArgumentNullException("Password cannot be empty");

            //Generate the alternate key to encrypt configuration database
            CryptographyProvider cryptoProvider = new CryptographyProvider();
            byte[] salt = cryptoProvider.GenerateRandonNumbers(64);

            string alternateKey = Convert.ToBase64String(cryptoProvider.GenerateKey(pass, 64, 4096, salt));
            salt = null;

            SecureString configKeyContainer = new SecureString();
            for (int i = 0; i < alternateKey.Length; i++)
            {
                configKeyContainer.AppendChar(alternateKey[i]);
            }
            alternateKey = null;

            Storage storage = new Storage();

            Configuration.SetPassword(pass, storage.BuildConnectionString(StorageType.Configuration));
            Configuration.SetValue(Constants.Fingerprint, cryptoProvider.RetrieveString(configKeyContainer));

            //This key will be used to encrypt values of configurations except this key
            Configuration.SetPassword(configKeyContainer, storage.BuildConnectionString(StorageType.Configuration));

            //Generate key to encrypt application database
            salt = cryptoProvider.GenerateRandonNumbers(64);
            alternateKey = Convert.ToBase64String(cryptoProvider.GenerateKey(configKeyContainer, 64, 4096, salt));
            salt = null;

            SecureString aplicationKeyContainer = new SecureString();
            for (int i = 0; i < alternateKey.Length; i++)
            {
                aplicationKeyContainer.AppendChar(alternateKey[i]);
            }
            alternateKey = null;

            Configuration.SetValue(Constants.ApplicationKey, cryptoProvider.RetrieveString(aplicationKeyContainer));

            SecureString applicationKey = EncryptionHelper.PackAESKeys(aplicationKeyContainer);
            var keys = EncryptionHelper.UnpackAESKeys(applicationKey);

            string encodedUserName = EncodeUserName(userName);

            string token = null;
            var certificate = CreateSelfSignedCertificate(
                        CertificateValidator.ISSUER,
                        ref token,
                        null,
                        new[] { KeyPurposeID.IdKPServerAuth },
                        encodedUserName);

            byte[] obfuscatedCert = EncryptionHelper.EncryptByteArray(certificate.Export(X509ContentType.Pfx, applicationKey), keys.Item1, keys.Item2);
            applicationKey = null;

            //Generate TOKEN for account: string of letters and numbers to identify uniquely this user
            string obfuscatedToken = EncryptionHelper.EncryptString(token, Encoding.UTF8, keys.Item1, keys.Item2);
            string obfuscatedUserName = EncryptionHelper.EncryptString(encodedUserName, Encoding.UTF8, keys.Item1, keys.Item2);
            keys = null;

            //Save on application database
            AccountManagerData accountData = new AccountManagerData();
            accountData.SaveAccount(
                obfuscatedUserName,
                obfuscatedToken,
                obfuscatedCert
                );
        }

        public string EncodeUserName(string userName)
        {
            CryptographyProvider cryptoProvider = new CryptographyProvider();
            string result = cryptoProvider.GetBase58Check(Encoding.UTF8.GetBytes(userName));
            return result;
        }

        public string DecodeUserName(string encodedUserName)
        {
            CryptographyProvider cryptoProvider = new CryptographyProvider();
            string result = Encoding.UTF8.GetString(cryptoProvider.DecodeBase58Check(encodedUserName));
            return result;
        }

        /// <summary>
        /// Generate token from public key encoded with BASE58Check.
        /// </summary>
        /// <param name="publicKey">The public key of user.</param>
        /// <returns>Encoded token with BASE58Check.</returns>
        public string BuildToken(string publicKey)
        {
            CryptographyProvider cryptoProvider = new CryptographyProvider();
            return BuildToken(cryptoProvider.DecodeBase58Check(publicKey));
        }

        public string BuildToken(byte[] publicKey)
        {
            if (publicKey == null || publicKey.Length == 0)
                throw new Exception("Invalid public key");

            CryptographyProvider cryptoProvider = new CryptographyProvider();

            string hash1 = cryptoProvider.GetHash(publicKey);
            string ripemd160 = cryptoProvider.GetRIPEMD160Hash(Encoding.UTF8.GetBytes(hash1));
            string hash2 = string.Concat("4B", ripemd160);
            string hash3 = cryptoProvider.GetHash(Encoding.UTF8.GetBytes(hash2));
            string hash4 = cryptoProvider.GetHash(Encoding.UTF8.GetBytes(hash3));
            string result = string.Concat(hash2, hash4.Substring(0, 8));

            byte[] tempToken = new byte[result.Length / 2];

            for (int i = 0, j = 0; j < tempToken.Length; i += 2, j++)
            {
                tempToken[j] = Convert.ToByte(result.Substring(i, 2), 16);
            }

            return cryptoProvider.GetBase58Check(tempToken);
        }

        public bool ValidateToken(string token, byte[] publicKey)
        {
            CryptographyProvider crypto = new CryptographyProvider();

            byte[] token1 = crypto.DecodeBase58Check(token);
            byte[] token2 = crypto.DecodeBase58Check(BuildToken(publicKey));

            bool result = token1.Length == token2.Length;

            for (int i = 0; i < token1.Length && i < token2.Length; i++)
            {
                result &= token1[i] == token2[i];
            }

            return result;
        }

        public PeerIdentity GetToken(SecureString passWd)
        {
            AccountManagerData accountData = new Data.AccountManagerData();
            DataTable result = accountData.GetToken();

            if (result.Rows.Count > 0)
            {
                var keys = EncryptionHelper.UnpackAESKeys(passWd);

                string userName = EncryptionHelper.DecryptString(result.Rows[0][0].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);
                string token = EncryptionHelper.DecryptString(result.Rows[0][1].ToString(), Encoding.UTF8, keys.Item1, keys.Item2);

                byte[] certData = EncryptionHelper.DecryptByteArray((byte[])result.Rows[0][2], keys.Item1, keys.Item2);
                keys = null;

                X509Certificate2 certificate = new X509Certificate2(certData, passWd, X509KeyStorageFlags.Exportable);
                certificate.FriendlyName = userName;

                PeerIdentity peer = new PeerIdentity(userName, token, certificate);

                return peer;
            }
            return null;
        }

        public void SetAccountState(PeerState state)
        {
            AccountManagerData accountData = new Data.AccountManagerData();
            accountData.SetAccountState((sbyte)state);
        }

        private X509Certificate2 CreateSelfSignedCertificate(string issuer, ref string subjectName, string[] subjectAlternativeNames, KeyPurposeID[] usages, string friendlyName)
        {
            // It's self-signed, so these are the same.
            var issuerName = issuer;

            var random = GetSecureRandom();
            var subjectKeyPair = GenerateKeyPair(random, 2048);

            //Generate user token from public key of X509 certificate
            SubjectPublicKeyInfo info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectKeyPair.Public);
            byte[] publicKey = info.PublicKeyData.GetBytes();

            //Generate user token
            string token = BuildToken(publicKey);
            subjectName = token;

            // It's self-signed, so these are the same.
            var issuerKeyPair = subjectKeyPair;

            var serialNumber = GenerateSerialNumber(random);
            var issuerSerialNumber = serialNumber; // Self-signed, so it's the same serial number.

            const bool isCertificateAuthority = false;
            var certificate = GenerateCertificate(random, string.Format("CN={0}", token), subjectKeyPair, serialNumber,
                                                  subjectAlternativeNames, issuerName, issuerKeyPair,
                                                  issuerSerialNumber, isCertificateAuthority,
                                                  usages);
            return ConvertCertificate(friendlyName, certificate, subjectKeyPair, random);
        }

        private static Org.BouncyCastle.X509.X509Certificate GenerateCertificate(SecureRandom random,
                                                           string subjectName,
                                                           AsymmetricCipherKeyPair subjectKeyPair,
                                                           BigInteger subjectSerialNumber,
                                                           string[] subjectAlternativeNames,
                                                           string issuerName,
                                                           AsymmetricCipherKeyPair issuerKeyPair,
                                                           BigInteger issuerSerialNumber,
                                                           bool isCertificateAuthority,
                                                           KeyPurposeID[] usages)
        {
            var certificateGenerator = new X509V3CertificateGenerator();

            certificateGenerator.SetSerialNumber(subjectSerialNumber);

            // Set the signature algorithm. This is used to generate the thumbprint which is then signed
            // with the issuer's private key. We'll use SHA-256, which is (currently) considered fairly strong.

            var issuerDN = new X509Name(issuerName);
            certificateGenerator.SetIssuerDN(issuerDN);

            // Note: The subject can be omitted if you specify a subject alternative name (SAN).
            var subjectDN = new X509Name(subjectName);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Our certificate needs valid from/to values.
            var notBefore = DateTime.UtcNow.Date;
            var notAfter = notBefore.AddYears(2);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // The subject's public key goes in the certificate.
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            AddAuthorityKeyIdentifier(certificateGenerator, issuerDN, issuerKeyPair, issuerSerialNumber);
            AddSubjectKeyIdentifier(certificateGenerator, subjectKeyPair);
            AddBasicConstraints(certificateGenerator, isCertificateAuthority);

            if (usages != null && usages.Any())
                AddExtendedKeyUsage(certificateGenerator, usages);

            if (subjectAlternativeNames != null && subjectAlternativeNames.Any())
                AddSubjectAlternativeNames(certificateGenerator, subjectAlternativeNames);

            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom securerandom = new SecureRandom(randomGenerator);

            ISignatureFactory signFactory = new Asn1SignatureFactory("SHA256WithRSA", issuerKeyPair.Private, securerandom);

            // The certificate is signed with the issuer's private key.
            var certificate = certificateGenerator.Generate(signFactory);
            return certificate;
        }

        private static System.Security.Cryptography.X509Certificates.X509Certificate2 ConvertCertificate(string friendlyName, Org.BouncyCastle.X509.X509Certificate certificate,
                                                         AsymmetricCipherKeyPair subjectKeyPair,
                                                         SecureRandom random)
        {
            // Now to convert the Bouncy Castle certificate to a .NET certificate.
            // See http://web.archive.org/web/20100504192226/http://www.fkollmann.de/v2/post/Creating-certificates-using-BouncyCastle.aspx
            // ...but, basically, we create a PKCS12 store (a .PFX file) in memory, and add the public and private key to that.
            var store = new Pkcs12Store();

            // What Bouncy Castle calls "alias" is the same as what Windows terms the "friendly name".

            // Add the certificate.
            var certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);

            // Add the private key.
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { certificateEntry });

            // Convert it to an X509Certificate2 object by saving/loading it from a MemoryStream.
            // It needs a password. Since we'll remove this later, it doesn't particularly matter what we use.
            const string password = "password";
            var stream = new MemoryStream();
            store.Save(stream, password.ToCharArray(), random);

            var convertedCertificate =
                new X509Certificate2(stream.ToArray(),
                                     password,
                                     X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            return convertedCertificate;
        }

        private static SecureRandom GetSecureRandom()
        {
            // Since we're on Windows, we'll use the CryptoAPI one (on the assumption
            // that it might have access to better sources of entropy than the built-in
            // Bouncy Castle ones):
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            return random;
        }

        private static BigInteger GenerateSerialNumber(SecureRandom random)
        {
            var serialNumber =
                BigIntegers.CreateRandomInRange(
                    BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            return serialNumber;
        }

        private static AsymmetricCipherKeyPair GenerateKeyPair(SecureRandom random, int strength)
        {
            var keyGenerationParameters = new KeyGenerationParameters(random, strength);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            return subjectKeyPair;
        }

        private static void AddAuthorityKeyIdentifier(X509V3CertificateGenerator certificateGenerator,
                                                      X509Name issuerDN,
                                                      AsymmetricCipherKeyPair issuerKeyPair,
                                                      BigInteger issuerSerialNumber)
        {
            var authorityKeyIdentifierExtension =
                new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerKeyPair.Public),
                    new GeneralNames(new GeneralName(issuerDN)),
                    issuerSerialNumber);
            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id, false, authorityKeyIdentifierExtension);
        }

        /// <summary>
        /// Add the "Subject Alternative Names" extension. Note that you have to repeat the value from the "Subject Name" property.
        /// </summary>
        /// <param name="certificateGenerator">The generator of certificate.</param>
        /// <param name="subjectAlternativeNames">List of alternative subject names.</param>
        private static void AddSubjectAlternativeNames(X509V3CertificateGenerator certificateGenerator,
                                                       IEnumerable<string> subjectAlternativeNames)
        {
            var subjectAlternativeNamesExtension =
                new DerSequence(
                    subjectAlternativeNames.Select(name => new GeneralName(GeneralName.DnsName, name))
                                           .ToArray<Asn1Encodable>());

            certificateGenerator.AddExtension(
                X509Extensions.SubjectAlternativeName.Id, false, subjectAlternativeNamesExtension);
        }

        private static void AddExtendedKeyUsage(X509V3CertificateGenerator certificateGenerator, KeyPurposeID[] usages)
        {
            certificateGenerator.AddExtension(
                X509Extensions.ExtendedKeyUsage.Id, false, new ExtendedKeyUsage(usages));
        }

        /// <summary>
        /// Add the "Basic Constraints" extension.
        /// </summary>
        /// <param name="certificateGenerator">The generator of certificate.</param>
        /// <param name="isCertificateAuthority">Indicates whether it is a certificate authority.</param>
        private static void AddBasicConstraints(X509V3CertificateGenerator certificateGenerator,
                                                bool isCertificateAuthority)
        {
            certificateGenerator.AddExtension(
                X509Extensions.BasicConstraints.Id, true, new BasicConstraints(isCertificateAuthority));
        }

        /// <summary>
        /// Add the Subject Key Identifier.
        /// </summary>
        /// <param name="certificateGenerator">The generator of certificate.</param>
        /// <param name="subjectKeyPair">The private and public key pair.</param>
        private static void AddSubjectKeyIdentifier(X509V3CertificateGenerator certificateGenerator,
                                                    AsymmetricCipherKeyPair subjectKeyPair)
        {
            var subjectKeyIdentifierExtension =
                new SubjectKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectKeyPair.Public));
            certificateGenerator.AddExtension(
                X509Extensions.SubjectKeyIdentifier.Id, false, subjectKeyIdentifierExtension);
        }
    }
}
