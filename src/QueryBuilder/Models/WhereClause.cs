namespace QueryBuilder;

public record WhereClause(string Column, SqlOperator Op, object Value);