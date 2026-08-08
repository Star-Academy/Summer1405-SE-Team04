using QueryBuilder.Models;

namespace QueryBuilder.Utils;

public class SqlValidator : IValidator
{
    public bool ValidateParams(object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        return parameters.Length > 0;
    }

    public bool ValidateStringsNotEmpty(params string[] parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        return parameters.Length > 0 && !parameters.Any(string.IsNullOrWhiteSpace);
    }

    public bool ValidateQuery(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (query.SelectColumns.Count == 0)
            return false;
        if (query.FromTable.Length == 0)
            return false;

        return true;
    }
}