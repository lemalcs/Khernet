using Khernet.UI.Cache;
using System;
using System.Windows;
using System.Windows.Controls;
using Vlc.DotNet.Wpf;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Creates a clip region from parent <see cref="Border"/> depending of <see cref="CornerRadius"/> value
    /// </summary>
    public class MediaSourceProperty : BaseAttachedProperty<MediaSourceProperty, string>
    {
        /// <summary>
        /// The audio volume in percents, the default range is from 0% to 125%
        /// </summary>
        private int defaultVolume = 60;

        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var control = d as VlcControl;

            //Check if control is a VlcControl
            if (control != null && control.SourceProvider.MediaPlayer == null)
            {
                string mediaPath = e.NewValue as string;

                //Sets the directory path for vlc library
                control.SourceProvider.CreatePlayer(Configurations.VlcDirectory);

                //Redirect log to console output rather than the default logger in vlc ibrary.
                control.SourceProvider.MediaPlayer.Log += (s, ev) =>
                 {
                     string message = $"libVlc : {ev.Level} {ev.Message} @ {ev.Module}";
                     System.Diagnostics.Debug.WriteLine(message);
                 };

                //Set default volume to 50 db (decibels)
                control.Volume = defaultVolume;

                //Set media source
                control.SourceProvider.MediaPlayer.SetMedia(new Uri(mediaPath));

                //Plays the video from file video
                control.SourceProvider.MediaPlayer.Play();
            }
        }

        protected override void OnValueUpdated(DependencyObject d, object baseValue)
        {
            var control = d as VlcControl;

            //Check if control is a VlcControl
            if (control != null && baseValue != null)
            {
                string mediaPath = baseValue as string;

                //Sets the directory path for vlc library
                control.SourceProvider.CreatePlayer(Configurations.VlcDirectory);

                //Redirect log to console output rather than the default logger in vlc ibrary.
                control.SourceProvider.MediaPlayer.Log += (s, ev) =>
                {
                    string message = $"libVlc : {ev.Level} {ev.Message} @ {ev.Module}";
                    System.Diagnostics.Debug.WriteLine(message);
                };

                //Set media source
                control.SourceProvider.MediaPlayer.SetMedia(new Uri(mediaPath));

                //Set default volume to 50 db (decibels)
                control.Volume = defaultVolume;

                //Plays the video from file video
                control.SourceProvider.MediaPlayer.Play();
            }
        }
    }
}
