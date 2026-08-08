using QueryBuilder.ParameterFixer;

namespace UnitTests;

public class ParameterFixerTest
{
    [Theory]
    [InlineData("id", "\"id\"")]
    [InlineData("Student", "\"Student\"")]
    public void PostgresParameterFixer_WrapIdentifier_WrapsInDoubleQuotes(string identifier, string expected)
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier(identifier);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("id", "[id]")]
    [InlineData("Student", "[Student]")]
    public void SqlServerParameterFixer_WrapIdentifier_WrapsInBrackets(string identifier, string expected)
    {
        // Act
        var result = SqlServerParameterFixer.Instance.WrapIdentifier(identifier);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void PostgresParameterFixer_WrapIdentifierWithEmptyString_WrapsEmptyValue()
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier("");

        // Assert
        result.Should().Be("\"\"");
    }

    [Fact]
    public void SqlServerParameterFixer_WrapIdentifierWithEmptyString_WrapsEmptyValue()
    {
        // Act
        var result = SqlServerParameterFixer.Instance.WrapIdentifier("");

        // Assert
        result.Should().Be("[]");
    }

    [Theory]
    [InlineData(0, "@p0")]
    [InlineData(1, "@p1")]
    [InlineData(42, "@p42")]
    public void PostgresParameterFixer_FormatParameter_ProducesIndexedPlaceholder(int index, string expected)
    {
        // Act
        var result = PostgresParameterFixer.Instance.FormatParameter(index);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, "@p0")]
    [InlineData(1, "@p1")]
    [InlineData(42, "@p42")]
    public void SqlServerParameterFixer_FormatParameter_ProducesIndexedPlaceholder(int index, string expected)
    {
        // Act
        var result = SqlServerParameterFixer.Instance.FormatParameter(index);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void PostgresParameterFixer_FormatParameterWithNegativeIndex_ProducesInvalidPlaceholder()
    {
        // Act
        var result = PostgresParameterFixer.Instance.FormatParameter(-1);

        // Assert
        result.Should().Be("@p-1");
    }

    [Fact]
    public void PostgresParameterFixer_Instance_ReturnsSameSingleton()
    {
        // Act & Assert
        PostgresParameterFixer.Instance.Should().BeSameAs(PostgresParameterFixer.Instance);
    }

    [Fact]
    public void SqlServerParameterFixer_Instance_ReturnsSameSingleton()
    {
        // Act & Assert
        SqlServerParameterFixer.Instance.Should().BeSameAs(SqlServerParameterFixer.Instance);
    }

    [Fact]
    public void PostgresParameterFixer_WrapIdentifierWithEmbeddedQuote_DoesNotEscape()
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier("a\"b");

        // Assert
        result.Should().Be("\"a\"b\"");
    }

    [Fact]
    public void SqlServerParameterFixer_WrapIdentifierWithEmbeddedBracket_DoesNotEscape()
    {
        // Act
        var result = SqlServerParameterFixer.Instance.WrapIdentifier("a]b");

        // Assert
        result.Should().Be("[a]b]");
    }

    [Fact]
    public void PostgresParameterFixer_WrapIdentifierWithSqlPayload_PassesPayloadThrough()
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier("x\"; DROP TABLE y--");

        // Assert
        result.Should().Be("\"x\"; DROP TABLE y--\"");
    }
}
