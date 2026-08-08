using QueryBuilder.Models;
using QueryBuilder.Compilers;
using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Factory;
using QueryBuilder.ParameterFixer;
using QueryBuilder.Utils;
using NSubstitute;

namespace UnitTests;
public class CompilerTest
{
    private readonly ICompiler _sut;
    private readonly IClauseCompilerFactory _factoryMock;
    private readonly IValidator _validatorMock;
    private readonly IParameterFixer _parameterFixerMock;
    
    public CompilerTest()
    {
        _parameterFixerMock= Substitute.For<IParameterFixer>();
        _parameterFixerMock.FormatParameter(Arg.Any<int>()).Returns(x => x.Arg<int>().ToString());
        _parameterFixerMock.WrapIdentifier(Arg.Any<string>()).Returns(x => x.Arg<string>());
        
        _factoryMock = Substitute.For<IClauseCompilerFactory>();
        _factoryMock.CreateClauses().Returns([
            new SelectClauseCompiler(_parameterFixerMock),
            new FromClauseCompiler(_parameterFixerMock),
            new WhereClauseCompiler(_parameterFixerMock)
        ]);

        _validatorMock = Substitute.For<IValidator>();
        _validatorMock.ValidateQuery(Arg.Any<Query>()).Returns(true);
        
        _sut = new Compiler(_parameterFixerMock, _factoryMock, _validatorMock);
    }

    [Fact]
    public void Compile_NoSelect_ValidatorCalls()
    {
        var query = new Query().From("Student");

        _sut.Compile(query);

        _validatorMock.Received(1).ValidateQuery(query);
    }

    [Fact]
    public void Compile_SingleColumn_ProducesSingleColumnQuery()
    {
        var query = new Query().Select("FirstName").From("Student");
        var (sqlString, binings) = _sut.Compile(query);

        Assert.Equal("SELECT FirstName FROM Student", sqlString);
        Assert.Empty(binings);
    }

    [Fact]
    public void Compile_MultiColumn_ProducesCommaSeperatedColumns()
    {
        var query = new Query().Select("FirstName", "LastName", "Age").From("Student");
        var (sqlString, binings) = _sut.Compile(query);

        Assert.Equal("SELECT FirstName, LastName, Age FROM Student", sqlString);
        Assert.Empty(binings);
    }

    [Fact]
    public void Compile_NoFromTable_ValidatorCalls()
    {
        var query = new Query().Select("FirstName");
    
        _sut.Compile(query);
        
        _validatorMock.Received(1).ValidateQuery(query);
    }

    [Fact]
    public void Compile_MultiWhereClause_ProducesAndSeperatedClauses()
    {
        var query = new Query()
            .Select("FirstName")
            .From("Student")
            .Where("Age", 10)
            .Where("IsMale", true);

        var (sqlString, bindings) = _sut.Compile(query);
        Assert.Equal("SELECT FirstName FROM Student WHERE Age = 0 AND IsMale = 1", sqlString);
        Assert.Equal([("0",10), ("1",true)], bindings);

    }
}
