using QueryBuilder.Models;

namespace QueryBuilder.Utils;

public interface IValidator
{
    bool ValidateParams(object[] parameters);
    bool ValidateStringsNotEmpty(params string[] parameters);
    bool ValidateQuery(Query query);
}