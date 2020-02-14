using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Khernet.Core.Utility
{
    public class Compressor
    {
        public void UnZipFile(Stream file, string destinationPath)
        {
            using (ZipInputStream zipInput = new ZipInputStream(file))
            {
                ZipEntry entry;
                string directoryName = null;
                string fileName = null;
                while ((entry = zipInput.GetNextEntry()) != null)
                {
                    directoryName = Path.GetDirectoryName(entry.Name);
                    fileName = Path.GetFileName(entry.Name);

                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(Path.Combine(destinationPath, directoryName));

                    if (fileName != string.Empty)
                    {
                        int size = 2048;
                        byte[] data = new byte[size];
                        using (FileStream f = File.Create(Path.Combine(destinationPath, entry.Name)))
                        {
                            while (true)
                            {
                                size = zipInput.Read(data, 0, data.Length);
                                if (size > 0)
                                    f.Write(data, 0, size);
                                else
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
