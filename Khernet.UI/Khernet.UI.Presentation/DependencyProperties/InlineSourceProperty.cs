using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Creates a <see cref="Inline"/> array from a <see cref="FlowDocument"/> byte collection
    /// </summary>
    public class InlineSourceProperty : BaseAttachedProperty<InlineSourceProperty, ReadOnlyCollection<byte>>
    {
        /// <summary>
        /// The current number of characters
        /// </summary>
        private int symbolsCount = 0;

        /// <summary>
        /// The maximun number of characters to take
        /// </summary>
        private readonly int symbolsCountLimit = 32;

        /// <summary>
        /// The textbloxk that will contain the read characters
        /// </summary>
        private TextBlock textBlock;

        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var control = d as TextBlock;

            //Check if control is a TextBlock
            if (control != null)
            {
                //Returns a stream based on byte array (FlowDocument from RichTextbox)
                if (e.NewValue == null || ((ReadOnlyCollection<byte>)e.NewValue).Count == 0)
                    return;

                SetInlines(control, (ReadOnlyCollection<byte>)e.NewValue);
            }
        }

        private void SetInlines(TextBlock control, ReadOnlyCollection<byte> document)
        {
            FlowDocument fw = new FlowDocument();

            TextRange range = new TextRange(fw.ContentStart, fw.ContentEnd);
            using (MemoryStream textContent = new MemoryStream(document.ToArray()))
            {
                range.Load(textContent, DataFormats.XamlPackage);
            }

            if (range.Start.Paragraph != null)
            {
                symbolsCount = 0;

                control.Inlines.Clear();
                textBlock = control;

                //Read the first 20 characters of first line
                ReadTextInlines(range.Start.Paragraph);

                textBlock = null;
            }
        }

        private void ReadTextInlines(Paragraph block)
        {
            TextInlineCollectionToString(block.Inlines);
        }

        private void TextInlineCollectionToString(InlineCollection inlines)
        {
            for (int i = 0; i < inlines.Count; i++)
            {
                if (inlines.ElementAt(i) is Run run)
                {
                    if (run.Text.Length + symbolsCount >= symbolsCountLimit)
                    {
                        Run newElement = new Run(run.Text.Substring(0, symbolsCountLimit - symbolsCount) + "...");
                        textBlock.Inlines.Add(newElement);

                        break;
                    }
                    else
                    {
                        Run newText = new Run(run.Text);

                        textBlock.Inlines.Add(newText);
                        symbolsCount = run.Text.Length;
                    }
                }
                else if (inlines.ElementAt(i) is LineBreak)
                {
                    return;
                }
                else if (inlines.ElementAt(i) is InlineUIContainer ui)
                {
                    if (symbolsCount + 1 <= symbolsCountLimit)
                    {
                        ImageSource img = ((Image)ui.Child).Source.Clone();
                        Image t = new Image();
                        t.Source = img;
                        t.Height = 20;

                        InlineUIContainer newElement = new InlineUIContainer(t);
                        newElement.BaselineAlignment = BaselineAlignment.Center;
                        textBlock.Inlines.Add(newElement);
                        symbolsCount++;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (inlines.ElementAt(i).GetType() == typeof(Span))
                {
                    ReadTextInlinesFromSpan((Span)inlines.ElementAt(i));
                }
                else if (inlines.ElementAt(i) is Hyperlink hyperlink)
                {
                    ReadTextInlinesFromHyperlink(hyperlink);
                }
                else if (inlines.ElementAt(i) is Bold bold)
                {
                    ReadTextInlinesFromBold(bold);
                }
            }
        }

        private void ReadTextInlinesFromSpan(Span span)
        {
            TextInlineCollectionToString(span.Inlines);
        }

        private void ReadTextInlinesFromHyperlink(Hyperlink hyperlink)
        {
            TextInlineCollectionToString(hyperlink.Inlines);
        }

        private void ReadTextInlinesFromBold(Bold bold)
        {
            TextInlineCollectionToString(bold.Inlines);
        }
    }
}
