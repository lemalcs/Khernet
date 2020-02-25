using System.Windows;
using System.Windows.Controls;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Viewer for animation files such as GIF, at full screen mode.
    /// </summary>
    public partial class AnimationMessageControl : UserControl
    {
        public AnimationMessageControl()
        {
            InitializeComponent();
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            //Get de MediaElement Control
            var mediaElement = sender as MediaElement;

            //Check if media element has video
            if (mediaElement != null && mediaElement.HasVideo)
            {
                //Set max width for media element so it can be as bigger than the original width of file
                mediaElement.MaxWidth = mediaElement.NaturalVideoWidth;
            }
        }
    }
}
