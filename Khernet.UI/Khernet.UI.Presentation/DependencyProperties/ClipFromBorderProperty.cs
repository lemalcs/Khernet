using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Creates a clip region from parent <see cref="Border"/> depending of <see cref="CornerRadius"/> value.
    /// </summary>
    public class ClipFromBorderProperty : BaseAttachedProperty<ClipFromBorderProperty, bool>
    {
        /// <summary>
        /// Called when element loads.
        /// </summary>
        private RoutedEventHandler eventHandler;

        /// <summary>
        /// Called when element changes its size.
        /// </summary>
        private SizeChangedEventHandler sizeHandler;

        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FrameworkElement;

            //Check if parent control is a Border
            if (control.Parent is Border)
            {
                //Get Border control
                var border = control.Parent as Border;

                //Set event handlers

                eventHandler = (sd, ev) => Border_Loaded(sd, ev, control);
                sizeHandler = (sd, ev) => Border_Loaded(sd, ev, control);

                //Hook events
                if ((bool)e.NewValue)
                {
                    border.Loaded += eventHandler;
                    border.SizeChanged += sizeHandler;
                }
                else //Unhook events if new value is false
                {
                    border.Loaded -= eventHandler;
                    border.SizeChanged -= sizeHandler;
                }
            }
        }

        private void Border_Loaded(object sender, RoutedEventArgs e, FrameworkElement child)
        {
            var border = sender as Border;

            //If border don't have width and height, return
            if (border.ActualWidth == 0 && border.ActualHeight == 0)
                return;

            var rect = new RectangleGeometry();

            //Match the geometry corner radius to the border's radius
            rect.RadiusX = rect.RadiusY = Math.Max(0, border.CornerRadius.TopLeft - border.BorderThickness.Left * 0.5);

            //Set rectangle size to match the actual child size
            rect.Rect = new Rect(child.RenderSize);

            //Set clipping area for child
            child.Clip = rect;
        }
    }
}
