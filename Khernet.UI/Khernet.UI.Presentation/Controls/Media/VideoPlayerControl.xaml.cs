using Khernet.UI.DependencyProperties;
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

        public VideoPlayerControl()
        {
            InitializeComponent();
            timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
            timer.Tick += Timer_Tick;
            timer.IsEnabled = true;
            timer.Interval = TimeSpan.FromSeconds(5);
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

        private void BaseDialogUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Tick -= Timer_Tick;
            vlcControl.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Pauses the video if it is playing
            if (vlcControl.SourceProvider.MediaPlayer.IsPlaying())
                vlcControl.SourceProvider.MediaPlayer.Pause();
            else if (vlcControl.SourceProvider.MediaPlayer.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Paused)
            {
                //Continue playing video if it is paused
                vlcControl.SourceProvider.MediaPlayer.Play();
            }
            else
            {
                //Otherwise play video again
                var mediaFile = MediaSourceProperty.GetValue(vlcControl);
                MediaSourceProperty.SetValue(vlcControl, mediaFile);
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            //Start timer so video controls hide after 5 seconds
            timer.Stop();
            timer.Start();
        }
    }
}
