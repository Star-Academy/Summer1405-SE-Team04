using System.ComponentModel.DataAnnotations;
using DotNetEnv;
using Npgsql;
using QueryBuilder.Compilers;
using QueryBuilder.Factory;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;
using QueryBuilder.Utils;
using SqlApp;
using SqlApp.QueryExecutor;
using SqlApp.QueryService;

var query = new Query()
    .From("Student")
    .Select("StudentNumber", "FirstName")
    .Where("IsMale", true);
var config = new Config();

var pgCompiler = new Compiler(PostgresParameterFixer.Instance, new PostgresClauseCompilerFactory(), new SqlValidator());
var executator = new QueryExecutor(NpgsqlFactory.Instance, pgCompiler, config.BuildPostgresConnectionString());

var queryService = new QueryService();
var reader = await executator.ExecuteQuery(query);
await queryService.PrintQueryResultAsync(reader);