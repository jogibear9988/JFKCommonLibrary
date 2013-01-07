using System;
using System.Windows;
using System.Windows.Data;

namespace JFKCommonLibrary.WPF.Converters
{
    public class IntToHiddenConverter : ValueConverter
    {
        public int HiddenValue { get; set; }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (System.Convert.ToInt32(value) == HiddenValue)
                return Visibility.Hidden;
            return Visibility.Visible;            
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
