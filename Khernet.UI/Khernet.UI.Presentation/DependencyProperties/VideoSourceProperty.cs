using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Enables video looping for <see cref="MediaElement"/>.
    /// </summary>
    public class VideoSourceProperty : BaseAttachedProperty<VideoSourceProperty, string>
    {
        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Panel;

            //Check if control is a VlcControl
            if (control != null)
            {
                MediaPlayer player = new MediaPlayer();

                player.Open(new Uri(e.NewValue as string, UriKind.RelativeOrAbsolute));
                player.MediaEnded += Player_MediaEnded;

                VideoDrawing aVideoDrawing = new VideoDrawing();

                aVideoDrawing.Rect = new Rect(0, 0, control.Width, control.Height);

                aVideoDrawing.Player = player;

                // Play the video once.
                player.Play();

                control.Background = new DrawingBrush(aVideoDrawing);
            }
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            var control = sender as MediaPlayer;
            control.Position = TimeSpan.Zero;
            control.Play();
        }
    }
}
