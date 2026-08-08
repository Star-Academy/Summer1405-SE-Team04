namespace QueryBuilder.ParameterFixer;

internal sealed class PostgresParameterFixer : IParameterFixer
{
    public static PostgresParameterFixer Instance { get; } = new PostgresParameterFixer();
    
    private PostgresParameterFixer()
    {
    }

    public string WrapIdentifier(string identifier)
    {
        return $"\"{identifier}\"";
    }

    public string FormatParameter(int index)
    {
        return $"@p{index}";
    }
}