namespace QueryBuilder.ParameterFixer;

public interface IParameterFixer
{
    string WrapIdentifier(string identifier);

    string FormatParameter(int index);
}