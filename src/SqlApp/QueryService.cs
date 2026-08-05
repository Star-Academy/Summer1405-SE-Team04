using System.Data.Common;

internal class QueryService
{
    public static async Task PrintQueryResultAsync(DbDataReader reader)
    {
        var rowNumber = 1;
        var fields = new List<string>();
        for (var i = 0; i < reader.FieldCount; i++) fields.Add(reader.GetName(i));
        Console.WriteLine(string.Join(", ", fields));
        while (await reader.ReadAsync())
            Console.WriteLine($"{rowNumber++}: {string.Join(", ", fields.Select(f => reader[f]))}");
    }
}