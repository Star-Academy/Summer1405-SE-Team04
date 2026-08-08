using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers.ClauseCompilers;

internal sealed class WhereClauseCompiler : IClauseCompiler
{
    private readonly IParameterFixer _parameterFixer;

    public WhereClauseCompiler(IParameterFixer parameterFixer)
    {
        _parameterFixer = parameterFixer;
    }

    public string Compile(Query query)
    {
        if (query.WhereEntries.Count == 0)
            return string.Empty;

        return $" WHERE {string.Join(" AND ", query.WhereEntries.Select((whereClause, index) =>
            $"{_parameterFixer.WrapIdentifier(whereClause.Column)} {whereClause.Operator} {_parameterFixer.FormatParameter(index)}"))}";
    }
}