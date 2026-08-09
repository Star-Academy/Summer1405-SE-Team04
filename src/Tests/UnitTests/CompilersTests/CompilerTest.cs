using QueryBuilder.Compilers;
using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Factory;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;
using QueryBuilder.Utils;

namespace UnitTests.CompilersTests;

public class CompilerTest
{
    private readonly IClauseCompilerFactory _factory;
    private readonly IParameterFixer _parameterFixer;
    private readonly ICompiler _sut;
    private readonly IValidator _validator;

    public CompilerTest()
    {
        _parameterFixer = Substitute.For<IParameterFixer>();
        _parameterFixer.FormatParameter(Arg.Any<int>()).Returns(x => x.Arg<int>().ToString());

        var selectClauseCompiler = Substitute.For<IClauseCompiler>();
        var fromClauseCompiler = Substitute.For<IClauseCompiler>();
        var whereClauseCompiler = Substitute.For<IClauseCompiler>();
        selectClauseCompiler.Compile(Arg.Any<Query>()).Returns("SELECT");
        fromClauseCompiler.Compile(Arg.Any<Query>()).Returns("FROM");
        whereClauseCompiler.Compile(Arg.Any<Query>()).Returns("WHERE");

        _factory = Substitute.For<IClauseCompilerFactory>();
        _factory.CreateClauses().Returns([
            selectClauseCompiler,
            fromClauseCompiler,
            whereClauseCompiler
        ]);

        _validator = Substitute.For<IValidator>();
        _validator.ValidateQuery(Arg.Any<Query>()).Returns(true);

        _sut = new Compiler(_parameterFixer, _factory, _validator);
    }

    [Fact]
    public void Compile_ShouldInvokeValidator_WhenQueryHasNoSelectColumns()
    {
        // Arrange
        var query = new Query().From("Student");

        // Act
        _sut.Compile(query);

        // Assert
        _validator.Received(1).ValidateQuery(query);
    }

    [Fact]
    public void Compile_ShouldInvokeValidator_WhenQueryHasNoFromTable()
    {
        // Arrange
        var query = new Query().Select("FirstName");

        // Act
        _sut.Compile(query);

        // Assert
        _validator.Received(1).ValidateQuery(query);
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
        _validator.ValidateQuery(Arg.Any<Query>()).Returns(false);
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
        _validator.ValidateQuery(Arg.Any<Query>()).Returns(false);
        var clauseCompiler = Substitute.For<IClauseCompiler>();
        _factory.CreateClauses().Returns([clauseCompiler]);
        var sut = new Compiler(_parameterFixer, _factory, _validator);
        var query = new Query();

        // Act
        var act = () => sut.Compile(query);

        // Assert
        act.Should().Throw<InvalidOperationException>();
        clauseCompiler.DidNotReceive().Compile(Arg.Any<Query>());
    }

    [Fact]
    public void Compile_ShouldThrowArgumentNullException_WhenParameterFixerIsNull()
    {
        // Act
        var act = () => new Compiler(null!, _factory, _validator);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("parameterFixer");
    }

    [Fact]
    public void Compile_ShouldThrowArgumentNullException_WhenValidatorIsNull()
    {
        // Act
        var act = () => new Compiler(_parameterFixer, _factory, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("validator");
    }

    [Fact]
    public void Compile_ShouldThrowArgumentNullException_WhenFactoryReturnsNullClauses()
    {
        // Arrange
        _factory.CreateClauses().Returns((IEnumerable<IClauseCompiler>)null!);

        // Act
        var act = () => new Compiler(_parameterFixer, _factory, _validator);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("clauseCompilers");
    }

    [Fact]
    public void Compile_ShouldThrowNullReferenceException_WhenFactoryIsNull()
    {
        // Act
        var act = () => new Compiler(_parameterFixer, null!, _validator);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Compile_ShouldStillFormatParameters_WhenQueryIsInvalid()
    {
        // Arrange
        _validator.ValidateQuery(Arg.Any<Query>()).Returns(false);
        var query = new Query().Where("Age", 10);

        // Act
        var act = () => _sut.Compile(query);

        // Assert
        act.Should().Throw<InvalidOperationException>();
        _parameterFixer.Received(1).FormatParameter(0);
    }

    [Fact]
    public void Compile_ShouldProduceEmptySqlWithBindings_WhenClauseCompilerListIsEmpty()
    {
        // Arrange
        _factory.CreateClauses().Returns([]);
        var sut = new Compiler(_parameterFixer, _factory, _validator);
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

        var whereClauseCompiler = Substitute.For<IClauseCompiler>();
        var fromClauseCompiler = Substitute.For<IClauseCompiler>();
        var selectClauseCompiler = Substitute.For<IClauseCompiler>();

        whereClauseCompiler.Compile(query).Returns("WHERE C = 0");
        fromClauseCompiler.Compile(query).Returns("FROM T");
        selectClauseCompiler.Compile(query).Returns("SELECT A");

        _factory.CreateClauses().Returns([
            whereClauseCompiler,
            fromClauseCompiler,
            selectClauseCompiler
        ]);

        var sut = new Compiler(_parameterFixer, _factory, _validator);

        // Act
        var (sqlString, _) = sut.Compile(query);

        // Assert
        sqlString.Should().Be("WHERE C = 0 FROM T SELECT A");
    }

    [Fact]
    public void Compile_ShouldInvokeEachClauseCompilerExactlyOnce_WhenCompilingAQuery()
    {
        // Arrange
        var firstClause = Substitute.For<IClauseCompiler>();
        var secondClause = Substitute.For<IClauseCompiler>();
        var thirdClause = Substitute.For<IClauseCompiler>();
        _factory.CreateClauses().Returns([firstClause, secondClause, thirdClause]);
        var sut = new Compiler(_parameterFixer, _factory, _validator);
        var query = new Query();

        // Act
        sut.Compile(query);

        // Assert
        firstClause.Received(1).Compile(query);
        secondClause.Received(1).Compile(query);
        thirdClause.Received(1).Compile(query);
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
        _parameterFixer.DidNotReceive().FormatParameter(Arg.Any<int>());
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