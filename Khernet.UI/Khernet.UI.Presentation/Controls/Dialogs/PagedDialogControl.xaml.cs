using Khernet.UI.IoC;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Navigator for pages.
    /// </summary>
    public partial class PagedDialogControl : BaseDialogUserControl, IModalDialog
    {
        public PagedDialogControl()
        {
            InitializeComponent();
        }

        public void Close()
        {
            base.CloseCommand.Execute(null);
        }
    }
}
