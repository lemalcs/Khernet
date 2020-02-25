using Khernet.UI.Converters;
using Khernet.UI.Media;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Edition of a field and returning of a byte array.
    /// </summary>
    public partial class FieldEditorControl : BasePopUpControl, IDocumentContainer
    {
        public FieldEditorControl()
        {
            InitializeComponent();
        }
        public FieldEditorControl(BaseModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();

        }

        public void ClearContent()
        {

        }

        public byte[] GetDocument(MessageType format)
        {
            if (format == MessageType.Html)
            {
                return GetTextMessage(rtxt.Document);
            }
            else
                return null;
        }

        public bool HasDocument()
        {
            return rtxt.HasDocument;
        }

        public void ScrollToCurrentContent()
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Commit change when Accept button or Cancel buttons is pressed
            OnCommited();
        }

        private byte[] GetTextMessage(FlowDocument document)
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
    }
}
