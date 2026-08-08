using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers.ClauseCompilers;

internal sealed class SelectClauseCompiler : IClauseCompiler
{
    private readonly IParameterFixer _parameterFixer;

    public SelectClauseCompiler(IParameterFixer parameterFixer)
    {
        _parameterFixer = parameterFixer;
    }

    public string Compile(Query query)
    {
        return $"SELECT {string.Join(", ", query.SelectColumns.Select(_parameterFixer.WrapIdentifier))}";
    }
}