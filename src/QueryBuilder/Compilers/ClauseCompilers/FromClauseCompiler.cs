using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers.ClauseCompilers;

internal sealed class FromClauseCompiler : IClauseCompiler
{
    private readonly IParameterFixer _parameterFixer;

    public FromClauseCompiler(IParameterFixer parameterFixer)
    {

        _parameterFixer = parameterFixer ?? throw new ArgumentNullException(nameof(parameterFixer));
    }

    public string Compile(Query query)
    {
        return $" FROM {_parameterFixer.WrapIdentifier(query.FromTable)}";
    }
}