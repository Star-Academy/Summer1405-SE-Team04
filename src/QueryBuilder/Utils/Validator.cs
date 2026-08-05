public static class Validator
{
    public static bool ValidateParams(object[] parameters)
    {
        return parameters.Count() != 0;
    }

    public static bool ValidateStringsNotEmpty(params string[] parameters)
    {
        return !parameters.Any(x => string.IsNullOrWhiteSpace(x));
    }
}