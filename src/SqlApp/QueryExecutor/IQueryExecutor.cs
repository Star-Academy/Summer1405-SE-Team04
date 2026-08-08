using System.Data.Common;
using QueryBuilder.Models;

namespace SqlApp.QueryExecutor;

public interface IQueryExecutor
{
    Task<DbDataReader> ExecuteQuery(Query query);
}