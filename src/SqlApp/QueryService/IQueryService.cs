using System.Data.Common;

namespace SqlApp.QueryService;

public interface IQueryService
{
    Task PrintQueryResultAsync(DbDataReader reader);
}