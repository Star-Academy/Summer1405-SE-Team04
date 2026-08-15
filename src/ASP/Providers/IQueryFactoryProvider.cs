using SqlKata.Execution;

namespace ASP.Providers;

public interface IQueryFactoryProvider : IDisposable
{
    QueryFactory GetQueryFactory(string dbName);
}