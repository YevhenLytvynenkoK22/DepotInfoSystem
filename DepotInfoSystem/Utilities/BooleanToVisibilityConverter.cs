using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DepotInfoSystem.Utilities
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (parameter != null && parameter.ToString() == "Inverse")
                {
                    return b ? Visibility.Collapsed : Visibility.Visible;
                }
                return b ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}