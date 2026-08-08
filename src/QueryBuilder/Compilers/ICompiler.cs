using QueryBuilder.Models;

namespace QueryBuilder.Compilers;

public interface ICompiler
{
   SqlResult Compile(Query query);
}