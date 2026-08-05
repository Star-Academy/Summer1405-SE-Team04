using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers;

public class PostgresCompiler() : Compiler(new PostgresParameterFixer(),
[
    new SelectClauseCompiler(),
    new FromClauseCompiler(),
    new WhereClauseCompiler()
]);