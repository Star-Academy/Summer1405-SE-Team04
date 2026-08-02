namespace QueryBuilder;

public class PostgresCompiler : Compiler
{
    public override string WrapIdentifier(string identifier)
    {
        return $"\"{identifier}\"";
    }
}