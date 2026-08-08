using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Factory;

public class PostgresClauseCompilerFactory : IClauseCompilerFactory
{
    public IEnumerable<IClauseCompiler> CreateClauses()
    {
        return
        [
            new SelectClauseCompiler(PostgresParameterFixer.Instance),
            new FromClauseCompiler(PostgresParameterFixer.Instance),
            new WhereClauseCompiler(PostgresParameterFixer.Instance)
        ];
    }
}