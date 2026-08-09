using System.Collections.ObjectModel;

namespace Khernet.UI.IoC
{
    /// <summary>
    /// Interface to manage emojis.
    /// </summary>
    internal interface IEmojiPalette
    {
        ObservableCollection<Emoji> EmojiList { get; }

        /// <summary>
        /// Adds a emoji on recent used group.
        /// </summary>
        /// <param name="code">The <see cref="Emoji.Code"/>.</param>
        void AddRecentUsedEmoji(string code);

        /// <summary>
        /// Loads a list of emojis in recent used group.
        /// </summary>
        /// <param name="emojisList">The <see cref="Emoji.Code"/>.</param>
        void LoadRecentUsedEmojis();

        /// <summary>
        /// Saves the list of emojis in recent used group.
        /// </summary>
        void SaveRecentUsedEmojis();
    }
}
