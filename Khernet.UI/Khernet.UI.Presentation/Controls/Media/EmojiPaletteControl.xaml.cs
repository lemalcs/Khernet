using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Palette for emojis.
    /// </summary>
    public partial class EmojiPaletteControl : UserControl
    {
        /// <summary>
        /// Fired when a emoji is selected from list.
        /// </summary>
        public event EventHandler<SelectedEmojiEventArgs> SelectedEmoji;

        /// <summary>
        /// The <see cref="ScrollViewer"/> owned by <see cref="ListBox"/> container.
        /// </summary>
        private ScrollViewer scrollViewer;

        public EmojiPaletteControl()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Emoji item = e.AddedItems[0] as Emoji;
                (DataContext as EmojiPaletteViewModel).AddRecentUsedEmoji(item.Code);
                OnSelectedEmoji(item.Code);

                //Clear selected index so a new emoji can be selected
                emojiControl.SelectedIndex = -1;
            }
        }

        protected void OnSelectedEmoji(string emojiCode)
        {
            SelectedEmoji?.Invoke(this, new SelectedEmojiEventArgs(emojiCode));
        }

        private void emojiControl_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if(!emojiControl.IsLoaded)
                return;

            if (scrollViewer == null)
                scrollViewer = FindVisualChild<ScrollViewer>(emojiControl);

            // Scroll to the top of the emojis list whenever the palette is showed
            if (scrollViewer != null)
                scrollViewer.ScrollToHome();
        }

        /// <summary>
        /// Search for an element of a certain type in the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of element to find.</typeparam>
        /// <param name="visual">The parent element.</param>
        /// <returns></returns>
        private T FindVisualChild<T>(Visual visual) where T : Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (child != null)
                {
                    T correctlyTyped = child as T;
                    if (correctlyTyped != null)
                    {
                        return correctlyTyped;
                    }

                    T descendent = FindVisualChild<T>(child);
                    if (descendent != null)
                    {
                        return descendent;
                    }
                }
            }

            return null;
        }
    }
}
