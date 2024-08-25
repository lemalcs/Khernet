using Khernet.Core.Common;
using Khernet.Core.Entity;
using Khernet.Core.Processor;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using System;
using System.Security;

namespace Khernet.Core.Host
{
    public class Authenticator
    {
        public void CreateUser(string userName, SecureString password)
        {
            try
            {
                AccountManager peerIdentity = new AccountManager();
                peerIdentity.Create(userName, password);

                //Delete key of internal communication service because it is exclusively for this application
                Configuration.SetValue(Constants.ListenerKey, string.Empty);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public PeerIdentity Login(string user, SecureString password)
        {
            try
            {
                AccountManager accountMan = new AccountManager();

                Storage storage = new Storage();

                Configuration.SetPassword(password);
                string tempKey = Configuration.GetValue(Constants.Fingerprint);
                SecureString fingerprintKey = new SecureString();
                for (int i = 0; i < tempKey.Length; i++)
                {
                    fingerprintKey.AppendChar(tempKey[i]);
                }
                tempKey = null;

                Configuration.SetPassword(fingerprintKey);

                tempKey = Configuration.GetValue(Constants.ApplicationKey);

                SecureString applicationtKey = new SecureString();
                for (int i = 0; i < tempKey.Length; i++)
                {
                    applicationtKey.AppendChar(tempKey[i]);
                }
                tempKey = null;

                if (applicationtKey.Length > 0)
                {
                    Configuration.SetValue(Constants.ListenerKey, string.Empty);

                    SaveListenerKey();

                    SecureString appKey = EncryptionHelper.PackAESKeys(applicationtKey);

                    PeerIdentity peer = accountMan.GetToken(appKey);

                    AccountManager accountManager = new AccountManager();
                    if (accountManager.DecodeUserName(peer.UserName) != user)
                        throw new Exception();

                    accountMan.SetAccountState(PeerState.Online);

                    //Key to encrypt the application database
                    Obfuscator.SetKey(applicationtKey);

                    //It will be used certificate authentication between server and client, both must 
                    //use their certificates to be recognized as a valid peers on network

                    //The user must have access to private key
                    ChannelConfiguration.SetClientCertificate(peer.Certificate);
                    ChannelConfiguration.SetFinder(new PeerFinder());

                    return peer;
                }
                return null;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw new Exception("Authentication error");
            }
        }

        /// <summary>
        /// Generate and save an random key to authenticate internal communication services. It will be different
        /// every time user log in and not equal to empty.
        /// </summary>
        private void SaveListenerKey()
        {
            //Generate random key and save it in application database
            CryptographyProvider cryptoProvider = new CryptographyProvider();
            byte[] listenerKey = cryptoProvider.GenerateRandonNumbers(32);

            Configuration.SetValue(Constants.ListenerKey, cryptoProvider.GetBase58Check(listenerKey));
        }

        public void Logout()
        {
            try
            {
                //Delete key for internal communication service
                Configuration.SetValue(Constants.ListenerKey, string.Empty);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void RememberCredentials(string userName, SecureString password)
        {
            AccountManager accountManager = new AccountManager();
            accountManager.SaveCredentials(userName, password);
        }

        public void ForgetCredentials()
        {
            AccountManager accountManager = new AccountManager();
            accountManager.RemoveCredentials();
        }

        public Tuple<string, SecureString> RetrieveCredentials()
        {
            AccountManager accountManager = new AccountManager();
            return Tuple.Create(accountManager.GetUserName(), accountManager.GetPassword());
        }
    }
}
