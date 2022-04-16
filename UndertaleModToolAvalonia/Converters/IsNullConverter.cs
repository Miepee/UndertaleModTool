﻿using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace UndertaleModToolAvalonia
{
    public class IsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = parameter is string par && par == "True";
            return (value is null) ^ invert;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
