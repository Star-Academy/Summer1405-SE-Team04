namespace QueryBuilder.ParameterFixer;

internal sealed class SqlServerParameterFixer : IParameterFixer
{
    public static SqlServerParameterFixer Instance { get; } = new SqlServerParameterFixer();
    
    private SqlServerParameterFixer(){}
    
    public string WrapIdentifier(string identifier)
    {
        return $"[{identifier}]";
    }

    public string FormatParameter(int index)
    {
        return $"@p{index}";
    }
}