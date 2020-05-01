namespace Khernet.UI
{
    /// <summary>
    /// Base class for text messages such as include HTML, markdown and XAML formats.
    /// </summary>
    public abstract class TextMessageItemViewModel : ChatMessageItemViewModel
    {
        /// <summary>
        /// Gets a copy of this message
        /// </summary>
        /// <returns></returns>
        public abstract ChatMessageItemViewModel GetInstanceCopy();
    }
}
