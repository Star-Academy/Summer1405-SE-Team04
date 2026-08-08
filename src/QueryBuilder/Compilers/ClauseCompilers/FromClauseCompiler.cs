using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers.ClauseCompilers;

internal sealed class FromClauseCompiler(IParameterFixer parameterFixer) : IClauseCompiler
{
    public string Compile(Query query)
    {
        return $" FROM {parameterFixer.WrapIdentifier(query.FromTable)}";
    }
}
