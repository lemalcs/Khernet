using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// <see cref="TextBlock"/> control that supports selection of text.
    /// </summary>
    public class SelectableTextBlock : TextBlock
    {
        TextRange range;
        Brush backgroundBrush;
        TextPointer selectionStartPosition;
        TextPointer selectionEndPosition;

        public string SelectedText { get; private set; }

        public SelectableTextBlock() : base()
        {
            //Create a context menu
            ContextMenu = new ContextMenu();
            MenuItem i1 = new MenuItem();

            //Command to copy selected text
            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Copy,
                (o, e1) => Clipboard.SetText(SelectedText),
                (p, e2) => { e2.CanExecute = !string.IsNullOrEmpty(SelectedText); }));

            //Create a item to copy command
            i1.Command = ApplicationCommands.Copy;

            ContextMenu.Items.Add(i1);

            //Set cursor to I-Beam when mouse is over this control
            Cursor = Cursors.IBeam;

            //Allow this control to capture focus, this is necessary if there are more focusable controls
            Focusable = true;

            backgroundBrush = App.Current.Resources["SelectedTextColor"] as Brush;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            //Set this control as focused so commands target this one
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), this);

            //Restore default foreground and background colors if there is selected text
            if (range != null)
            {
                range.ApplyPropertyValue(TextElement.BackgroundProperty, null);
                range.ClearAllProperties();
            }

            selectionEndPosition = null;

            //Get initial mouse position to start selection of text
            Point mousePosition = e.GetPosition(this);
            selectionStartPosition = this.GetPositionFromPoint(mousePosition, true);

            if (range == null)
                range = new TextRange(selectionStartPosition, selectionStartPosition);

            // Select all text if double click is made
            if (e.ClickCount > 1 && Inlines.Count > 0)
            {
                range = new TextRange(Inlines.FirstInline.ContentStart, Inlines.LastInline.ContentEnd);
                range.ApplyPropertyValue(TextElement.BackgroundProperty, backgroundBrush);
                SelectedText = range.Text;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (selectionStartPosition == null)
                return;

            //Get current mouse position
            Point mousePosition = e.GetPosition(this);
            selectionEndPosition = this.GetPositionFromPoint(mousePosition, true);

            //Set selected text while mouse is moving
            range.ClearAllProperties();
            range.Select(selectionStartPosition, selectionEndPosition);

            //TextElement.BackgroundProperty default value is null
            //Apply and foreground and background color to selected text
            range.ApplyPropertyValue(TextElement.BackgroundProperty, backgroundBrush);

            //Save current selected text
            SelectedText = range.Text;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            //Set start selection position to null for a new selection
            //when mouse button is released from this control
            selectionStartPosition = null;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            //Set start selection position to null for a new selection
            //when mouse leaves from this control
            selectionStartPosition = null;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            //Clear selection
            selectionStartPosition = null;
            selectionEndPosition = null;
            if (range != null)
            {
                range.ClearAllProperties();
                range = null;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            //Fire Copy command when Ctrl + C combination is pressed
            if (e.Key == Key.C && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                ApplicationCommands.Copy.Execute(null, this);
            }
        }
    }
}
