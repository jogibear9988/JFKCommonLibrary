using System;
using System.Windows;
using System.Windows.Data;

namespace JFKCommonLibrary.WPF.Converters
{
    public class IntToVisibilityConverter : ValueConverter
    {
        public int VisibleValue { get; set; }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (System.Convert.ToInt32(value) != VisibleValue)
                return Visibility.Hidden;
            return Visibility.Visible;            
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
