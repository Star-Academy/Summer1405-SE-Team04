using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers.ClauseCompilers;

internal sealed class SelectClauseCompiler(IParameterFixer parameterFixer) : IClauseCompiler
{
    private readonly IParameterFixer _parameterFixer = parameterFixer;
    public string Compile(Query query)
    {
        return $"SELECT {string.Join(", ", query.SelectColumns.Select(_parameterFixer.WrapIdentifier))}";
    }
}