namespace QueryBuilder;

public interface ICompiler
{
    public (string Sql, List<object> Bindings) Compile(Query query);
}