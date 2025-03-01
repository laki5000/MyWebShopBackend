using ApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureConfiguration();
builder.ConfigureServices();

var app = builder.Build();
app.ConfigureMiddleware();
app.ConfigureEndpoints();

app.Run();


