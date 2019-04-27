using System;
using System.Globalization;
using System.Windows.Data;

namespace iBank.Operator.Desktop.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class AccountNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string val && val.Length == 16 ? value?.ToString().Remove(0, 8).Insert(4, " ") : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
