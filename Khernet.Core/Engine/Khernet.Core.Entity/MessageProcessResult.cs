using Khernet.Services.Messages;

namespace Khernet.Core.Entity
{
    public class MessageProcessResult
    {
        /// <summary>
        /// The id of message
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The result of process
        /// </summary>
        public MessageState Result { get; private set; }

        public MessageProcessResult(int id, MessageState result)
        {
            Id = id;
            Result = result;
        }
    }
}
