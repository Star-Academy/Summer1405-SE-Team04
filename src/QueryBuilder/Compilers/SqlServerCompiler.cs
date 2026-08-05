namespace QueryBuilder;

public class SqlServerCompiler : Compiler
{
    public SqlServerCompiler() : base(
        new SqlServerParameterFixer(),
        [
            new SelectClauseCompiler(),
            new FromClauseCompiler(),
            new WhereClauseCompiler()
        ]
    )
    {
    }
}