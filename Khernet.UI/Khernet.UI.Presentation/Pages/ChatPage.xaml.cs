using Khernet.UI.Controls;
using Khernet.UI.Converters;
using Khernet.UI.Media;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// Current chat page.
    /// </summary>
    public partial class ChatPage : BasePage<ChatMessageListViewModel>, IDocumentContainer
    {
        /// <summary>
        /// Line height for chat mesage
        /// </summary>
        double lineheight = 20;

        public ChatPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with an specific view model
        /// </summary>
        /// <param name="viewModel">The view model</param>
        public ChatPage(ChatMessageListViewModel viewModel) : base(viewModel)
        {
            viewModel.ScrollToCurrentContent = ScrollToCurrentContent;
            viewModel.GetContent = GetMessageContent;
            viewModel.SetContent = SetMessageContent;
            viewModel.ScrollToChatMessage = ScrollToChatMessage;

            InitializeComponent();
        }

        /// <summary>
        /// Capture keys for textbox and control new line insertions and message sending
        /// </summary>
        /// <param name="sender">The textbox</param>
        /// <param name="e">Arguments for event</param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textbox = sender as MarkedRichTextBox;

            if (e.Key == Key.Escape)
                return;

            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    //Adds new line to text
                    var lineBreak = textbox.CaretPosition.InsertLineBreak();

                    //Moves de cursor forward to the new line
                    if (!lineBreak.IsAtLineStartPosition)
                        textbox.CaretPosition = textbox.CaretPosition.Paragraph.ContentEnd;
                    else
                        textbox.CaretPosition = lineBreak;
                }
                else if (textbox.HasDocument)
                {
                    //Send message if Ctrl Key is not pressed

                    SpecificViewModel.Send(GetDocument(SpecificViewModel.MessageFormat));
                    ClearCurrentMessage();
                    chatList.ScrollToEnd();
                }
                else if (SpecificViewModel.CanSendMessage)
                {
                    SpecificViewModel.Send((byte[])null);
                    chatList.ScrollToEnd();
                }

                //Mark this event as handled so next control cannot capture it again
                e.Handled = true;
            }

            //Send writing message to peer every 3 seconds after a key is pressed
            else
            {
                SpecificViewModel.SendWritingMessage();
            }
        }

        private void ClearCurrentMessage()
        {
            rtxt.BeginChange();
            rtxt.Document.Blocks.Clear();
            rtxt.EndChange();
        }

        private void PasteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                var textbox = sender as MarkedRichTextBox;

                //Get text from clipboard
                string clipboardText = Clipboard.GetText();

                //Get the cursor of text at start line
                var firstPos = textbox.CaretPosition.GetLineStartPosition(0);

                //Get Number or symbols(characters) from line start to current position
                var symbols = firstPos.GetOffsetToPosition(textbox.CaretPosition);

                //Adds new line to text
                textbox.CaretPosition.InsertTextInRun(clipboardText);

                var pos = firstPos.GetPositionAtOffset(symbols + clipboardText.Length);

                //Set new position of caret
                if (pos != null)
                    textbox.CaretPosition = pos;
            }
            else if (Clipboard.ContainsFileDropList())
            {
                //Get file paths from clipboard
                var list = Clipboard.GetFileDropList();
                string[] filePaths = new string[list.Count];
                list.CopyTo(filePaths, 0);

                //Send file messages
                SpecificViewModel.Send(filePaths);

                chatList.ScrollToEnd();
            }
            else if (Clipboard.ContainsImage())
            {
                //Send image
                using (var mem = Clipboard.GetData(DataFormats.Dib) as MemoryStream)
                {
                    SpecificViewModel.Send(mem);
                }
                chatList.ScrollToEnd();
            }
        }

        private void PasteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Check if there is data on clipboard
            e.CanExecute = Clipboard.ContainsText()
                || Clipboard.ContainsAudio()
                || Clipboard.ContainsFileDropList()
                || Clipboard.ContainsImage();
        }

        private void ChatMessageListControl_Drop(object sender, DragEventArgs e)
        {
            //Ckeck if there is a file to drop over this control
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //Get files path
                string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop);

                //Send files to chat
                SpecificViewModel.Send(fileList);

                chatList.ScrollToEnd();
            }
        }

        private void Rtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as MarkedRichTextBox;

            if (SpecificViewModel != null)
                SpecificViewModel.SetHasMessage(textbox.HasDocument);
            else
                return;

            //Set line height
            if (textbox.Document != null && textbox.Document.LineHeight != lineheight)
            {
                textbox.Document.LineHeight = lineheight;
            }

            ActivateMarkdownPreview();
        }

        /// <summary>
        /// Renders a preview of markdown message
        /// </summary>
        private void ActivateMarkdownPreview()
        {
            if (!IsLoaded)
                return;

            if (SpecificViewModel.MessageFormat == MessageType.Markdown)
            {
                FlowDocumentMarkdownConverter converter = new FlowDocumentMarkdownConverter();
                string markText = converter.GetMarkDownText(rtxt.Document);

                var flow = converter.ConvertToFlowDocument(markText);
                flow.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#roboto");
                flow.FontSize = 12.5;
                rtxt_Tmp.Document = flow;
            }
        }

        public void ClearContent()
        {
            ClearCurrentMessage();
        }

        public void ScrollToCurrentContent()
        {
            chatList.ScrollToEnd();
        }

        private void MediaGalleryControl_SelectedGIF(object sender, SelectedGIFEventArgs e)
        {
            SpecificViewModel.SendAnimation(e.AnimationModel as GIFItemViewModel, this);
            ScrollToCurrentContent();
        }

        private void MouseBinding_Click(object sender, RoutedEventArgs e)
        {
            chatList.ScrollToEnd();
        }

        private byte[] GetMessageContent()
        {
            return GetHtmlMessage(rtxt.Document);
        }

        private void SetMessageContent(byte[] message)
        {
            ClearContent();
            if (message == null)
            {
                return;
            }

            SetTextMessage(message);
        }

        /// <summary>
        /// Get a text message in HTML format.
        /// </summary>
        /// <param name="document">The <see cref="FlowDocument"/> that contains text message</param>
        /// <returns>A byte array containing HTML string encoded in UTF-8</returns>
        private byte[] GetHtmlMessage(FlowDocument document)
        {
            if (document == null)
                return null;

            if (document.Blocks.Count == 0)
                return null;

            FlowDocumentHtmlConverter converter = new FlowDocumentHtmlConverter();
            string htmlResult = converter.ConvertToHTML(document);

            byte[] result = Encoding.UTF8.GetBytes(htmlResult);

            return result;
        }

        /// <summary>
        /// Get a text message in markdown format.
        /// </summary>
        /// <param name="document">The <see cref="FlowDocument"/> that contains text message</param>
        /// <returns>A byte array containing markdown string encoded in UTF-8</returns>
        private byte[] GetMarkdownTextMessage(FlowDocument document)
        {
            if (document == null)
                return null;

            if (document.Blocks.Count == 0)
                return null;

            FlowDocumentMarkdownConverter converter = new FlowDocumentMarkdownConverter();
            string htmlResult = converter.GetMarkDownText(document);

            byte[] result = Encoding.UTF8.GetBytes(htmlResult);

            return result;
        }

        private void SetTextMessage(byte[] message)
        {
            if (message == null)
            {
                return;
            }

            try
            {
                FlowDocumentHtmlConverter converter = new FlowDocumentHtmlConverter();
                FlowDocument fw = converter.ConvertFromHtml(Encoding.UTF8.GetString(message));
                rtxt.Document = fw;
                
                //Check is preview has to be activated for markdown message
                if (SpecificViewModel.MessageFormat == MessageType.Markdown)                
                    ActivateMarkdownPreview();
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine(error.Message);
            }

        }

        public byte[] GetDocument(MessageType format)
        {
            if (format == MessageType.Html)
            {
                return GetHtmlMessage(rtxt.Document);
            }
            else if (format == MessageType.Markdown)
            {
                return GetMarkdownTextMessage(rtxt.Document);
            }
            else
                return null;
        }

        public bool HasDocument()
        {
            return rtxt.HasDocument;
        }

        private void tgBtn_Markdown_Checked(object sender, RoutedEventArgs e)
        {
            ActivateMarkdownPreview();
        }

        public void ScrollToChatMessage(ChatMessageItemViewModel chatModel,int startIndex)
        {
            chatList.ScrollToItem(chatModel,startIndex);
        }
    }
}
