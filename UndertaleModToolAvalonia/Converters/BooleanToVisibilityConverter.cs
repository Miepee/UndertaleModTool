using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace UndertaleModToolAvalonia
{
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility trueValue { get; set; }
        public Visibility falseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool boolean && boolean) ? trueValue : falseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
