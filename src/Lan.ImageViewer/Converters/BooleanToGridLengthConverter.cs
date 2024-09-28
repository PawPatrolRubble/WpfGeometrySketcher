using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lan.ImageViewer.Converters
{
    public class BooleanToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                return booleanValue ? GridLength.Auto : new GridLength(450);
            }

            return new GridLength(450); // Default case
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}