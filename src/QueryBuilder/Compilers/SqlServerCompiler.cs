using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.ParameterFixer;

namespace QueryBuilder.Compilers;

internal sealed class SqlServerCompiler() : Compiler(SqlServerParameterFixer.Instance,
[
    new SelectClauseCompiler(SqlServerParameterFixer.Instance),
    new FromClauseCompiler(SqlServerParameterFixer.Instance),
    new WhereClauseCompiler(SqlServerParameterFixer.Instance)
]);