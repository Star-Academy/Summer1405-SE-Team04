using SqlKata.Execution;

namespace Providers;

public interface IQueryFactoryProvider : IDisposable
{
    QueryFactory GetQueryFactory(string dbName);
}