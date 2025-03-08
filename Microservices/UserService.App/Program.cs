using UserService.App.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureConfiguration();
builder.ConfigureServices();

var app = builder.Build();
app.ConfigureEndpoints();
app.ApplyDatabaseMigrations();

app.Run();