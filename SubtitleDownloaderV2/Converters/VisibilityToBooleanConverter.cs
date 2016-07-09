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
    internal class VisibilityToBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (object)(!(value is Visibility) 
                ? 0 
                : ((Visibility)value == Visibility.Visible 
                    ? 1 
                    : 0
                ));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (object)(Visibility)((bool)value ? 0 : 2);
        }
    }
}
