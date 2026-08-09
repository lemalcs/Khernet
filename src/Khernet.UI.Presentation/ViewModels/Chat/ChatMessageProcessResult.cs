namespace Khernet.UI
{
    /// <summary>
    /// The state of messages.
    /// </summary>
    public class ChatMessageProcessResult
    {
        public int Id { get; private set; }
        public ChatMessageState State { get; private set; }

        public ChatMessageProcessResult(int id, ChatMessageState state)
        {
            Id = id;
            State = state;
        }
    }
}
