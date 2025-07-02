using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddOcelot();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGet("/", () => "MyAuctions API Gateway is running!");

app.UseCors("AllowAll");

await app.UseOcelot(new OcelotPipelineConfiguration
{
    AuthorizationMiddleware = async (context, next) =>
    {
        await next.Invoke();
    }
});

app.Run();