using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SubtitleDownloaderV2.Converters
{
    internal class InvertedNullOrWhitespaceToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var toString = value as string;
            return string.IsNullOrWhiteSpace(toString) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If this is met, turn off TwoWay Binding.
            return null;
        }
    }
}
