using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts circle radius to <see cref="Size"/> for right and left arcs
    /// </summary>
    public class RadiusToSizeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Size((double)value, (double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    /// <summary>
    /// Converts the circle radius to width for grid container
    /// </summary>
    public class RadiusToWidthMultiConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //Return zero for design mode
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return 0;

            //Set value for grid container height and width
            double radius = (double)values[0];
            double thinkness = (double)values[1];
            return radius * 2 + thinkness;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    /// <summary>
    /// Converts the circle radius to width for grid columns
    /// </summary>
    public class RadiusToHalfWidthMultiConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //Return zero for design mode
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return 0;

            //Set value for height and width of grid columns.
            double radius = (double)values[0];
            double thinkness = (double)values[1];
            return new GridLength(radius + thinkness / 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
