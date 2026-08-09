using QueryBuilder.Models;
using QueryBuilder.Utils;

namespace UnitTests;

public class QueryModelTest
{
    [Theory]
    [InlineData("Every column must be valid name.")]
    [InlineData("Every column must be valid name.", "")]
    [InlineData("Every column must be valid name.", "a", "\t")]
    public void Select_ShouldThrowArgumentException_WhenSelectArgsAreEmpty(string expectedMessage, params string[] inputs)
    {
        // Arrange
        var query = new Query();

        // Act
        var act = () => query.Select(inputs);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage(expectedMessage);
    }

    [Fact]
    public void Select_ShouldOverrideColumns_WhenSelectIsCalledTwice()
    {
        // Act
        var query = new Query().Select("FirstName", "LastName", "Age").Select("Grade");

        // Assert
        query.SelectColumns.Should().Equal("Grade");
    }

    [Theory]
    [InlineData("TableName must be valid name.", "")]
    [InlineData("TableName must be valid name.", "\t")]
    public void From_ShouldThrowArgumentException_WhenFromArgIsEmpty(string expectedMessage, string input)
    {
        // Arrange
        var query = new Query();

        // Act
        var act = () => query.From(input);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage(expectedMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("\t")]
    public void Where_ShouldThrowArgumentException_WhenWhereColumnIsEmpty(string input)
    {
        // Arrange
        var query = new Query();

        // Act
        var act = () => query.Where(input, "harchi");

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("ColumnName must be valid name.");
    }

    [Fact]
    public void Where_ShouldAccumulateClausesInOrder_WhenWhereIsCalledMultipleTimes()
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
        query.WhereEntries.Should().Equal(expectedClauses);
    }

    [Fact]
    public void From_ShouldOverrideFromTable_WhenFromIsCalledTwice()
    {
        // Act
        var query = new Query().From("FirstName").From("LastName");

        // Assert
        query.FromTable.Should().Be("LastName");
    }

    [Fact]
    public void Query_ShouldHaveEmptyState_WhenIsNew()
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
    public void Where_ShouldStoreOperatorVerbatim_WhenCustomOperatorIsUsed(string op)
    {
        // Act
        var query = new Query().Where("Age", op, 10);

        // Assert
        query.WhereEntries.Should().Equal(new WhereClause("Age", op, 10));
    }

    [Fact]
    public void Where_ShouldDefaultToEqualsOperator_WhenWhereIsCalledWithTwoArgs()
    {
        // Act
        var query = new Query().Where("Age", 10);

        // Assert
        query.WhereEntries.Single().Operator.Should().Be("=");
    }

    [Fact]
    public void Where_ShouldAccumulateBothEntries_WhenWhereIsCalledTwiceOnSameColumn()
    {
        // Act
        var query = new Query().Where("Age", 10).Where("Age", 20);

        // Assert
        query.WhereEntries.Should().Equal(
            new WhereClause("Age", "=", 10),
            new WhereClause("Age", "=", 20));
    }

    [Fact]
    public void Where_ShouldKeepBothColumns_WhenSelectHasDuplicateColumns()
    {
        // Act
        var query = new Query().Select("A", "A");

        // Assert
        query.SelectColumns.Should().Equal("A", "A");
    }

    [Fact]
    public void Select_ShouldReturnSameInstance_WhenCalled()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = query.Select("A");

        // Assert
        result.Should().BeSameAs(query);
    }

    [Fact]
    public void From_ShouldReturnSameInstance_WhenCalled()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = query.From("T");

        // Assert
        result.Should().BeSameAs(query);
    }

    [Fact]
    public void Where_ShouldReturnSameInstance_WhenCalled()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = query.Where("A", 1);

        // Assert
        result.Should().BeSameAs(query);
    }

    [Fact]
    public void Select_ShouldThrowArgumentException_WhenInjectedValidatorRejectsColumns()
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
    public void Select_ShouldReceiveSelectColumns_WhenValidatorIsInjected()
    {
        // Arrange
        var validatorMock = Substitute.For<IValidator>();
        validatorMock.ValidateStringsNotEmpty(Arg.Any<string[]>()).Returns(true);
        var query = new Query(validatorMock);

        // Act
        query.Select("A", "B");

        // Assert
        validatorMock.Received(1).ValidateStringsNotEmpty("A", "B");
    }

    [Fact]
    public void Where_ShouldAcceptOperatorUnvalidated_WhenOperatorIsNull()
    {
        // Act
        var query = new Query().Where("Age", null!, 10);

        // Assert
        query.WhereEntries.Single().Operator.Should().BeNull();
    }

    [Fact]
    public void Where_ShouldAcceptOperatorUnvalidated_WhenOperatorIsEmpty()
    {
        // Act
        var query = new Query().Where("Age", "", 10);

        // Assert
        query.WhereEntries.Single().Operator.Should().Be("");
    }

    [Fact]
    public void Where_ShouldAcceptValueUnvalidated_WhenValueIsNull()
    {
        // Act
        var query = new Query().Where("Age", null!);

        // Assert
        query.WhereEntries.Single().Value.Should().BeNull();
    }

    [Fact]
    public void Select_ShouldRejectMutation_WhenSelectColumnsIsModifiedDirectly()
    {
        // Arrange
        var query = new Query().Select("A");

        // Act
        var act = () => ((IList<string>)query.SelectColumns).Add("X");

        // Assert
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Select_ShouldReflectLaterSelectCalls_WhenSelectColumnsReferenceIsHeld()
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
    public void Select_ShouldThrowNullReferenceException_WhenValidatorIsNullOnFirstUse()
    {
        // Arrange
        var query = new Query(null!);

        // Act
        var act = () => query.Select("A");

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
