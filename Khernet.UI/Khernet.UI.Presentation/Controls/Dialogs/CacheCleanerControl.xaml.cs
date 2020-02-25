using System.Windows;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Dialog used when a cleaninig operation of cache is performed.
    /// </summary>
    public partial class CacheCleanerControl : BasePopUpControl
    {
        public CacheCleanerControl()
        {
            InitializeComponent();
        }

        public CacheCleanerControl(BaseModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Commit change when Accept button or Cancel buttons is pressed
            OnCommited();
        }
    }
}
