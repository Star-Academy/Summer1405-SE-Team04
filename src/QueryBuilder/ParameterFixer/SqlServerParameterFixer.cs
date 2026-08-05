public class SqlServerParameterFixer : IParameterFixer
{
    public string WrapIdentifier(string identifier)
    {
        return $"[{identifier}]";
    }

    public string FormatParameter(int index)
    {
        return $"@p{index}";
    }
}