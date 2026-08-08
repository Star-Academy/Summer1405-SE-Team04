using QueryBuilder.Models;

namespace UnitTests;



public class QueryModelTest
{

    [Theory]
    [InlineData("Every column must be valid name.")]
    [InlineData("Every column must be valid name.", "")]
    [InlineData("Every column must be valid name.", "a", "\t")]
    public void QueryModel_SelectEmptyArgs_ThrowsArgumentException(string expectedMessage, params string[] inputs)
    {
        var query = new Query();
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            query.Select(inputs);
        });
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void QueryModel_SelectTwice_OverridesColumns()
    {
        var query = new Query().Select("FirstName", "LastName", "Age").Select("Grade");

        Assert.Equal(["Grade"], query.SelectColumns);
    }


    [Theory]
    [InlineData("TableName must be valid name.", "")]
    [InlineData("TableName must be valid name.", "\t")]
    public void QueryModel_FromEmptyArgs_ThrowsArgumentException(string expectedMessage, string input)
    {
        var query = new Query();
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            query.From(input);
        });
        Assert.Equal(exception.Message, expectedMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("\t")]
    public void QueryModel_WhereEmptyArgs_ThrowsArgumentException(string input)
    {
        var query = new Query();
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            query.Where(input, "harchi");
        });
        Assert.Equal("ColumnName must be valid name.", exception.Message);
    }

    [Fact]
    public void QueryModel_MultiWhere_ThrowsArgumentException()
    {
        var query = new Query()
            .Where("c1", 10)
            .Where("c2", true)
            .Where("c3", "Ali");

        var expectedClauses = new[]
           {
                new WhereClause("c1", "=", 10),
                new WhereClause("c2", "=", true),
                new WhereClause("c3", "=", "Ali")
           };
        Assert.Equal(expectedClauses, query.WhereEntries);
    }

    [Fact]
    public void QueryModel_TwiceFrom_OverridesFromTable()
    {
        var query = new Query().From("FirstName").From("LastName");
        Assert.Equal("LastName", query.FromTable);
    }
}
