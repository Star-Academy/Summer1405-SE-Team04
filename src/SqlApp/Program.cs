using DotNetEnv;
using QueryBuilder;

Env.TraversePath().Load();

var query = new Query()
    .From("Student")
    .Select("StudentNumber", "FirstName")
    .Where("IsMale", true);