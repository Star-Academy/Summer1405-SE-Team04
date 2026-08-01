using Npgsql;
using QueryBuilder;
using Microsoft.Data.SqlClient;
using System.Data;
using DotNetEnv;

Env.TraversePath().Load();

string pgHost = Environment.GetEnvironmentVariable("PG_HOST") ?? "localhost";
string pgPort = Environment.GetEnvironmentVariable("PG_PORT") ?? "5432";
string pgDb = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "team04";
string pgUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "mohaymen";
string pgPass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? string.Empty;

string msServer = Environment.GetEnvironmentVariable("MS_SERVER") ?? "localhost,1433";
string msDb = Environment.GetEnvironmentVariable("MS_DATABASE") ?? "mohaymen-sqlserver";
string msPass = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD") ?? string.Empty;


string pgConnStr = $"Host={pgHost};Port={pgPort};Database={pgDb};Username={pgUser};Password={pgPass};";
string msConnStr = @$"Server={msServer};Database={msDb};User Id=sa;Password={msPass};Connection Timeout=30;TrustServerCertificate=True;";

var query = new Query()
    .From("Student")
    .Select("StudentNumber", "FirstName")
    .Where("IsMale", true);


// Compile for PostgreSQL
var pgCompiler = new PostgresCompiler();
var pgResult = pgCompiler.Compile(query);

// Compile for SQL Server
var msCompiler = new SqlServerCompiler();
var msResult = msCompiler.Compile(query);


await using var dataSource = NpgsqlDataSource.Create(pgConnStr);

await using (var cmd = dataSource.CreateCommand(pgResult.Sql.ToLower()))
{  
    foreach(var binding in pgResult.Bindings)
    {
        cmd.Parameters.AddWithValue(binding);
    }

    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            Console.WriteLine($"STID: {reader["studentnumber"]}, firstname: {reader["firstname"]}");
        }
    }
}

using (var connection = new SqlConnection(msConnStr))  
			{  
				connection.Open();  
				Console.WriteLine("Connected successfully.");  
                using (var command = new SqlCommand())  
			{  
				command.Connection = connection;  
				command.CommandType = CommandType.Text;  
				command.CommandText = msResult.Sql;

                for(int i=0; i<msResult.Bindings.Count; i++)
                {
                    var parameter = new SqlParameter($"@p{i}", SqlDbType.NVarChar, 50);  
                    parameter.Value = msResult.Bindings[i];  
                    command.Parameters.Add(parameter);  
                }
  
				SqlDataReader reader = command.ExecuteReader();  
  
				while (await reader.ReadAsync())
                {
                    Console.WriteLine($"STID: {reader["studentnumber"]}, firstname: {reader["firstname"]}");
                }
			} 
				Console.WriteLine("Press any key to finish...");  
				Console.ReadKey(true);  
			}  