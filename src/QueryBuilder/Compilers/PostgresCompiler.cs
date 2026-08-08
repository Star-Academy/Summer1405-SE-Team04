using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers;

internal sealed class PostgresCompiler() : Compiler(PostgresParameterFixer.Instance,
[
    new SelectClauseCompiler(PostgresParameterFixer.Instance),
    new FromClauseCompiler(PostgresParameterFixer.Instance),
    new WhereClauseCompiler(PostgresParameterFixer.Instance)
]);