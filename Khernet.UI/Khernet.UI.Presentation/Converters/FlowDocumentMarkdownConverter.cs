using Khernet.UI.Cache;
using Khernet.UI.Converters.Emojis;
using Markdig;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Convert Markdown code to <see cref="FlowDocument"/> and viceversa
    /// </summary>
    public class FlowDocumentMarkdownConverter
    {
        public string GetMarkDownText(FlowDocument document)
        {
            if (document == null || document.Blocks == null)
                return null;

            StringBuilder sb = new StringBuilder();

            for (int j = 0; j < document.Blocks.Count; j++)
            {
                if (document.Blocks.ElementAt(j) is Paragraph par)
                {
                    sb.Append(ReadTextInlines(par));
                    sb.Append("\n\n");
                }
            }

            return sb.ToString();
        }

        public FlowDocument ConvertToFlowDocument(string markdownText)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions()
                .UseEmphasisExtras()
                .UseGridTables()
                .UsePipeTables()
                .UseTaskLists()
                .UseAutoLinks()
                .UseEmojiAndSmiley()
                .ConfigureNewLine("\n")
                .Build();

            EmojiWpfRenderer rend = new EmojiWpfRenderer();

            FlowDocument document = Markdig.Wpf.Markdown.ToFlowDocument(markdownText, pipeline, rend);

            return document;
        }

        public string ReadTextInlines(Paragraph block)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TextInlineCollectionToString(block.Inlines));

            return sb.ToString();
        }

        private string TextInlineCollectionToString(InlineCollection inlines)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < inlines.Count; i++)
            {
                if (inlines.ElementAt(i) is Run run)
                {
                    sb.Append(run.Text.Replace("\r\n","  \n"));
                }
                else if (inlines.ElementAt(i) is LineBreak)
                {
                    sb.Append("  \n");
                }
                else if (inlines.ElementAt(i) is InlineUIContainer ui)
                {
                    EmojiConverter emojiConverter = new EmojiConverter();
                    string emojiValue = emojiConverter.ConvertToString(ui.Tag.ToString());

                    EmojiMarkdown emojiMarkdown = new EmojiMarkdown();
                    string markdownCode = emojiMarkdown.GetEmojiPrefix(emojiValue);
                    if (markdownCode == null)
                        markdownCode = emojiValue;

                    sb.Append(markdownCode + "\n");
                }
                else if (inlines.ElementAt(i).GetType() == typeof(Span))
                {
                    sb.Append(ReadTextInlinesFromSpan((Span)inlines.ElementAt(i)));
                }
                else if (inlines.ElementAt(i) is Hyperlink hyperlink)
                {
                    sb.Append(ReadTextInlinesFromHyperlink(hyperlink));
                }
                else if (inlines.ElementAt(i) is Bold bold)
                {
                    sb.Append(ReadTextInlinesFromBold(bold));
                }
            }

            return sb.ToString();
        }

        private string ReadTextInlinesFromSpan(Span span)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(TextInlineCollectionToString(span.Inlines));

            return sb.ToString();
        }

        private string ReadTextInlinesFromHyperlink(Hyperlink hyperlink)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(TextInlineCollectionToString(hyperlink.Inlines));

            return sb.ToString();
        }

        private string ReadTextInlinesFromBold(Bold bold)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(TextInlineCollectionToString(bold.Inlines));

            return sb.ToString();
        }
    }
}
