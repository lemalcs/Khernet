using Khernet.UI.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Adds a emoji to a <see cref="FlowDocument"/>
    /// </summary>
    public class EmojiSourceProperty : BaseAttachedProperty<EmojiSourceProperty, UserControl>
    {
        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var control = d as RichTextBox;

            //Check if control is a EmojiPaletteControl
            if (control != null)
            {
                var emojiControl = e.NewValue as EmojiPaletteControl;

                if (emojiControl == null)
                    return;

                emojiControl.SelectedEmoji += (sender, args) => EmojiControl_SelectedEmoji(sender, args, control);
            }
        }

        private void EmojiControl_SelectedEmoji(object sender, SelectedEmojiEventArgs e, RichTextBox richText)
        {
            //Get current insertion position
            TextPointer tp = richText.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);

            //Get current paragraph
            Paragraph masterContainer = tp.Paragraph;

            //Create paragraph if it does not exists yet
            if (masterContainer == null)
            {
                masterContainer = new Paragraph();
                ((FlowDocument)tp.Parent).Blocks.Add(masterContainer);
            }


            //Create emoji container
            InlineUIContainer emojiContainer = new InlineUIContainer();
            emojiContainer.Tag = e.EmojiCode;

            //Create image from emoji code
            Image emojiImage = new Image();
            BitmapImage bm = new BitmapImage();
            bm.BeginInit();
            bm.UriSource = new Uri($"pack://application:,,,/Khernet.UI.Container;component/{e.EmojiCode}.png");
            bm.EndInit();
            emojiImage.Source = bm;

            emojiImage.Height = 20;
            emojiImage.Margin = new Thickness(0, 0, 0, -3);
            emojiImage.VerticalAlignment = VerticalAlignment.Center;

            emojiContainer.Child = emojiImage;

            //Insert emoji at current caret position
            if (richText.CaretPosition.Parent is Run)
            {
                //Get current text before insert emoji
                string forwardText = richText.CaretPosition.GetTextInRun(LogicalDirection.Forward);
                string backwardText = richText.CaretPosition.GetTextInRun(LogicalDirection.Backward);

                Run parent1 = richText.CaretPosition.Parent as Run;

                if (string.IsNullOrEmpty(forwardText) && string.IsNullOrEmpty(backwardText))
                {
                    masterContainer.Inlines.InsertAfter(parent1, emojiContainer);
                }
                else
                {
                    Run backWardInline = new Run(backwardText);
                    masterContainer.Inlines.InsertBefore(parent1, backWardInline);

                    Run forwardInline = new Run(forwardText);
                    masterContainer.Inlines.InsertAfter(parent1, forwardInline);

                    masterContainer.Inlines.InsertAfter(parent1, emojiContainer);

                    masterContainer.Inlines.Remove(parent1);
                }
            }
            else if (richText.CaretPosition.Parent is Paragraph)
            {
                if (richText.CaretPosition.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
                {
                    var p1 = richText.CaretPosition.GetNextContextPosition(LogicalDirection.Forward);
                    masterContainer.Inlines.Add(emojiContainer);

                    p1 = p1.GetInsertionPosition(LogicalDirection.Forward);
                    richText.CaretPosition = p1;
                }
                else
                {
                    if (richText.CaretPosition.IsAtLineStartPosition)
                    {
                        var p1 = richText.CaretPosition.GetNextContextPosition(LogicalDirection.Forward);
                        if (!p1.IsAtLineStartPosition)
                            masterContainer.Inlines.InsertBefore(p1.Parent as Inline, emojiContainer);
                        else
                            masterContainer.Inlines.InsertAfter(p1.Parent as Inline, emojiContainer);

                        return;
                    }
                    else
                    {
                        var p1 = richText.CaretPosition.GetNextContextPosition(LogicalDirection.Backward);
                        if (p1.Parent is FlowDocument)
                            masterContainer.Inlines.InsertAfter(masterContainer.Inlines.FirstInline, emojiContainer);
                        else
                            masterContainer.Inlines.InsertAfter(p1.Parent as Inline, emojiContainer);

                        richText.CaretPosition = p1;
                    }
                }

            }

            // Get next caret position

            var nextContext = richText.CaretPosition.GetNextContextPosition(LogicalDirection.Forward);

            if (nextContext == null)
                return;

            var tempContext = nextContext.GetPointerContext(LogicalDirection.Forward);

            if (tempContext == TextPointerContext.None)
                return;

            if (nextContext.IsAtLineStartPosition)
            {
                nextContext = nextContext.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            else if (nextContext.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
            {
                nextContext = nextContext.GetInsertionPosition(LogicalDirection.Forward);
            }
            else
            {
                nextContext = nextContext.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            richText.CaretPosition = nextContext;

            richText.Focus();
        }
    }
}
