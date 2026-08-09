using QueryBuilder.Models;
using QueryBuilder.Utils;
using NSubstitute;

namespace UnitTests;

public class QueryModelTest
{
    [Theory]
    [InlineData("Every column must be valid name.")]
    [InlineData("Every column must be valid name.", "")]
    [InlineData("Every column must be valid name.", "a", "\t")]
    public void Select_Should_ThrowArgumentException_When_SelectArgsAreEmpty(string expectedMessage, params string[] inputs)
    {
        // Arrange
        var query = new Query();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            query.Select(inputs);
        });

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void Select_Should_OverrideColumns_When_SelectIsCalledTwice()
    {
        // Act
        var query = new Query().Select("FirstName", "LastName", "Age").Select("Grade");

        // Assert
        Assert.Equal(["Grade"], query.SelectColumns);
    }

    [Theory]
    [InlineData("TableName must be valid name.", "")]
    [InlineData("TableName must be valid name.", "\t")]
    public void From_Should_ThrowArgumentException_When_FromArgIsEmpty(string expectedMessage, string input)
    {
        // Arrange
        var query = new Query();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            query.From(input);
        });

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("\t")]
    public void Where_Should_ThrowArgumentException_When_WhereColumnIsEmpty(string input)
    {
        // Arrange
        var query = new Query();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            query.Where(input, "harchi");
        });

        // Assert
        Assert.Equal("ColumnName must be valid name.", exception.Message);
    }

    [Fact]
    public void Where_Should_AccumulateClausesInOrder_When_WhereIsCalledMultipleTimes()
    {
        // Act
        var query = new Query()
            .Where("c1", 10)
            .Where("c2", true)
            .Where("c3", "Ali");

        // Assert
        var expectedClauses = new[]
           {
                new WhereClause("c1", "=", 10),
                new WhereClause("c2", "=", true),
                new WhereClause("c3", "=", "Ali")
           };
        Assert.Equal(expectedClauses, query.WhereEntries);
    }

    [Fact]
    public void From_Should_OverrideFromTable_When_FromIsCalledTwice()
    {
        // Act
        var query = new Query().From("FirstName").From("LastName");

        // Assert
        Assert.Equal("LastName", query.FromTable);
    }

    [Fact]
    public void Query_Should_HaveEmptyState_When_IsNew()
    {
        // Arrange
        var query = new Query();

        // Assert
        query.SelectColumns.Should().BeEmpty();
        query.WhereEntries.Should().BeEmpty();
        query.FromTable.Should().Be(string.Empty);
    }

    [Theory]
    [InlineData(">")]
    [InlineData("<")]
    [InlineData(">=")]
    [InlineData("LIKE")]
    [InlineData("IN")]
    [InlineData("!=")]
    public void Where_Should_StoreOperatorVerbatim_When_CustomOperatorIsUsed(string op)
    {
        // Act
        var query = new Query().Where("Age", op, 10);

        // Assert
        query.WhereEntries.Should().Equal(new WhereClause("Age", op, 10));
    }

    [Fact]
    public void Where_Should_DefaultToEqualsOperator_When_WhereIsCalledWithTwoArgs()
    {
        // Act
        var query = new Query().Where("Age", 10);

        // Assert
        query.WhereEntries.Single().Operator.Should().Be("=");
    }

    [Fact]
    public void Where_Should_AccumulateBothEntries_When_WhereIsCalledTwiceOnSameColumn()
    {
        // Act
        var query = new Query().Where("Age", 10).Where("Age", 20);

        // Assert
        query.WhereEntries.Should().Equal(
            new WhereClause("Age", "=", 10),
            new WhereClause("Age", "=", 20));
    }

    [Fact]
    public void Where_Should_KeepBothColumns_When_SelectHasDuplicateColumns()
    {
        // Act
        var query = new Query().Select("A", "A");

        // Assert
        query.SelectColumns.Should().Equal("A", "A");
    }

    [Fact]
    public void Query_Should_ReturnSameInstance_When_FluentMethodsAreCalled()
    {
        // Arrange
        var query = new Query();

        // Act & Assert
        query.Select("A").Should().BeSameAs(query);
        query.From("T").Should().BeSameAs(query);
        query.Where("A", 1).Should().BeSameAs(query);
    }

    [Fact]
    public void Select_Should_ThrowArgumentException_When_InjectedValidatorRejectsColumns()
    {
        // Arrange
        var validatorMock = Substitute.For<IValidator>();
        validatorMock.ValidateStringsNotEmpty(Arg.Any<string[]>()).Returns(false);
        var query = new Query(validatorMock);

        // Act
        var act = () => query.Select("PerfectlyValidName");

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Every column must be valid name.");
    }

    [Fact]
    public void Select_Should_ReceiveSelectColumns_When_ValidatorIsInjected()
    {
        // Arrange
        var validatorMock = Substitute.For<IValidator>();
        validatorMock.ValidateStringsNotEmpty(Arg.Any<string[]>()).Returns(true);
        var query = new Query(validatorMock);

        // Act
        query.Select("A", "B");

        // Assert
        validatorMock.Received(1).ValidateStringsNotEmpty(Arg.Any<string[]>());
    }

    [Fact]
    public void Where_Should_AcceptOperatorUnvalidated_When_OperatorIsNull()
    {
        // Act
        var query = new Query().Where("Age", null!, 10);

        // Assert
        query.WhereEntries.Single().Operator.Should().BeNull();
    }

    [Fact]
    public void Where_Should_AcceptOperatorUnvalidated_When_OperatorIsEmpty()
    {
        // Act
        var query = new Query().Where("Age", "", 10);

        // Assert
        query.WhereEntries.Single().Operator.Should().Be("");
    }

    [Fact]
    public void Where_Should_AcceptValueUnvalidated_When_ValueIsNull()
    {
        // Act
        var query = new Query().Where("Age", null!);

        // Assert
        query.WhereEntries.Single().Value.Should().BeNull();
    }

    [Fact]
    public void Select_Should_RejectMutation_When_SelectColumnsIsModifiedDirectly()
    {
        // Arrange
        var query = new Query().Select("A");

        // Act
        var act = () => ((IList<string>)query.SelectColumns).Add("X");

        // Assert
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Select_Should_ReflectLaterSelectCalls_When_SelectColumnsReferenceIsHeld()
    {
        // Arrange
        var query = new Query().Select("A");
        var columns = query.SelectColumns;

        // Act
        query.Select("X");

        // Assert
        columns.Should().Equal("X");
    }

    [Fact]
    public void Select_Should_ThrowNullReferenceException_When_ValidatorIsNullOnFirstUse()
    {
        // Arrange
        var query = new Query(null!);

        // Act
        var act = () => query.Select("A");

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
