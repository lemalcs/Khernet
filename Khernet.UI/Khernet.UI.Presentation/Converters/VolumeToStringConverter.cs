﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Vlc.DotNet.Core.Interops.Signatures;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Change a volume value to a icon name.
    /// </summary>
    public class VolumeToStringConverter : BaseValueConverter<VolumeToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int volume = (int)value;
            return volume == 0 ? "VolumeOff" : "VolumeHigh";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
