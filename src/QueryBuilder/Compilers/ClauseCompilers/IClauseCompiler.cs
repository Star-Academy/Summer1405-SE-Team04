using QueryBuilder.Models;

namespace QueryBuilder.Compilers.ClauseCompilers;

public interface IClauseCompiler
{
    string Compile(Query query);
}