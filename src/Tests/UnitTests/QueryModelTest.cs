namespace UnitTests;

using QueryBuilder;


public class QueryModelTest
{

    [Theory]
    [InlineData("Columns can't be empty.")]
    [InlineData("Every column must be valid name.", "")]
    [InlineData("Every column must be valid name.", "a", "\t")]
    public void QueryModel_SelectEmptyArgs_ThrowsArgumentException(string expectedMessage, params string[] inputs)
    {
        var query = new Query();
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            query.Select(inputs);
        });
        Assert.Equal(exception.Message, expectedMessage);
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
            query.WhereEquals(input, "harchi");
        });
        Assert.Equal("ColumnName must be valid name.", exception.Message);
    }

    [Fact]
    public void QueryModel_MultiWhere_ThrowsArgumentException()
    {
        var query = new Query()
            .WhereEquals("c1", 10)
            .WhereEquals("c2", true)
            .WhereEquals("c3", "Ali");

        var expectedClauses = new[]
           {
                new WhereClause("c1", SqlOperator.Equal, 10),
                new WhereClause("c2", SqlOperator.Equal, true),
                new WhereClause("c3", SqlOperator.Equal, "Ali")
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
