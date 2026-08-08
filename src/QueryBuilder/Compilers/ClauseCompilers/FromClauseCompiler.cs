using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers.ClauseCompilers;

internal sealed class FromClauseCompiler(IParameterFixer parameterFixer) : IClauseCompiler
{
    private readonly IParameterFixer _parameterFixer = parameterFixer;
    public string Compile(Query query)
    {
        return $" FROM {_parameterFixer.WrapIdentifier(query.FromTable)}";
    }
}
