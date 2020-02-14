using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace Khernet.Core.Processor
{
    public class PeerIdentity
    {
        public void Create(string userName, SecureString pass)
        {
            //Validar el nombre de usuario
            //Validar la contraseña
            //GUardar el nombre de usuario
            //Generar el par de claves usando el algoritmo RSA
            //Cifrar las claves RSA con la contrasñe ingresada por el usuario
            //Generar la dirección usando la clave publica
            //Generar una clave aleatoria para cifrado asimétrico AES-256
            //Guardar en la base de datos lo siguiente: usuario, claves RSA, dirección y clave AES-256 
        }

        public string GetPublicKey()
        {
            return null;
        }
        public string BuildAddress(string publicKey)
        {

            return null;
        }

        public bool ValidateAddress(string adress,string publicKey)
        {

            return false;
        }
    }
}
