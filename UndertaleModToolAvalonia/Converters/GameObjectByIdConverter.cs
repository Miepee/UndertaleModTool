﻿using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using UndertaleModLib.Models;

namespace UndertaleModToolAvalonia
{
    [ValueConversion(typeof(uint), typeof(UndertaleGameObject))]
    public class GameObjectByIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint val = System.Convert.ToUInt32(value);
            UndertaleGameObject returnObj = null;
            if (val < (Application.Current.MainWindow as MainWindow).Data.GameObjects.Count)
            {
                returnObj = (Application.Current.MainWindow as MainWindow).Data.GameObjects[(int)val];
                return returnObj;
            }
            else
            {
                return returnObj;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (uint)(Application.Current.MainWindow as MainWindow).Data.GameObjects.IndexOf((UndertaleGameObject)value);
        }
    }
}
