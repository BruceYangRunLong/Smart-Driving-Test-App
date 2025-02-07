namespace dpa.Converters;
using System;
using System.Globalization;
using Avalonia.Data.Converters;

public class StringNullOrEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 判断字符串是否为空或为null
        if (value is string str)
        {
            return string.IsNullOrEmpty(str);
        }
        return false; // 默认返回 false
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
