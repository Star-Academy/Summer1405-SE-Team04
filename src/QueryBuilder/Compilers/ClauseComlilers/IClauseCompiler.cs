using QueryBuilder;

public interface IClauseCompiler
{
    string Compile(Query query, IParameterFixer parameterFixer);
}