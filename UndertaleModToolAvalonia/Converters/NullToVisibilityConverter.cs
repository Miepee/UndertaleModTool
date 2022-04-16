﻿using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace UndertaleModToolAvalonia
{
    public sealed class NullToVisibilityConverter : IValueConverter
    {
        public bool nullValue { get; set; }
        public bool notNullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? nullValue : notNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
