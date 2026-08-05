namespace QueryBuilder;

public interface ICompiler
{
    (string Sql, List<(string parameterName, object value)> Bindings) Compile(Query query);
}