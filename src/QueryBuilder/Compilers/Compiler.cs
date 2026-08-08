using System.Text;
using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;
using QueryBuilder.Utils;

namespace QueryBuilder.Compilers;

internal class Compiler : ICompiler
{
    private readonly List<IClauseCompiler> _clauseCompilers;
    private readonly IParameterFixer _parameterFixer;

    protected Compiler(IParameterFixer parameterFixer, IEnumerable<IClauseCompiler> clauseCompilers)
    {
        if (parameterFixer != null ||  clauseCompilers != null)
        {
            _parameterFixer = parameterFixer;
            _clauseCompilers = clauseCompilers.ToList();
        }
        else
        {
            throw new ArgumentNullException(nameof(parameterFixer));
        }
    }

    public SqlResult Compile(Query query)
    {
        var sqlBuilder = new StringBuilder();
        var bindings = query.WhereEntries.Select((clause, index) =>
                (_parameterFixer.FormatParameter(index), clause.Value))
            .ToList();
        Validator.ValidateQuery(query);
        foreach (var clauseCompiler in _clauseCompilers)
            sqlBuilder.Append(clauseCompiler.Compile(query));
        return new SqlResult(sqlBuilder.ToString(), bindings);
    }
}