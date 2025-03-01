using AuthService.App.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureConfiguration();
builder.ConfigureServices();

var app = builder.Build();
app.ConfigureEndpoints();
app.ApplyDatabaseMigrations();
app.ApplyRoleInitialization();
app.ApplyKafkaTopicCreation();

app.Run();