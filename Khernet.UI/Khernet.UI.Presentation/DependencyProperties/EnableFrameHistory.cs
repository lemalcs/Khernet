using System.Windows;
using System.Windows.Controls;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Avoids save frame history and hides navigation bar
    /// </summary>
    public class DisableFrameHistoryProperty : BaseAttachedProperty<DisableFrameHistoryProperty, bool>
    {
        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Frame)
            {
                var frame = d as Frame;

                //Hide navigation var
                frame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;

                frame.Navigated += (sd, ev) => ((Frame)sd).NavigationService.RemoveBackEntry();
            }
        }
    }
}
