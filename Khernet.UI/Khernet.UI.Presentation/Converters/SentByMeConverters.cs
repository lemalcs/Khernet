using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    public class SentByMeToColumn : BaseValueConverter<SentByMeToColumn>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Change grid column location for chat buuble and chat bubble anchor
            bool sentByMe = (bool)value;
            if (sentByMe)
            {
                return 2;
            }
            return 0;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SentByMeToBubbleAnchorMarginConverter : BaseValueConverter<SentByMeToBubbleAnchorMarginConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            //Change margin for chat bubble anchor
            if (!(bool)value)
            {
                return new Thickness(0, 0, 0, 0);
            }
            return new Thickness(10, 0, 0, 0);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SentByMeToBrushConverter : BaseValueConverter<SentByMeToBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Change background color for chat bubble anchor
            if ((bool)value)
            {
                return Application.Current.FindResource("LightBlueBrush");
            }
            return Application.Current.FindResource("VeryLightGrayBrush");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SentByMeToAligment : BaseValueConverter<SentByMeToAligment>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SentByMeToCornerRadiusConverter : BaseValueConverter<SentByMeToCornerRadiusConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new CornerRadius(7, 7, 0, 7) : new CornerRadius(7, 7, 7, 0);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Changes the upper and bottom anchor of chat message container
    /// </summary>
    public class SentByMeToPathDataConverter : BaseValueConverter<SentByMeToPathDataConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool sentByMe = (bool)value;

            if (sentByMe && parameter.ToString() == "upperAnchor"
                || !sentByMe && parameter.ToString() == "bottomAnchor")
            {
                return "M0,0 L9,9";
            }
            return "M9,0 L0,9";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Changes the brush of chat message container
    /// </summary>
    public class SentByMeToAnchorBrushConverter : BaseValueConverter<SentByMeToAnchorBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Change background color for chat bubble anchor
            if ((bool)value)
            {
                return Application.Current.FindResource("LightBlueAnchorBrush");
            }
            return Application.Current.FindResource("LightGrayAnchorBrush");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
