using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Factory;

public class SqlServerClauseCompilerFactory : IClauseCompilerFactory
{
    public IEnumerable<IClauseCompiler> CreateClauses()
    {
        return
        [
            new SelectClauseCompiler(SqlServerParameterFixer.Instance),
            new FromClauseCompiler(SqlServerParameterFixer.Instance),
            new WhereClauseCompiler(SqlServerParameterFixer.Instance)
        ];
    }
}