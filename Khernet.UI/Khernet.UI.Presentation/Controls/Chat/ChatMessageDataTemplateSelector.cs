using System.Windows;
using System.Windows.Controls;

namespace Khernet.UI
{
    /// <summary>
    /// Template selector for hat messages.
    /// </summary>
    public class ChatMessageDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Select a template based on types of item.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data bound object.</param>
        /// <returns>A <see cref="DataTemplate"/> object with selected template.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //If item is null return null template
            if (item != null)
            {
                //Template for text messages
                if (item is TextChatMessageViewModel)
                {
                    return Application.Current.FindResource("TextDataTemplate") as DataTemplate;
                }

                //Template for images messages
                if (item is ImageChatMessageViewModel)
                {
                    return Application.Current.FindResource("ImageDataTemplate") as DataTemplate;
                }

                //Template for video messages
                if (item is AnimationChatMessageViewModel)
                {
                    return Application.Current.FindResource("AnimationDataTemplate") as DataTemplate;
                }

                //Template for video messages
                if (item is VideoChatMessageViewModel)
                {
                    return Application.Current.FindResource("VideoDataTemplate") as DataTemplate;
                }

                //Template for audio messages
                if (item is AudioChatMessageViewModel)
                {
                    return Application.Current.FindResource("AudioDataTemplate") as DataTemplate;
                }

                //Template for audio messages
                if (item is FileChatMessageViewModel)
                {
                    return Application.Current.FindResource("FileDataTemplate") as DataTemplate;
                }

                //Template for HTML messages
                if (item is HtmlChatMessageViewModel)
                {
                    return Application.Current.FindResource("HtmlDataTemplate") as DataTemplate;
                }

                //Template for markdown messages
                if (item is MarkdownChatMessageViewModel)
                {
                    return Application.Current.FindResource("MarkdownDataTemplate") as DataTemplate;
                }
            }

            return null;
        }
    }
}