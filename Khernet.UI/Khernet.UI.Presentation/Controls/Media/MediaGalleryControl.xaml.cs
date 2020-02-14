using System;
using System.Windows.Controls;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Lógica de interacción para MediaGalleryControl.xaml
    /// </summary>
    public partial class MediaGalleryControl : UserControl
    {
        public event EventHandler<SelectedGIFEventArgs> SelectedGIF;

        public MediaGalleryControl()
        {
            InitializeComponent();
        }

        protected void OnSelectedGIF(BaseModel model)
        {
            SelectedGIF?.Invoke(this, new SelectedGIFEventArgs(model));
        }

        private void ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                //Fire and event to notify that a video was selected
                OnSelectedGIF(e.AddedItems[0] as GIFItemViewModel);
                listGIF.SelectedIndex = -1;
            }

        }
    }
}
