using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers.ClauseCompilers;

public interface IClauseCompiler
{
    string Compile(Query query);
}