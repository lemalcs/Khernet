using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Viewer for video files such as MP4, AVI.
    /// </summary>
    public partial class VideoPlayerControl : BaseDialogUserControl
    {
        /// <summary>
        /// Timer to control when video controls hide.
        /// </summary>
        DispatcherTimer timer;

        /// <summary>
        /// The model for video player
        /// </summary>
        VideoPlayerViewModel videoPlayerModel;

        public VideoPlayerViewModel VideoPlayerModel
        {
            get => videoPlayerModel;
            private set => videoPlayerModel = value;
        }

        public VideoPlayerControl()
        {
            InitializeComponent();
            timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
            timer.Tick += Timer_Tick;
            timer.IsEnabled = true;
            timer.Interval = TimeSpan.FromSeconds(5);
            VideoPlayerModel = new VideoPlayerViewModel();
            VideoPlayerModel.Player = Media;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //Animation that hide video controls
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1;
            myDoubleAnimation.To = 0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.1));

            videoControls.BeginAnimation(StackPanel.OpacityProperty, myDoubleAnimation);

            //Stop timer
            timer.IsEnabled = false;
            timer.Stop();
        }

        private async void BaseDialogUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Tick -= Timer_Tick;
            await Media.Close();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            //Start timer so video controls hide after 5 seconds
            timer.Stop();
            timer.Start();
        }

        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            VideoPlayerModel.SetStoppedVideo();
        }
    }
}
