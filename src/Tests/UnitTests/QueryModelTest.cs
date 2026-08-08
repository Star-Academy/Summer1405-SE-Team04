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
        exception.Message.Should().Be(expectedMessage);
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
    public void QueryModel_MultiWhere_AccumulatesClausesInOrder()
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

    [Fact]
    public void QueryModel_NewQuery_HasEmptyState()
    {
        var query = new Query();

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
    public void QueryModel_CustomOperator_StoresOperatorVerbatim(string op)
    {
        var query = new Query().Where("Age", op, 10);

        query.WhereEntries.Should().Equal(new WhereClause("Age", op, 10));
    }

    [Fact]
    public void QueryModel_TwoArgWhere_DefaultsToEqualsOperator()
    {
        var query = new Query().Where("Age", 10);

        query.WhereEntries.Single().Operator.Should().Be("=");
    }

    [Fact]
    public void QueryModel_WhereSameColumnTwice_AccumulatesBothEntries()
    {
        var query = new Query().Where("Age", 10).Where("Age", 20);

        query.WhereEntries.Should().Equal(
            new WhereClause("Age", "=", 10),
            new WhereClause("Age", "=", 20));
    }

    [Fact]
    public void QueryModel_SelectDuplicateColumns_KeepsBoth()
    {
        var query = new Query().Select("A", "A");

        query.SelectColumns.Should().Equal("A", "A");
    }

    [Fact]
    public void QueryModel_FluentMethods_ReturnSameInstance()
    {
        var query = new Query();

        query.Select("A").Should().BeSameAs(query);
        query.From("T").Should().BeSameAs(query);
        query.Where("A", 1).Should().BeSameAs(query);
    }

    [Fact]
    public void QueryModel_InjectedValidatorRejectingColumns_ThrowsArgumentException()
    {
        var validatorMock = Substitute.For<IValidator>();
        validatorMock.ValidateStringsNotEmpty(Arg.Any<string[]>()).Returns(false);
        var query = new Query(validatorMock);

        var act = () => query.Select("PerfectlyValidName");

        act.Should().Throw<ArgumentException>().WithMessage("Every column must be valid name.");
    }

    [Fact]
    public void QueryModel_InjectedValidator_ReceivesSelectColumns()
    {
        var validatorMock = Substitute.For<IValidator>();
        validatorMock.ValidateStringsNotEmpty(Arg.Any<string[]>()).Returns(true);
        var query = new Query(validatorMock);

        query.Select("A", "B");

        validatorMock.Received(1).ValidateStringsNotEmpty(Arg.Any<string[]>());
    }

    [Fact]
    public void QueryModel_NullOperator_IsAcceptedUnvalidated()
    {
        var query = new Query().Where("Age", null!, 10);

        query.WhereEntries.Single().Operator.Should().BeNull();
    }

    [Fact]
    public void QueryModel_EmptyOperator_IsAcceptedUnvalidated()
    {
        var query = new Query().Where("Age", "", 10);

        query.WhereEntries.Single().Operator.Should().Be("");
    }

    [Fact]
    public void QueryModel_NullValue_IsAcceptedUnvalidated()
    {
        var query = new Query().Where("Age", null!);

        query.WhereEntries.Single().Value.Should().BeNull();
    }

    [Fact]
    public void QueryModel_SelectColumns_RejectsMutation()
    {
        var query = new Query().Select("A");

        var act = () => ((IList<string>)query.SelectColumns).Add("X");

        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void QueryModel_SelectColumns_ReflectsLaterSelectCalls()
    {
        var query = new Query().Select("A");
        var columns = query.SelectColumns;

        query.Select("X");

        columns.Should().Equal("X");
    }

    [Fact]
    public void QueryModel_NullValidator_ThrowsNullReferenceOnFirstUse()
    {
        var query = new Query(null!);

        var act = () => query.Select("A");

        act.Should().Throw<NullReferenceException>();
    }
}
