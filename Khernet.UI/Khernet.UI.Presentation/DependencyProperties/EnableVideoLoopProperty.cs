using System;
using System.Windows;
using System.Windows.Controls;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Enables video looping for <see cref="MediaElement"/>
    /// </summary>
    public class EnableVideoLoopProperty : BaseAttachedProperty<EnableVideoLoopProperty, bool>
    {
        /// <summary>
        /// Called when media ends playing
        /// </summary>
        private RoutedEventHandler mediaEndedHandler;

        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MediaElement;

            //Check if control is a VlcControl
            if (control != null)
            {
                control.LoadedBehavior = MediaState.Play;
                control.UnloadedBehavior = MediaState.Pause;

                //Set the event handler for MediaEnded event of MediaElement
                mediaEndedHandler = (sender, ev) => Control_MediaEnded(sender, ev);

                if ((bool)e.NewValue)
                {
                    //Attach event to enable video looping
                    control.MediaEnded += mediaEndedHandler;
                }
                else
                {
                    //Attach event to disable video looping
                    control.MediaEnded -= mediaEndedHandler;
                }
            }
        }

        private void Control_MediaEnded(object sender, RoutedEventArgs e)
        {
            var control = sender as MediaElement;
            if (control != null)
            {
                //Set video position to start
                control.Position = TimeSpan.Zero;
            }
        }
    }
}
