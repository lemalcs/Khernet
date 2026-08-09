using System.Windows;
using System.Windows.Controls;

namespace Khernet.UI.DependencyProperties
{
    public class ScrollToBottomOnWriteProperty : BaseAttachedProperty<ScrollToBottomOnWriteProperty, bool>
    {
        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Check if control is a ScrollViewer
            if (!(d is ScrollViewer))
                return;

            //Get scrollviewer
            var scrollControl = (ScrollViewer)d;

            if (!(bool)e.NewValue)
            {
                scrollControl.ScrollToEnd();
            }
        }
    }
}
