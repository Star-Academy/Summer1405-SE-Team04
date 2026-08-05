using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers;

public class SqlServerCompiler() : Compiler(new SqlServerParameterFixer(),
[
    new SelectClauseCompiler(),
    new FromClauseCompiler(),
    new WhereClauseCompiler()
]);