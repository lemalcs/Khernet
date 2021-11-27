using Khernet.UI.Cache;
using Markdig.Extensions.Emoji;
using Markdig.Renderers;
using Markdig.Renderers.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Khernet.UI.Converters.Emojis
{
    public class EmojiInlineRenderer : WpfObjectRenderer<EmojiInline>
    {
        protected override void Write(WpfRenderer renderer, EmojiInline obj)
        {
            EmojiConverter converter = new EmojiConverter();
            string emojiCode = converter.ConvertUnicodeCodePoint(obj.Content.Text);

            try
            {
                //Create image from emoji code
                Image emojiImage = new Image();
                BitmapImage bm = new BitmapImage();
                bm.BeginInit();
                bm.UriSource = new Uri($"pack://application:,,,/Khernet.UI.Container;component/{emojiCode}.png");
                bm.EndInit();
                emojiImage.Source = bm;
                emojiImage.Height = 24;
                emojiImage.Margin = new Thickness(0, 0, 0, 0);
                emojiImage.VerticalAlignment = VerticalAlignment.Center;

                //Create emoji container
                InlineUIContainer emojiContainer = new InlineUIContainer();

                emojiContainer.Child = emojiImage;
                emojiContainer.BaselineAlignment = BaselineAlignment.Center;

                renderer.WriteInline(emojiContainer);
            }
            catch (Exception error)
            {
                // if there was an error getting the emoji image
                // then add it as text to the markdown message
                Debug.WriteLine(error.Message);
                Run emojiText = new Run(obj.Content.Text);
                renderer.WriteInline(emojiText);
            }
        }
    }
}
