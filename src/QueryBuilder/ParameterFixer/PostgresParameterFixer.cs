namespace QueryBuilder.ParameterFixer;

internal sealed class PostgresParameterFixer : IParameterFixer
{
    private PostgresParameterFixer()
    {
    }

    public static PostgresParameterFixer Instance { get; } = new();

    public string WrapIdentifier(string identifier)
    {
        return $"\"{identifier}\"";
    }

    public string FormatParameter(int index)
    {
        return $"@p{index}";
    }
}