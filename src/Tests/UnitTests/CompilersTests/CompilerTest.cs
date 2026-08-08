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
        _parameterFixerMock = Substitute.For<IParameterFixer>();
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
        // Arrange
        var query = new Query().From("Student");

        // Act
        _sut.Compile(query);

        // Assert
        _validatorMock.Received(1).ValidateQuery(query);
    }

    [Fact]
    public void Compile_SingleColumn_ProducesSingleColumnQuery()
    {
        // Arrange
        var query = new Query().Select("FirstName").From("Student");

        // Act
        var (sqlString, bindings) = _sut.Compile(query);

        // Assert
        Assert.Equal("SELECT FirstName FROM Student", sqlString);
        Assert.Empty(bindings);
    }

    [Fact]
    public void Compile_MultiColumn_ProducesCommaSeperatedColumns()
    {
        // Arrange
        var query = new Query().Select("FirstName", "LastName", "Age").From("Student");

        // Act
        var (sqlString, bindings) = _sut.Compile(query);

        // Assert
        Assert.Equal("SELECT FirstName, LastName, Age FROM Student", sqlString);
        Assert.Empty(bindings);
    }

    [Fact]
    public void Compile_NoFromTable_ValidatorCalls()
    {
        // Arrange
        var query = new Query().Select("FirstName");

        // Act
        _sut.Compile(query);

        // Assert
        _validatorMock.Received(1).ValidateQuery(query);
    }

    [Fact]
    public void Compile_MultiWhereClause_ProducesAndSeperatedClauses()
    {
        // Arrange
        var query = new Query()
            .Select("FirstName")
            .From("Student")
            .Where("Age", 10)
            .Where("IsMale", true);

        // Act
        var (sqlString, bindings) = _sut.Compile(query);

        // Assert
        Assert.Equal("SELECT FirstName FROM Student WHERE Age = 0 AND IsMale = 1", sqlString);
        Assert.Equal([("0",10), ("1",true)], bindings);
    }

    [Fact]
    public void Compile_InvalidQuery_ThrowsInvalidOperationException()
    {
        // Arrange
        _validatorMock.ValidateQuery(Arg.Any<Query>()).Returns(false);
        var query = new Query();

        // Act
        var act = () => _sut.Compile(query);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("Query is not valid.");
    }

    [Fact]
    public void Compile_InvalidQuery_DoesNotInvokeClauseCompilers()
    {
        // Arrange
        _validatorMock.ValidateQuery(Arg.Any<Query>()).Returns(false);
        var clauseCompilerMock = Substitute.For<IClauseCompiler>();
        _factoryMock.CreateClauses().Returns([clauseCompilerMock]);
        var sut = new Compiler(_parameterFixerMock, _factoryMock, _validatorMock);
        var query = new Query();

        // Act
        var act = () => sut.Compile(query);

        // Assert
        act.Should().Throw<InvalidOperationException>();
        clauseCompilerMock.DidNotReceive().Compile(Arg.Any<Query>());
    }

    [Fact]
    public void Compiler_NullParameterFixer_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new Compiler(null!, _factoryMock, _validatorMock);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("parameterFixer");
    }

    [Fact]
    public void Compiler_NullValidator_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new Compiler(_parameterFixerMock, _factoryMock, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("validator");
    }

    [Fact]
    public void Compiler_FactoryReturningNullClauses_ThrowsArgumentNullException()
    {
        // Arrange
        _factoryMock.CreateClauses().Returns((IEnumerable<IClauseCompiler>)null!);

        // Act
        var act = () => new Compiler(_parameterFixerMock, _factoryMock, _validatorMock);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("clauseCompilers");
    }

    [Fact]
    public void Compiler_NullFactory_ThrowsNullReferenceException()
    {
        // Act
        var act = () => new Compiler(_parameterFixerMock, null!, _validatorMock);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Compile_InvalidQuery_StillFormatsParameters()
    {
        // Arrange
        _validatorMock.ValidateQuery(Arg.Any<Query>()).Returns(false);
        var query = new Query().Where("Age", 10);

        // Act
        var act = () => _sut.Compile(query);

        // Assert
        act.Should().Throw<InvalidOperationException>();
        _parameterFixerMock.Received(1).FormatParameter(0);
    }

    [Fact]
    public void Compile_EmptyClauseCompilerList_ProducesEmptySqlWithBindings()
    {
        // Arrange
        _factoryMock.CreateClauses().Returns([]);
        var sut = new Compiler(_parameterFixerMock, _factoryMock, _validatorMock);
        var query = new Query().Where("Age", 10);

        // Act
        var (sqlString, bindings) = sut.Compile(query);

        // Assert
        sqlString.Should().Be("");
        bindings.Should().Equal([("0", (object)10)]);
    }

    [Fact]
    public void Compile_ReorderedClauseCompilers_ProducesSqlInFactoryOrder()
    {
        // Arrange
        _factoryMock.CreateClauses().Returns([
            new WhereClauseCompiler(_parameterFixerMock),
            new FromClauseCompiler(_parameterFixerMock),
            new SelectClauseCompiler(_parameterFixerMock)
        ]);
        var sut = new Compiler(_parameterFixerMock, _factoryMock, _validatorMock);
        var query = new Query().Select("A").From("T").Where("C", 1);

        // Act
        var (sqlString, _) = sut.Compile(query);

        // Assert
        sqlString.Should().Be(" WHERE C = 0 FROM TSELECT A");
    }

    [Fact]
    public void Compile_Query_InvokesEveryClauseCompilerOnceWithSameQuery()
    {
        // Arrange
        var firstClauseMock = Substitute.For<IClauseCompiler>();
        var secondClauseMock = Substitute.For<IClauseCompiler>();
        var thirdClauseMock = Substitute.For<IClauseCompiler>();
        _factoryMock.CreateClauses().Returns([firstClauseMock, secondClauseMock, thirdClauseMock]);
        var sut = new Compiler(_parameterFixerMock, _factoryMock, _validatorMock);
        var query = new Query();

        // Act
        sut.Compile(query);

        // Assert
        firstClauseMock.Received(1).Compile(query);
        secondClauseMock.Received(1).Compile(query);
        thirdClauseMock.Received(1).Compile(query);
    }

    [Fact]
    public void Compile_NullQuery_ThrowsNullReferenceException()
    {
        // Act
        var act = () => _sut.Compile(null!);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Compile_NoWhereClause_DoesNotFormatParameters()
    {
        // Arrange
        var query = new Query().Select("A").From("T");

        // Act
        _sut.Compile(query);

        // Assert
        _parameterFixerMock.DidNotReceive().FormatParameter(Arg.Any<int>());
    }

    [Fact]
    public void SqlResult_TwoResultsWithIdenticalContent_AreNotEqual()
    {
        // Arrange
        var query = new Query().Select("A").From("T");

        // Act
        var firstResult = _sut.Compile(query);
        var secondResult = _sut.Compile(query);

        // Assert
        firstResult.Should().NotBe(secondResult);
    }
}
