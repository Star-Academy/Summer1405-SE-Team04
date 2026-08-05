namespace QueryBuilder;

public class PostgresCompiler : Compiler
{
    public PostgresCompiler() : base(
        new PostgresParameterFixer(),
        [
            new SelectClauseCompiler(),
            new FromClauseCompiler(),
            new WhereClauseCompiler()
        ]
    )
    {
    }
}