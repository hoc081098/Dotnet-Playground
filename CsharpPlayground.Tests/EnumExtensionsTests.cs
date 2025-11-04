using CsharpPlayground.OOP;

namespace CsharpPlayground.Tests;

public class EnumExtensionsTests
{
    // Test enum for testing purposes
    private enum TestEnum
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 3
    }

    #region FindOrThrow Tests

    [Fact]
    public void FindOrThrow_ShouldReturnMatchingValue_WhenPredicateMatches()
    {
        // Act
        var result = EnumExtensions.FindOrThrow<DayOfWeek>(d => d == DayOfWeek.Monday);

        // Assert
        Assert.Equal(DayOfWeek.Monday, result);
    }

    [Fact]
    public void FindOrThrow_ShouldReturnFirstMatch_WhenMultipleValuesMatch()
    {
        // Act
        var result = EnumExtensions.FindOrThrow<DayOfWeek>(d => d is DayOfWeek.Monday or DayOfWeek.Tuesday);

        // Assert
        Assert.Equal(DayOfWeek.Monday, result);
    }

    [Fact]
    public void FindOrThrow_ShouldThrowInvalidOperationException_WhenNoMatch()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            EnumExtensions.FindOrThrow<DayOfWeek>(d => d.ToString() == "InvalidDay"));
    }

    [Fact]
    public void FindOrThrow_ShouldWorkWithCustomEnum()
    {
        // Act
        var result = EnumExtensions.FindOrThrow<TestEnum>(e => e == TestEnum.Second);

        // Assert
        Assert.Equal(TestEnum.Second, result);
    }

    #endregion

    #region FindOrDefault Tests

    [Fact]
    public void FindOrDefault_ShouldReturnMatchingValue_WhenPredicateMatches()
    {
        // Act
        var result = EnumExtensions.FindOrDefault<DayOfWeek>(d => d == DayOfWeek.Sunday);

        // Assert
        Assert.Equal(DayOfWeek.Sunday, result);
    }

    [Fact]
    public void FindOrDefault_ShouldReturnDefault_WhenNoMatch()
    {
        // Act
        var result = EnumExtensions.FindOrDefault<DayOfWeek>(d => d.ToString() == "InvalidDay");

        // Assert
        Assert.Equal(default, result);
        Assert.Equal(DayOfWeek.Sunday, result); // Sunday is 0, which is the default
    }

    [Fact]
    public void FindOrDefault_ShouldReturnFirstMatch_WhenMultipleValuesMatch()
    {
        // Act
        var result = EnumExtensions.FindOrDefault<DayOfWeek>(d => d is DayOfWeek.Friday or DayOfWeek.Saturday);

        // Assert
        Assert.Equal(DayOfWeek.Friday, result);
    }

    [Fact]
    public void FindOrDefault_ShouldWorkWithCustomEnum()
    {
        // Act
        var result = EnumExtensions.FindOrDefault<TestEnum>(e => e == TestEnum.Third);

        // Assert
        Assert.Equal(TestEnum.Third, result);
    }

    [Fact]
    public void FindOrDefault_ShouldReturnDefaultCustomEnum_WhenNoMatch()
    {
        // Act
        var result = EnumExtensions.FindOrDefault<TestEnum>(e => (int)e == 999);

        // Assert
        Assert.Equal(default, result);
        Assert.Equal(TestEnum.None, result);
    }

    #endregion

    #region FindByNameOrThrow Tests

    [Fact]
    public void FindByNameOrThrow_ShouldReturnEnum_WhenNameMatchesExactly()
    {
        // Act
        var result = EnumExtensions.FindByNameOrThrow<DayOfWeek>("Monday");

        // Assert
        Assert.Equal(DayOfWeek.Monday, result);
    }

    [Fact]
    public void FindByNameOrThrow_ShouldThrowArgumentException_WhenNameDoesNotMatch()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            EnumExtensions.FindByNameOrThrow<DayOfWeek>("InvalidDay"));
    }

    [Fact]
    public void FindByNameOrThrow_ShouldBeCaseSensitive_WhenIgnoreCaseIsFalse()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            EnumExtensions.FindByNameOrThrow<DayOfWeek>("monday", ignoreCase: false));
    }

    [Fact]
    public void FindByNameOrThrow_ShouldBeCaseInsensitive_WhenIgnoreCaseIsTrue()
    {
        // Act
        var result = EnumExtensions.FindByNameOrThrow<DayOfWeek>("monday", ignoreCase: true);

        // Assert
        Assert.Equal(DayOfWeek.Monday, result);
    }

    [Fact]
    public void FindByNameOrThrow_ShouldWorkWithMixedCase_WhenIgnoreCaseIsTrue()
    {
        // Act
        var result = EnumExtensions.FindByNameOrThrow<DayOfWeek>("wEdNeSdAy", ignoreCase: true);

        // Assert
        Assert.Equal(DayOfWeek.Wednesday, result);
    }

    [Fact]
    public void FindByNameOrThrow_ShouldWorkWithCustomEnum()
    {
        // Act
        var result = EnumExtensions.FindByNameOrThrow<TestEnum>("First");

        // Assert
        Assert.Equal(TestEnum.First, result);
    }

    #endregion

    #region FindByValueOrThrow Tests

    [Fact]
    public void FindByValueOrThrow_ShouldReturnEnum_WhenValueMatches()
    {
        // Act
        var result = EnumExtensions.FindByValueOrThrow<DayOfWeek>(1); // Monday = 1

        // Assert
        Assert.Equal(DayOfWeek.Monday, result);
    }

    [Fact]
    public void FindByValueOrThrow_ShouldThrowArgumentException_WhenValueDoesNotExist()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            EnumExtensions.FindByValueOrThrow<DayOfWeek>(999));

        Assert.Contains("No enum value of type DayOfWeek with value 999 found", exception.Message);
    }

    [Fact]
    public void FindByValueOrThrow_ShouldWorkWithZeroValue()
    {
        // Act
        var result = EnumExtensions.FindByValueOrThrow<DayOfWeek>(0); // Sunday = 0

        // Assert
        Assert.Equal(DayOfWeek.Sunday, result);
    }

    [Fact]
    public void FindByValueOrThrow_ShouldWorkWithCustomEnum()
    {
        // Act
        var result = EnumExtensions.FindByValueOrThrow<TestEnum>(2);

        // Assert
        Assert.Equal(TestEnum.Second, result);
    }

    [Fact]
    public void FindByValueOrThrow_ShouldThrowForNegativeValue_WhenNotDefined()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            EnumExtensions.FindByValueOrThrow<DayOfWeek>(-1));

        Assert.Contains("No enum value of type DayOfWeek with value -1 found", exception.Message);
    }

    [Fact]
    public void FindByValueOrThrow_ShouldWorkWithConsoleColor()
    {
        // Act
        var result = EnumExtensions.FindByValueOrThrow<ConsoleColor>(12); // Red = 12

        // Assert
        Assert.Equal(ConsoleColor.Red, result);
    }

    #endregion
}

