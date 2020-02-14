using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Allows a <see cref="RichTextBox.Document"/> to be binded to another <see cref="FlowDocument"/>
    /// </summary>
    public class FormatedDocumentSourceProperty : BaseAttachedProperty<FormatedDocumentSourceProperty, FlowDocument>
    {

        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var control = d as RichTextBox;

            //Check if control is a RichTextBox
            if (control != null)
            {
                //Returns a stream based on byte array (FlowDocument from RichTextbox)
                if (e.NewValue == null || ((FlowDocument)e.NewValue) == null)
                    return;

                try
                {
                    control.Document = (FlowDocument)e.NewValue;
                }
                catch (Exception error)
                {
                    System.Diagnostics.Debug.WriteLine(error.Message);
                }
            }
        }
    }
}
