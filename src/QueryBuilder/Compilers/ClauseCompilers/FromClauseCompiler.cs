using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers.ClauseCompilers;

public class FromClauseCompiler : IClauseCompiler
{
    public string Compile(Query query, IParameterFixer parameterFixer)
    {
        return $" FROM {parameterFixer.WrapIdentifier(query.FromTable)}";
    }
}