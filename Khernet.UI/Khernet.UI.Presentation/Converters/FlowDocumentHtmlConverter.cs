using HtmlAgilityPack;
using Khernet.UI.Cache;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Convert HTML code to <see cref="FlowDocument"/> and vice versa.
    /// </summary>
    public class FlowDocumentHtmlConverter
    {
        public string ConvertToHTML(FlowDocument document)
        {
            if (document == null || document.Blocks == null)
                return null;

            StringBuilder sb = new StringBuilder();

            for (int j = 0; j < document.Blocks.Count; j++)
            {
                if (document.Blocks.ElementAt(j) is Paragraph par)
                {
                    sb.Append(ReadInlines(par));
                }
            }

            return sb.ToString();
        }

        public FlowDocument ConvertFromHtml(string htmlDocument)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlDocument);

            var root = html.DocumentNode;

            FlowDocument document = new FlowDocument();

            ReadChildNodes(root, document);

            return document;
        }

        public string ReadInlines(Paragraph block)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<p>");

            sb.Append(InlineCollectionToString(block.Inlines));

            sb.Append("</p>");
            return sb.ToString();
        }

        private string ReadInlinesFromSpan(Span span)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<span>");

            sb.Append(InlineCollectionToString(span.Inlines));

            sb.Append("</span>");
            return sb.ToString();
        }

        private string ReadInlinesFromHyperlink(Hyperlink hyperlink)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<a>");

            sb.Append(InlineCollectionToString(hyperlink.Inlines));

            sb.Append("</a>");
            return sb.ToString();
        }

        private string ReadInlinesFromBold(Bold bold)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<strong>");

            sb.Append(InlineCollectionToString(bold.Inlines));

            sb.Append("</strong>");
            return sb.ToString();
        }

        private string InlineCollectionToString(InlineCollection inlines)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < inlines.Count; i++)
            {
                if (inlines.ElementAt(i) is Run run)
                {
                    sb.Append(WebUtility.HtmlEncode(run.Text));
                }
                else if (inlines.ElementAt(i) is LineBreak)
                {
                    sb.Append("<br>");
                }
                else if (inlines.ElementAt(i) is InlineUIContainer ui)
                {
                    EmojiConverter converter = new EmojiConverter();
                    string emoji = converter.ConvertToString(ui.Tag.ToString());
                    sb.Append($"<img alt=\"{emoji}\" src=\"{ui.Tag}\"/>");
                }
                else if (inlines.ElementAt(i).GetType() == typeof(Span))
                {
                    sb.Append(ReadInlinesFromSpan((Span)inlines.ElementAt(i)));
                }
                else if (inlines.ElementAt(i) is Hyperlink hyperlink)
                {
                    sb.Append(ReadInlinesFromHyperlink(hyperlink));
                }
                else if (inlines.ElementAt(i) is Bold bold)
                {
                    sb.Append(ReadInlinesFromBold(bold));
                }
            }

            return sb.ToString();
        }

        private void ReadChildNodes(HtmlNode node, FlowDocument document, TextElement parent = null)
        {
            foreach (var t in node.ChildNodes)
            {
                string name = t.Name;

                if (t.Name == "p")
                {
                    Paragraph p = new Paragraph();
                    document.Blocks.Add(p);

                    if (t.ChildNodes.Count > 0)
                    {
                        ReadChildNodes(t, document, p);
                    }
                }
                if (t.Name == "span")
                {
                    Span sp = new Span();

                    AddInlineTo(sp, parent);

                    if (t.ChildNodes.Count > 0)
                    {
                        ReadChildNodes(t, null, sp);
                    }
                }
                else if (t.Name == "#text")
                {
                    if (parent == null)
                    {
                        Paragraph p = new Paragraph();
                        document.Blocks.Add(p);
                        parent = p;
                    }
                    AddInlineTo(new Run(WebUtility.HtmlDecode(t.InnerText)), parent);
                }
                else if (t.Name == "strong")
                {
                    Bold bold = new Bold();
                    AddInlineTo(bold, parent);

                    if (t.ChildNodes.Count > 0)
                    {
                        ReadChildNodes(t, null, bold);
                    }
                }
                else if (t.Name == "a")
                {
                    Hyperlink hyperlink = new Hyperlink();

                    AddInlineTo(hyperlink, parent);

                    if (t.ChildNodes.Count > 0)
                    {
                        ReadChildNodes(t, null, hyperlink);
                    }
                }
                else if (t.Name == "img")
                {
                    try
                    {
                        InlineUIContainer inlineUI = new InlineUIContainer();

                        //Create image from emoji code
                        Image emojiImage = new Image();
                        BitmapImage bm = new BitmapImage();
                        bm.BeginInit();
                        bm.UriSource = new Uri($"pack://application:,,,/Khernet.UI.Container;component/{t.Attributes[1].Value}.png");
                        bm.EndInit();

                        emojiImage.Source = bm;

                        emojiImage.Height = 24;
                        emojiImage.Margin = new Thickness(0, 0, 0, 0);
                        emojiImage.VerticalAlignment = VerticalAlignment.Center;

                        inlineUI.Child = emojiImage;
                        inlineUI.BaselineAlignment = BaselineAlignment.Center;

                        inlineUI.Tag = t.Attributes[1].Value;

                        AddInlineTo(inlineUI, parent);
                    }
                    catch (Exception error)
                    {
                        // If emoji image could not be loaded then add that as text
                        Debug.Write(error.Message);
                        EmojiConverter converter = new EmojiConverter();
                        string emoji = converter.ConvertToString(t.Attributes[1].Value);
                        AddInlineTo(new Run(emoji), parent);
                    }
                }
                else if (t.Name == "br")
                {
                    AddInlineTo(new LineBreak(), parent);
                }
            }
        }

        private void AddInlineTo(Inline inline, TextElement parent)
        {
            if (parent is Span)
            {
                ((Span)parent).Inlines.Add(inline);
            }
            else if (parent is Paragraph)
            {
                ((Paragraph)parent).Inlines.Add(inline);
            }
        }
    }
}
