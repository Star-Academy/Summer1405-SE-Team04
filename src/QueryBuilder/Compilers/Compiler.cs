using System.Text;
using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers;

public class Compiler : ICompiler
{
    private readonly List<IClauseCompiler> _clauseCompilers;
    private readonly IParameterFixer _parameterFixer;

    protected Compiler(IParameterFixer parameterFixer, IEnumerable<IClauseCompiler> clauseCompilers)
    {
        _parameterFixer = parameterFixer;
        _clauseCompilers = clauseCompilers.ToList();
    }

    public (string Sql, List<(string parameterName, object value)> Bindings) Compile(Query query)
    {
        var sqlBuilder = new StringBuilder();
        var bindings = query.WhereEntries.Select((clause, index) =>
                (_parameterFixer.FormatParameter(index), clause.Value))
            .ToList();
        _validateQuery(query);
        foreach (var clauseCompiler in _clauseCompilers)
            sqlBuilder.Append(clauseCompiler.Compile(query, _parameterFixer));
        return (sqlBuilder.ToString(), bindings);
    }

    private static void _validateQuery(Query query)
    {
        if (query.SelectColumns.Count == 0)
            throw new ArgumentException("no column");
        if (query.FromTable.Length == 0)
            throw new ArgumentException("no from table");
    }
}