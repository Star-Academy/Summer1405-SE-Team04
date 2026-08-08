using QueryBuilder.ParameterFixer;

namespace UnitTests;

public class ParameterFixerTest
{
    [Theory]
    [InlineData("id", "\"id\"")]
    [InlineData("Student", "\"Student\"")]
    public void PostgresParameterFixer_WrapIdentifier_WrapsInDoubleQuotes(string identifier, string expected)
    {
        var result = PostgresParameterFixer.Instance.WrapIdentifier(identifier);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("id", "[id]")]
    [InlineData("Student", "[Student]")]
    public void SqlServerParameterFixer_WrapIdentifier_WrapsInBrackets(string identifier, string expected)
    {
        var result = SqlServerParameterFixer.Instance.WrapIdentifier(identifier);

        result.Should().Be(expected);
    }

    [Fact]
    public void PostgresParameterFixer_WrapIdentifierWithEmptyString_WrapsEmptyValue()
    {
        var result = PostgresParameterFixer.Instance.WrapIdentifier("");

        result.Should().Be("\"\"");
    }

    [Fact]
    public void SqlServerParameterFixer_WrapIdentifierWithEmptyString_WrapsEmptyValue()
    {
        var result = SqlServerParameterFixer.Instance.WrapIdentifier("");

        result.Should().Be("[]");
    }

    [Theory]
    [InlineData(0, "@p0")]
    [InlineData(1, "@p1")]
    [InlineData(42, "@p42")]
    public void PostgresParameterFixer_FormatParameter_ProducesIndexedPlaceholder(int index, string expected)
    {
        var result = PostgresParameterFixer.Instance.FormatParameter(index);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, "@p0")]
    [InlineData(1, "@p1")]
    [InlineData(42, "@p42")]
    public void SqlServerParameterFixer_FormatParameter_ProducesIndexedPlaceholder(int index, string expected)
    {
        var result = SqlServerParameterFixer.Instance.FormatParameter(index);

        result.Should().Be(expected);
    }

    [Fact]
    public void PostgresParameterFixer_FormatParameterWithNegativeIndex_ProducesInvalidPlaceholder()
    {
        var result = PostgresParameterFixer.Instance.FormatParameter(-1);

        result.Should().Be("@p-1");
    }

    [Fact]
    public void PostgresParameterFixer_Instance_ReturnsSameSingleton()
    {
        PostgresParameterFixer.Instance.Should().BeSameAs(PostgresParameterFixer.Instance);
    }

    [Fact]
    public void SqlServerParameterFixer_Instance_ReturnsSameSingleton()
    {
        SqlServerParameterFixer.Instance.Should().BeSameAs(SqlServerParameterFixer.Instance);
    }

    [Fact]
    public void PostgresParameterFixer_WrapIdentifierWithEmbeddedQuote_DoesNotEscape()
    {
        var result = PostgresParameterFixer.Instance.WrapIdentifier("a\"b");

        result.Should().Be("\"a\"b\"");
    }

    [Fact]
    public void SqlServerParameterFixer_WrapIdentifierWithEmbeddedBracket_DoesNotEscape()
    {
        var result = SqlServerParameterFixer.Instance.WrapIdentifier("a]b");

        result.Should().Be("[a]b]");
    }

    [Fact]
    public void PostgresParameterFixer_WrapIdentifierWithSqlPayload_PassesPayloadThrough()
    {
        var result = PostgresParameterFixer.Instance.WrapIdentifier("x\"; DROP TABLE y--");

        result.Should().Be("\"x\"; DROP TABLE y--\"");
    }
}
