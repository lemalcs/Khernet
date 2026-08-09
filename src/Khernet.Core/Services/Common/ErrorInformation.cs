using System.Runtime.Serialization;

namespace Khernet.Services.Common
{
    [DataContract]
    public class ErrorInformation
    {
        [DataMember]
        public bool Result
        {
            get;
            set;
        }

        [DataMember]
        public int ErrorCode
        {
            get;
            set;
        }

        [DataMember]
        public string ExceptionName
        {
            get;
            set;
        }

        [DataMember]
        public string Message
        {
            get;
            set;
        }

        [DataMember]
        public string Source
        {
            get;
            set;
        }
    }
}