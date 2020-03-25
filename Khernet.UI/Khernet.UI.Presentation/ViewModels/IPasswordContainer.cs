using Khernet.UI.Media;
using System.Security;

namespace Khernet.UI
{
    /// <summary>
    /// Holds passwords
    /// </summary>
    public interface IPasswordContainer
    {
        SecureString password { get; }
        SecureString secondPassword { get; }

        /// <summary>
        /// Clear any passwords typed
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Operations for documents
    /// </summary>
    public interface IDocumentContainer
    {
        bool HasDocument();

        byte[] GetDocument(MessageType format);

        void ClearContent();

        void ScrollToCurrentContent();
    }
}
