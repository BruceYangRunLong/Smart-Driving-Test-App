
using Avalonia.Media;
using dpa.Converters;
using dpa.Library.Models;

namespace dpa.UnitTest.Converters;

public class WrongAnswerHighlightConverterTest
{
    [Fact]
    public void Convert_ReturnsLightGreen_WhenAnswerIsCorrect()
    {
        // Arrange
        var exercise = new Exercise { user_answer = "1", answer = "1" };
        var converter = new WrongAnswerHighlightConverter();

        // Act
        var result = converter.Convert(exercise, typeof(Brush), "1", null);

        // Assert
        Assert.Equal(Brushes.LightGreen, result);
    }

    [Fact]
    public void Convert_ReturnsLightCoral_WhenAnswerIsWrong()
    {
        // Arrange
        var exercise = new Exercise { user_answer = "2", answer = "3" };
        var converter = new WrongAnswerHighlightConverter();

        // Act
        var result = converter.Convert(exercise, typeof(Brush), "2", null);

        // Assert
        Assert.Equal(Brushes.LightCoral, result);
    }

    [Fact]
    public void Convert_ReturnsWhite_WhenOptionIsNeitherCorrectNorSelected()
    {
        // Arrange
        var exercise = new Exercise { user_answer = "2", answer = "3" };
        var converter = new WrongAnswerHighlightConverter();

        // Act
        var result = converter.Convert(exercise, typeof(Brush), "1", null);

        // Assert
        Assert.Equal(Brushes.White, result);
    }
}