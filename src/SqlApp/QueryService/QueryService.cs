using System.Data.Common;

namespace SqlApp.QueryService;

internal class QueryService : IQueryService
{
    public async Task PrintQueryResultAsync(DbDataReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        var rowNumber = 1;
        var fields = new List<string>();
        for (var i = 0; i < reader.FieldCount; i++) fields.Add(reader.GetName(i));
        Console.WriteLine(string.Join(", ", fields));
        while (await reader.ReadAsync())
            Console.WriteLine($"{rowNumber++}: {string.Join(", ", fields.Select(f => reader[f]))}");
    }
}