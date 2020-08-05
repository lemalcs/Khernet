using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Returns a <see cref="PointCollection"/> containing a hexagon.
    /// </summary>
    public class ButtonShapeMultiConverter : BaseMultiValueConverter<ButtonShapeMultiConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return null;

            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue)
                return null;


            double width = (double)values[0];
            double height = (double)values[1] / 2;

            double xFirstCoordinate = height * Math.Tan(Math.PI / 6);

            Point Point4 = new Point(xFirstCoordinate, 0);
            Point Point5 = new Point(0, height);
            Point Point6 = new Point(xFirstCoordinate, height * 2);
            Point Point7 = new Point(width, height * 2);
            Point Point8 = new Point(width + xFirstCoordinate, height);
            Point Point9 = new Point(width, 0);

            PointCollection myPointCollection2 = new PointCollection();

            myPointCollection2.Add(Point4);
            myPointCollection2.Add(Point5);
            myPointCollection2.Add(Point6);
            myPointCollection2.Add(Point7);
            myPointCollection2.Add(Point8);
            myPointCollection2.Add(Point9);

            return myPointCollection2;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
