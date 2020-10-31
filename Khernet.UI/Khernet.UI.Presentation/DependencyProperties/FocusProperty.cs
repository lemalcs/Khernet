using System.Windows;
using System.Windows.Controls;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Focus an control.
    /// </summary>
    public class FocusProperty : BaseAttachedProperty<FocusProperty, bool>
    {
        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Check is d parameter is a control
            if (!(d is Control))
                return;

            var control = (Control)d;

            control.Focus();

        }
    }
}
