using System.Data.Common;
using QueryBuilder;

public interface IQueryExecutor
{
    Task<DbDataReader> ExecuteQuery(Query query);
}