namespace QueryBuilder.Models;

public record SqlResult(string SqlQuery, List<(string parameterName, object value)> Bindings);