using Khernet.UI.Converters;
using Khernet.UI.Media;
using System.Text;
using System.Windows.Documents;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// Lógica de interacción para ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : BasePage<ProfileViewModel>, IDocumentContainer
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        public ProfilePage(ProfileViewModel model) : base(model)
        {
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
