using QueryBuilder.ParameterFixer;

namespace UnitTests;

public class ParameterFixerTest
{
    [Theory]
    [InlineData("id", "\"id\"")]
    [InlineData("Student", "\"Student\"")]
    public void WrapIdentifier_Should_WrapInDoubleQuotes_When_UsingPostgresFixer(string identifier, string expected)
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier(identifier);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("id", "[id]")]
    [InlineData("Student", "[Student]")]
    public void WrapIdentifier_Should_WrapInBrackets_When_UsingSqlServerFixer(string identifier, string expected)
    {
        // Act
        var result = SqlServerParameterFixer.Instance.WrapIdentifier(identifier);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void WrapIdentifier_Should_WrapEmptyValue_When_IdentifierIsEmptyStringInPostgres()
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier("");

        // Assert
        result.Should().Be("\"\"");
    }

    [Fact]
    public void WrapIdentifier_Should_WrapEmptyValue_When_IdentifierIsEmptyStringInSqlServer()
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
    public void FormatParameter_Should_ProduceIndexedPlaceholder_When_UsingPostgresFixer(int index, string expected)
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
    public void FormatParameter_Should_ProduceIndexedPlaceholder_When_UsingSqlServerFixer(int index, string expected)
    {
        // Act
        var result = SqlServerParameterFixer.Instance.FormatParameter(index);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void FormatParameter_Should_ProduceInvalidPlaceholder_When_IndexIsNegative()
    {
        // Act
        var result = PostgresParameterFixer.Instance.FormatParameter(-1);

        // Assert
        result.Should().Be("@p-1");
    }

    [Fact]
    public void PostgresParameterFixerInstance_Should_ReturnSameSingleton_When_AccessingTwice()
    {
        // Act & Assert
        PostgresParameterFixer.Instance.Should().BeSameAs(PostgresParameterFixer.Instance);
    }

    [Fact]
    public void SqlServerParameterFixerInstance_Should_ReturnSameSingleton_When_AccessingTwice()
    {
        // Act & Assert
        SqlServerParameterFixer.Instance.Should().BeSameAs(SqlServerParameterFixer.Instance);
    }

    [Fact]
    public void WrapIdentifier_Should_NotEscapeEmbeddedQuote_When_WrappingIdentifierInPostgres()
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier("a\"b");

        // Assert
        result.Should().Be("\"a\"b\"");
    }

    [Fact]
    public void WrapIdentifier_Should_NotEscapeEmbeddedBracket_When_WrappingIdentifierInSqlServer()
    {
        // Act
        var result = SqlServerParameterFixer.Instance.WrapIdentifier("a]b");

        // Assert
        result.Should().Be("[a]b]");
    }

    [Fact]
    public void WrapIdentifier_Should_PassPayloadThrough_When_IdentifierContainsSqlInjectionPayload()
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier("x\"; DROP TABLE y--");

        // Assert
        result.Should().Be("\"x\"; DROP TABLE y--\"");
    }
}
