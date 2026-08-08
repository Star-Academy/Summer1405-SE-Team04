namespace QueryBuilder.ParameterFixer;

internal sealed class SqlServerParameterFixer : IParameterFixer
{
    private SqlServerParameterFixer()
    {
    }

    public static SqlServerParameterFixer Instance { get; } = new();

    public string WrapIdentifier(string identifier)
    {
        return $"[{identifier}]";
    }

    public string FormatParameter(int index)
    {
        return $"@p{index}";
    }
}