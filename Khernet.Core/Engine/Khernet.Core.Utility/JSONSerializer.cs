using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Khernet.Core.Utility
{
    public static class JSONSerializer<T>
    {
        public static byte[] Serialize(T dataObject)
        {
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                MemoryStream mem = new MemoryStream();
                serializer.WriteObject(mem, dataObject);
                byte[] result = mem.ToArray();
                mem.Close();
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static T DeSerialize(byte[] dataObject)
        {
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                MemoryStream mem = new MemoryStream(dataObject);
                T result = (T)serializer.ReadObject(mem);
                mem.Close();
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
