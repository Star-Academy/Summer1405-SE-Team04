namespace QueryBuilder;

public class SqlServerCompiler : Compiler
{
    public override string WrapIdentifier(string identifier)
    {
        return $"[{identifier}]";
    }
}