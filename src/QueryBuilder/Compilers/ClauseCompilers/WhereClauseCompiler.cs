using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers.ClauseCompilers;

internal sealed class WhereClauseCompiler(IParameterFixer parameterFixer) : IClauseCompiler
{
    public string Compile(Query query)
    {
        if (query.WhereEntries.Count == 0)
            return string.Empty;

        return $" WHERE {string.Join(" AND ", query.WhereEntries.Select((whereClause, index) =>
            $"{parameterFixer.WrapIdentifier(whereClause.Column)} {whereClause.Operator} {parameterFixer.FormatParameter(index)}"))}";
    }
}