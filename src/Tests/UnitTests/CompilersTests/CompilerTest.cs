using QueryBuilder.Compilers;
using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Factory;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;
using QueryBuilder.Utils;

namespace UnitTests.CompilersTests;

public class CompilerTest
{
    private readonly IClauseCompilerFactory _factoryMock;
    private readonly IParameterFixer _parameterFixerMock;
    private readonly ICompiler _sut;
    private readonly IValidator _validatorMock;

    public CompilerTest()
    {
        _parameterFixerMock = Substitute.For<IParameterFixer>();
        _parameterFixerMock.FormatParameter(Arg.Any<int>()).Returns(x => x.Arg<int>().ToString());

        var selectClauseCompilerMock = Substitute.For<IClauseCompiler>();
        var fromClauseCompilerMock = Substitute.For<IClauseCompiler>();
        var whereClauseCompilerMock = Substitute.For<IClauseCompiler>();
        selectClauseCompilerMock.Compile(Arg.Any<Query>()).Returns("SELECT");
        fromClauseCompilerMock.Compile(Arg.Any<Query>()).Returns("FROM");
        whereClauseCompilerMock.Compile(Arg.Any<Query>()).Returns("WHERE");

        _factoryMock = Substitute.For<IClauseCompilerFactory>();
        _factoryMock.CreateClauses().Returns([
            selectClauseCompilerMock,
            fromClauseCompilerMock,
            whereClauseCompilerMock
        ]);

        _validatorMock = Substitute.For<IValidator>();
        _validatorMock.ValidateQuery(Arg.Any<Query>()).Returns(true);

        _sut = new Compiler(_parameterFixerMock, _factoryMock, _validatorMock);
    }

    [Fact]
    public void Compile_ShouldInvokeValidator_WhenQueryHasNoSelectColumns()
    {
        // Arrange
        var query = new Query().From("Student");

        // Act
        _sut.Compile(query);

        // Assert
        _validatorMock.Received(1).ValidateQuery(query);
    }

    [Fact]
    public void Compile_ShouldInvokeValidator_WhenQueryHasNoFromTable()
    {
        // Arrange
        var query = new Query().Select("FirstName");

        // Act
        _sut.Compile(query);

        // Assert
        _validatorMock.Received(1).ValidateQuery(query);
    }

    [Fact]
    public void Compile_ShouldProduceOneBindingPerWhereEntry_WhenQueryHasMultipleWhereClauses()
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
        sqlString.Should().Be("SELECT FROM WHERE");
        bindings.Should().Equal(("0", 10), ("1", true));
    }

    [Fact]
    public void Compile_ShouldThrowInvalidOperationException_WhenQueryIsInvalid()
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
    public void Compile_ShouldNotInvokeClauseCompilers_WhenQueryIsInvalid()
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
    public void Compile_ShouldThrowArgumentNullException_WhenParameterFixerIsNull()
    {
        // Act
        var act = () => new Compiler(null!, _factoryMock, _validatorMock);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("parameterFixer");
    }

    [Fact]
    public void Compile_ShouldThrowArgumentNullException_WhenValidatorIsNull()
    {
        // Act
        var act = () => new Compiler(_parameterFixerMock, _factoryMock, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("validator");
    }

    [Fact]
    public void Compile_ShouldThrowArgumentNullException_WhenFactoryReturnsNullClauses()
    {
        // Arrange
        _factoryMock.CreateClauses().Returns((IEnumerable<IClauseCompiler>)null!);

        // Act
        var act = () => new Compiler(_parameterFixerMock, _factoryMock, _validatorMock);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("clauseCompilers");
    }

    [Fact]
    public void Compile_ShouldThrowNullReferenceException_WhenFactoryIsNull()
    {
        // Act
        var act = () => new Compiler(_parameterFixerMock, null!, _validatorMock);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Compile_ShouldStillFormatParameters_WhenQueryIsInvalid()
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
    public void Compile_ShouldProduceEmptySqlWithBindings_WhenClauseCompilerListIsEmpty()
    {
        // Arrange
        _factoryMock.CreateClauses().Returns([]);
        var sut = new Compiler(_parameterFixerMock, _factoryMock, _validatorMock);
        var query = new Query().Where("Age", 10);

        // Act
        var (sqlString, bindings) = sut.Compile(query);

        // Assert
        sqlString.Should().Be("");
        bindings.Should().Equal(("0", 10));
    }

    [Fact]
    public void Compile_ShouldProduceSqlInFactoryOrder_WhenClauseCompilersAreReordered()
    {
        // Arrange
        var query = new Query().Select("A").From("T").Where("C", 1);

        var whereClauseCompilerMock = Substitute.For<IClauseCompiler>();
        var fromClauseCompilerMock = Substitute.For<IClauseCompiler>();
        var selectClauseCompilerMock = Substitute.For<IClauseCompiler>();

        whereClauseCompilerMock.Compile(query).Returns("WHERE C = 0");
        fromClauseCompilerMock.Compile(query).Returns("FROM T");
        selectClauseCompilerMock.Compile(query).Returns("SELECT A");

        _factoryMock.CreateClauses().Returns([
            whereClauseCompilerMock,
            fromClauseCompilerMock,
            selectClauseCompilerMock
        ]);

        var sut = new Compiler(_parameterFixerMock, _factoryMock, _validatorMock);

        // Act
        var (sqlString, _) = sut.Compile(query);

        // Assert
        sqlString.Should().Be("WHERE C = 0 FROM T SELECT A");
    }

    [Fact]
    public void Compile_ShouldInvokeEachClauseCompilerExactlyOnce_WhenCompilingAQuery()
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
    public void Compile_ShouldThrowNullReferenceException_WhenQueryIsNull()
    {
        // Act
        var act = () => _sut.Compile(null!);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Compile_ShouldNotFormatParameters_WhenQueryHasNoWhereClause()
    {
        // Arrange
        var query = new Query().Select("A").From("T");

        // Act
        _sut.Compile(query);

        // Assert
        _parameterFixerMock.DidNotReceive().FormatParameter(Arg.Any<int>());
    }

    [Fact]
    public void Compile_ShouldNotBeEqual_WhenTwoResultsHaveIdenticalContent()
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