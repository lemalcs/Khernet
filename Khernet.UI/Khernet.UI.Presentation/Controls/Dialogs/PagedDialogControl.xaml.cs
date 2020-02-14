using Khernet.UI.IoC;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Lógica de interacción para SettingsControl.xaml
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
