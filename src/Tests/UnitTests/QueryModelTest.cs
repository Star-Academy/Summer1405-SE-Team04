namespace UnitTests;

using QueryBuilder;


public class QueryModelTest
{
    [Theory]
    [InlineData("Columns can't be empty.")]
    [InlineData("Every column must be valid name.", "")]
    [InlineData("Every column must be valid name.", "a", "\t")]
    public void SelectEmptyArgsTest(string expectedMessage, params string[] inputs)
    {
        var query = new Query();
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            query.Select(inputs);
        });
        Assert.Equal(exception.Message, expectedMessage);
    }

    [Theory]
    [InlineData("TableName must be valid name.", "")]
    [InlineData("TableName must be valid name.", "\t")]
    public void FromEmptyArgsTest(string expectedMessage, string input)
    {
        var query = new Query();
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            query.From(input);
        });
        Assert.Equal(exception.Message, expectedMessage);
    }

    [Theory]
    [InlineData("ColumnName must be valid name.", "")]
    [InlineData("ColumnName must be valid name.", "\t")]
    public void WhereEmptyArgsTest(string expectedMessage, string input)
    {
        var query = new Query();
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            query.Where(input,"harchi");
        });
        Assert.Equal(exception.Message, expectedMessage);
    }

    [Fact]
    public void MultiWhereTest()
    {
        var query= new Query();
        Assert.Throws<ArgumentException>(() =>
        {
            query.Where("c1",10).Where("",20);
        });
    }
}
