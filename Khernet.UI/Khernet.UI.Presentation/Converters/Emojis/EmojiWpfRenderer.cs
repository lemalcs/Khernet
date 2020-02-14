using Markdig.Renderers;

namespace Khernet.UI.Converters.Emojis
{
    public class EmojiWpfRenderer : WpfRenderer
    {
        protected override void LoadRenderers()
        {
            ObjectRenderers.Add(new EmojiInlineRenderer());
            base.LoadRenderers();
        }
    }
}
