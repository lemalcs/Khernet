using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Khernet.Core.Utility;
using Khernet.Core.Common;

namespace Khernet.Core.Host
{
    public class Region
    {
        /// <summary>
        /// Validar las base de datos usadas por la aplicación, verificando su existencia y conexion correcta.
        /// </summary>
        /// <returns></returns>
        public bool ValidateDatabase()
        {
            Storage storage = new Storage();

            if (File.Exists(storage.EngineAddress) && File.Exists(storage.ConfigAddress) && File.Exists(storage.RepoAddress))
            {
                bool isValidFile = storage.TryConnectTo(StorageType.Configuration);
                isValidFile &= storage.TryConnectTo(StorageType.Repository);

                return isValidFile;
            }
            return false;
        }

        public void Build()
        {
            if(!ValidateDatabase())
            {
                InstallDatabaseEngine();
            }
            if(!ValidateDatabase())
            {
                CreateDatabases();
            }
        }

        public bool IsInitialized()
        {
            //Verificar si el usuario ya ha sido creado, consultando la tabla ACCOUNT de la base de datos de la aplicación
            Storage storage = new Storage();
            return storage.VerifyInitialization();
        }

        public void InstallDatabaseEngine()
        {
            //Crear carpetas y archivos necesarios del motor de base de datos firebird
            Storage storage = new Storage();
            string firebirdEnginePath = Path.GetDirectoryName(storage.EngineAddress);

            if (!Directory.Exists(firebirdEnginePath))
            {
                Directory.CreateDirectory(firebirdEnginePath);
            }
            
            using (MemoryStream memStream = new MemoryStream(ResourceContainer.FIREBIRD))
            {
                Compressor compressor = new Compressor();
                compressor.UnZipFile(memStream,firebirdEnginePath);
            }
        }

        public void CreateDatabases()
        {
            //Crear las base de datos de configuraciones y de almacenamiento de datos de la aplicación

            Storage storage = new Storage();
            string repositoryPath = Path.GetDirectoryName(storage.RepoAddress);

            if(!Directory.Exists(repositoryPath))
            {
                Directory.CreateDirectory(repositoryPath);
            }

            //Crear base de datos de configuración de la aplicación
            RenameExistingFile(storage.ConfigAddress);//Renombrar si existe algùn archivo con el mismo nombre

            //Archivo de base de datos comprimido en formato ZIP
            using (MemoryStream memStream = new MemoryStream(ResourceContainer.CONFIG_zip))
            {
                Compressor compressor = new Compressor();
                compressor.UnZipFile(memStream, Path.GetDirectoryName(storage.ConfigAddress));

                File.Move(Path.Combine(Path.GetDirectoryName(storage.ConfigAddress), "CONFIG"), storage.ConfigAddress);
            }

            //Crear base de datos de la aplicación
            RenameExistingFile(storage.RepoAddress);//Renombrar si existe algùn archivo con el mismo nombre

            //Archivo de base de datos comprimido en formato ZIP
            using (MemoryStream memStream = new MemoryStream(ResourceContainer.KH_zip))
            {
                Compressor compressor = new Compressor();
                compressor.UnZipFile(memStream, Path.GetDirectoryName(storage.RepoAddress));

                File.Move(Path.Combine(Path.GetDirectoryName(storage.RepoAddress), "KH"), storage.RepoAddress);
            }
        }

        private string RenameExistingFile(string fileName)
        {
            if(File.Exists(fileName))
            {
                string newFileName = fileName;
                for(int counter = 0;File.Exists(newFileName);)
                {
                    counter++;
                    newFileName = Path.Combine(
                        Path.GetDirectoryName(fileName),//Ruta del archivo
                        string.Format("{0} ({1}){2}", 
                                        Path.GetFileNameWithoutExtension(fileName), //nombre del archivo
                                        counter,//Contador para nuevo nombre de archivo
                                        Path.GetExtension(fileName))//extensión del archivo
                        );
                }
                File.Move(fileName, newFileName);
                return newFileName;
            }
            return null;
        }
    }
}
