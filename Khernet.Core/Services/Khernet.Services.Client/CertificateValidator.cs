using Khernet.Core.Utility;
using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Khernet.Services.Client
{
    public class CertificateValidator : X509CertificateValidator
    {
        public static string ISSUER
        {
            get { return "CN=khernet.org"; }
        }
        public override void Validate(X509Certificate2 foreingCertificate)
        {
            try
            {
                bool valid = foreingCertificate.IssuerName.Name == ISSUER;

                string foreingToken = foreingCertificate.SubjectName.Name.Substring(3, 34);

                //Get certificate from application database
                X509Certificate2 certificate = ChannelConfiguration.Finder.GetCertificate(foreingToken);

                valid &= certificate != null;

                if (certificate == null)
                    throw new ArgumentNullException("Unidentified peer");

                valid &= certificate.GetPublicKeyString() == foreingCertificate.GetPublicKeyString();
                valid &= ChannelConfiguration.Finder.ValidateToken(foreingToken, certificate.GetPublicKey());

                if (!valid)
                    throw new SecurityTokenValidationException("Invalid peer credentials");
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }
    }
}
