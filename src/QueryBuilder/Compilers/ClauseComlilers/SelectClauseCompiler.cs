using QueryBuilder;

public class SelectClauseCompiler : IClauseCompiler
{
    public string Compile(Query query, IParameterFixer parameterFixer)
    {
        return $"SELECT {string.Join(", ", query.SelectColumns.Select(parameterFixer.WrapIdentifier))}";
    }
}