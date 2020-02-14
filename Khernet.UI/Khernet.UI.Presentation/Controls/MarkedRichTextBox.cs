using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Khernet.UI.Controls
{
    public class MarkedRichTextBox : RichTextBox
    {
        /// <summary>
        /// Sets watermark text for this control
        /// </summary>
        public string WaterMark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }

        // The dependencyProperty as the backing store for WaterMark.
        public static readonly DependencyProperty WaterMarkProperty =
            DependencyProperty.Register(nameof(WaterMark), typeof(string), typeof(MarkedRichTextBox), new PropertyMetadata(null));

        /// <summary>
        /// Indicates if this control has a document
        /// </summary>
        public bool HasDocument
        {
            get { return (bool)GetValue(HasDocumentProperty); }
        }

        // The dependencyProperty as the backing store for HasDocument.
        internal static readonly DependencyPropertyKey HasDocumentPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasDocument), typeof(bool), typeof(MarkedRichTextBox), new PropertyMetadata(false));

        public static readonly DependencyProperty HasDocumentProperty = HasDocumentPropertyKey.DependencyProperty;

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            var rtxt = this;

            bool hasContent = true;

            //Verify if this RichtextBox control has content or not
            if (rtxt.Document != null && (rtxt.Document.Blocks != null && rtxt.Document.Blocks.Count == 0))
            {
                hasContent = false;
            }
            else
            {
                for (int i = 0; i < e.Changes.Count; i++)
                {
                    if (e.Changes.ElementAt(i).AddedLength > 0)
                    {
                        hasContent = true;
                        break;
                    }
                }

                if (rtxt.Document.Blocks.Count == 1)
                {
                    if (rtxt.Document.Blocks.FirstBlock.GetType() == typeof(Paragraph))
                    {
                        var par = rtxt.Document.Blocks.FirstBlock as Paragraph;

                        if (par.Inlines.Count == 0)
                        {
                            hasContent = false;
                        }
                        else if (par.Inlines.Count == 1)
                        {
                            if (par.Inlines.FirstInline.GetType() == typeof(Run))
                            {
                                var text = par.Inlines.FirstInline as Run;
                                if (string.IsNullOrEmpty(text.Text))
                                {
                                    hasContent = false;
                                }
                            }
                        }
                    }
                }
            }

            //Set HasDocument property to true if there are changes otherwise set false
            SetValue(HasDocumentPropertyKey, hasContent);
            base.OnTextChanged(e);
        }
    }
}
