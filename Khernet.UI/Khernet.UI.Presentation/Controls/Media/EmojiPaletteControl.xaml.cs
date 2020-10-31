using System;
using System.Windows.Controls;

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

        public EmojiPaletteControl()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Emoji item = e.AddedItems[0] as Emoji;
                OnSelectedEmoji(item.Code);

                //Clear selected index so a new emoji can be selected
                listEmojis.SelectedIndex = -1;
            }
        }

        protected void OnSelectedEmoji(string emojiCode)
        {
            SelectedEmoji?.Invoke(this, new SelectedEmojiEventArgs(emojiCode));
        }
    }
}
