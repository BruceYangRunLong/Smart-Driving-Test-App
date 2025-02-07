using dpa.Library.Models;

namespace dpa.Converters;

using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

public class WrongAnswerHighlightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 检查输入值是否有效
        if (value == null || parameter == null)
            return Brushes.White;

        // 当前选项
        string currentOption = parameter.ToString();
        if (currentOption == null)
            return Brushes.White;

        // 从 CurrentQuestion 的数据中提取用户答案和正确答案
        if (value is Exercise exercise)
        {
            string userAnswer = exercise.user_answer;
            string correctAnswer = exercise.answer;

            // 如果当前选项是正确答案，返回绿色
            if (correctAnswer == currentOption)
            {
                return Brushes.LightGreen;
            }

            // 如果当前选项是用户错误选择，返回红色
            if (userAnswer == currentOption && userAnswer != correctAnswer)
            {
                return Brushes.LightCoral;
            }
        }

        // 默认返回白色
        return Brushes.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not supported.");
    }
}