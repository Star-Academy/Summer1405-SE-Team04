using QueryBuilder;

public class WhereClauseCompiler : IClauseCompiler
{
    public string Compile(Query query, IParameterFixer parameterFixer)
    {
        if (query.WhereEntries.Count == 0)
            return string.Empty;

        return $" WHERE {string.Join(" AND ", query.WhereEntries.Select((whereClause, index) =>
        {
            return $"{parameterFixer.WrapIdentifier(whereClause.Column)} {whereClause.Op} {parameterFixer.FormatParameter(index)}";
        }))}";
    }
}