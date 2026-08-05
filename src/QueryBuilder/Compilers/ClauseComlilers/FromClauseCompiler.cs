using QueryBuilder;

public class FromClauseCompiler : IClauseCompiler
{
    public string Compile(Query query, IParameterFixer parameterFixer)
    {
        return $" FROM {parameterFixer.WrapIdentifier(query.FromTable)}";
    }
}