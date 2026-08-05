using System.Text;

namespace QueryBuilder;

public class Compiler : ICompiler
{
    private readonly List<IClauseCompiler> _clauseCompilers;
    private readonly IParameterFixer _parameterFixer;

    public Compiler(IParameterFixer parameterFixer, IEnumerable<IClauseCompiler> clauseCompilers)
    {
        _parameterFixer = parameterFixer;
        _clauseCompilers = clauseCompilers.ToList();
    }

    public (string Sql, List<(string parameterName, object value)> Bindings) Compile(Query query)
    {
        var sqlBuilder = new StringBuilder();
        var bindings = query.WhereEntries.Select((x, Index) => (_parameterFixer.FormatParameter(Index), x.Value))
            .ToList();
        _validateQuery(query);
        foreach (var clauseComiler in _clauseCompilers)
            sqlBuilder.Append(clauseComiler.Compile(query, _parameterFixer));
        return (sqlBuilder.ToString(), bindings);
    }

    public static void _validateQuery(Query query)
    {
        if (query.SelectColumns.Count == 0)
            throw new ArgumentException("no column");
        if (query.FromTable.Length == 0)
            throw new ArgumentException("no from table");
    }
}