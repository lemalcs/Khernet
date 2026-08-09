using System.Windows;
using System.Windows.Controls;

namespace Khernet.UI.DependencyProperties
{
    public class ScrollToBottomOnLoadProperty : BaseAttachedProperty<ScrollToBottomOnLoadProperty, bool>
    {
        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Check if control is a ScrollViewer
            if (!(d is ScrollViewer))
                return;

            //Get scrollviewer
            var scrollControl = (ScrollViewer)d;

            //Scroll to bottom when DataContext changes
            scrollControl.DataContextChanged -= ScrollControl_DataContextChanged;
            scrollControl.DataContextChanged += ScrollControl_DataContextChanged;
        }

        private void ScrollControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ScrollViewer).ScrollToBottom();
        }
    }
}
