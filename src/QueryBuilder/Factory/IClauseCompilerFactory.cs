using QueryBuilder.Compilers.ClauseCompilers;

namespace QueryBuilder.Factory;

public interface IClauseCompilerFactory
{
    IEnumerable<IClauseCompiler> CreateClauses();
}