using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Creates a <see cref="FlowDocument"/> from a byte collection
    /// </summary>
    public class DocumentSourceProperty : BaseAttachedProperty<DocumentSourceProperty, ReadOnlyCollection<byte>>
    {

        protected override void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var control = d as RichTextBox;

            //Check if control is a VlcControl
            if (control != null)
            {
                FlowDocument fw = new FlowDocument();

                //Returns a stream based on byte array (FlowDocument from RichTextbox)
                if (e.NewValue == null || ((ReadOnlyCollection<byte>)e.NewValue).Count == 0)
                    return;

                try
                {
                    TextRange range = new TextRange(fw.ContentStart, fw.ContentEnd);

                    using (MemoryStream textContent = new MemoryStream(((ReadOnlyCollection<byte>)e.NewValue).ToArray()))
                    {
                        range.Load(textContent, DataFormats.XamlPackage);
                    }
                    range.ApplyPropertyValue(TextElement.FontFamilyProperty, App.Current.Resources["RobotoRegularFont"]);
                    range.ApplyPropertyValue(TextElement.FontSizeProperty, App.Current.Resources["RegularFontSize"]);
                    range.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#roboto"));

                    control.Document = fw;
                    control.Document.PagePadding = new Thickness(0, 0, 0, 0);
                }
                catch (Exception error)
                {
                    System.Diagnostics.Debug.WriteLine(error.Message);
                }
            }
        }
    }
}
