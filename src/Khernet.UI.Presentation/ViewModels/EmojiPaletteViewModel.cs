using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Khernet.UI
{
    /// <summary>
    /// View model to manage emojis.
    /// </summary>
    public class EmojiPaletteViewModel : BaseModel, IEmojiPalette
    {
        /// <summary>
        /// The list of emojis.
        /// </summary>
        private readonly ObservableCollection<Emoji> emojiList;

        public ObservableCollection<Emoji> EmojiList => emojiList;

        /// <summary>
        /// The number of emojis in recent used group.
        /// </summary>
        private int recentEmojiCount = 0;

        /// <summary>
        /// The maximum number of emojis in recent used group.
        /// </summary>
        private const int maxEmojiCount = 18;

        public EmojiPaletteViewModel()
        {
            emojiList = new EmojiListCollection();
        }

        /// <summary>
        /// Adds a emoji on recent used group.
        /// </summary>
        /// <param name="code">The <see cref="Emoji.Code"/>.</param>
        public void AddRecentUsedEmoji(string code)
        {
            // Search if emoji is already on recent used group
            var emj = EmojiList.FirstOrDefault(
                (emoji) =>
                {
                    return emoji.Code == code && emoji.Category == EmojiCategory.Recent;
                });

            // Add emoji to the recent used group
            if (emj == null)
            {
                emj = EmojiList.FirstOrDefault(
                (emoji) =>
                {
                    return emoji.Code == code;
                });

                if (emj != null)
                {
                    // Remove the last emoji from recent used group 
                    // if maximum number is reached
                    if (recentEmojiCount == maxEmojiCount)
                    {
                        EmojiList.RemoveAt(maxEmojiCount - 1);
                        recentEmojiCount--;
                    }

                    EmojiList.Insert(0, new Emoji { Code = emj.Code, Description = emj.Description, Category = EmojiCategory.Recent });
                    recentEmojiCount++;
                }
            }
        }

        /// <summary>
        /// Loads a list of emojis in recent used group.
        /// </summary>
        /// <param name="emojisList">The <see cref="Emoji.Code"/>.</param>
        public void LoadRecentUsedEmojis()
        {
            try
            {
                string[] recentUsedEmojisList = IoCContainer.Get<Messenger>().LoadRecentUsedEmojis();
                if (recentUsedEmojisList == null || recentUsedEmojisList.Length == 0)
                    return;

                for (int i = recentUsedEmojisList.Length - 1, added = 0; added < maxEmojiCount && i >= 0; i--, added++)
                {
                    AddRecentUsedEmoji(recentUsedEmojisList[i]);
                }
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
            }
        }

        /// <summary>
        /// Gets the list of recent used emojis if exists.
        /// </summary>
        /// <returns></returns>
        public IList<Emoji> GetRecentUsedEmojis()
        {
            return EmojiList.Where((emoji) => emoji.Category == EmojiCategory.Recent).ToList();
        }

        /// <summary>
        /// Saves the list of emojis in recent used group.
        /// </summary>
        public void SaveRecentUsedEmojis()
        {
            try
            {
                Emoji[] recentUsedEmojis = EmojiList.Where((emoji) => emoji.Category == EmojiCategory.Recent).ToArray();
                List<string> emojis = new List<string>(recentUsedEmojis.Count());
                foreach (Emoji emoji in recentUsedEmojis)
                {
                    emojis.Add(emoji.Code);
                }
                IoCContainer.Get<Messenger>().SaveRecentUsedEmojiList(emojis.ToArray());
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
            }
        }
    }
}
