using System.Text;
using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Factory;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;
using QueryBuilder.Utils;

namespace QueryBuilder.Compilers;

internal class Compiler : ICompiler
{
    private readonly List<IClauseCompiler> _clauseCompilers;
    private readonly IParameterFixer _parameterFixer;

    private readonly IValidator _validator;

    public Compiler(IParameterFixer parameterFixer, IClauseCompilerFactory clauseCompilerFactory, IValidator validator)
        : this(parameterFixer, clauseCompilerFactory.CreateClauses(), validator)
    {
    }

    protected Compiler(IParameterFixer parameterFixer, IEnumerable<IClauseCompiler> clauseCompilers,
        IValidator validator)
    {
        ArgumentNullException.ThrowIfNull(parameterFixer);
        ArgumentNullException.ThrowIfNull(clauseCompilers);
        ArgumentNullException.ThrowIfNull(validator);

        _parameterFixer = parameterFixer;
        _clauseCompilers = clauseCompilers.ToList();
        _validator = validator;
    }

    public SqlResult Compile(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);
        var bindings = query.WhereEntries.Select((clause, index) =>
                (_parameterFixer.FormatParameter(index), clause.Value))
            .ToList();

        if (!_validator.ValidateQuery(query))
            throw new InvalidOperationException("Query is not valid.");
        
        var sqlQueryString = string.Join(" ", _clauseCompilers.Select(x => x.Compile(query)));

        return new SqlResult(sqlQueryString, bindings);
    }
}