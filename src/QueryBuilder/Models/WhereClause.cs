namespace QueryBuilder.Models;

public record WhereClause(string Column, string Operator, object Value);