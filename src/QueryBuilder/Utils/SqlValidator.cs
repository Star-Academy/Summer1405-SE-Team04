using QueryBuilder.Models;

namespace QueryBuilder.Utils;

public class SqlValidator : IValidator
{
    public bool ValidateParams(object[] parameters)
    {
        return parameters.Length > 0;
    }

    public bool ValidateStringsNotEmpty(params string[] parameters)
    {
        return parameters.Length > 0 && !parameters.Any(string.IsNullOrWhiteSpace);
    }

    public bool ValidateQuery(Query query)
    {
        if (query.SelectColumns.Count == 0)
            return false;
        if (query.FromTable.Length == 0)
            return false;

        return true;
    }
}