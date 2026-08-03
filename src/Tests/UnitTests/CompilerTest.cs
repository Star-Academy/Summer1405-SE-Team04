namespace UnitTests;

using NSubstitute;
using QueryBuilder;


public class CompilerTest
{
    private readonly Compiler _compilerMock;
    public CompilerTest()
    {
        _compilerMock = Substitute.ForPartsOf<Compiler>();
        _compilerMock.WrapIdentifier(Arg.Any<string>()).Returns(x => x.Arg<string>());
    }

    [Fact]
    public void Compile_NoSelect_ThrowsArgumentException()
    {
        var query = new Query().From("Student");

        Assert.Throws<ArgumentException>(() =>
        {
            var (sqlString, binings) = _compilerMock.Compile(query);
        });
    }

    [Fact]
    public void Compile_SingleColumn_ProducesSingleColumnQuery()
    {
        var query = new Query().Select("FirstName").From("Student");
        var (sqlString, binings) = _compilerMock.Compile(query);

        Assert.Equal("SELECT FirstName FROM Student", sqlString);
        Assert.Empty(binings);
    }

    [Fact]
    public void Compile_MultiColumn_ProducesCommaSeperatedColumns()
    {
        var query = new Query().Select("FirstName", "LastName", "Age").From("Student");
        var (sqlString, binings) = _compilerMock.Compile(query);

        Assert.Equal("SELECT FirstName, LastName, Age FROM Student", sqlString);
        Assert.Empty(binings);
    }

    [Fact]
    public void Compile_NoFromTable_ThrowsArgumentException()
    {
        var query = new Query().Select("FirstName");

        Assert.Throws<ArgumentException>(() =>
        {
            var (sqlString, binings) = _compilerMock.Compile(query);
        });
    }

    [Fact]
    public void Compile_MultiWhereClause_ProducesAndSeperatedClauses()
    {
        var query = new Query()
            .Select("FirstName")
            .From("Student")
            .WhereEquals("Age", 10)
            .WhereEquals("IsMale", true);

        var (sqlString, bindings) = _compilerMock.Compile(query);
        Assert.Equal("SELECT FirstName FROM Student WHERE Age = @p0 AND IsMale = @p1", sqlString);
        Assert.Equal([10, true], bindings);

    }
}
