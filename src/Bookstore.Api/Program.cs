using System.Diagnostics;
using Bookstore.Api.Configuration;
using Bookstore.Api.Endpoints;
using Serilog;
using Serilog.Context;
using Serilog.Formatting.Compact;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Instance = $"urn:trace:{traceId}";
    };
});

builder.Services.AddOpenApi();

builder.Services.AddSingleton(new MongoDbOptions(
    builder.Configuration["MONGODB_URI"] ?? builder.Configuration.GetConnectionString("MongoDb")));

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(new CompactJsonFormatter());
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.Use(async (context, next) =>
{
    var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

    context.Response.Headers["X-Trace-Id"] = traceId;

    using (LogContext.PushProperty("TraceId", traceId))
    {
        await next();
    }
});

app.MapOpenApi("/openapi/v1.json");

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app.MapBookRoutes();

app.Run();

public partial class Program;
