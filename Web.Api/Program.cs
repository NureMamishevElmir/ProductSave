using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using Service;
using Web.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Host.UseNLog();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

var logger = LogManager.Setup().LoadConfigurationFromSection(builder.Configuration).GetCurrentClassLogger();

try
{
    var env = builder.Environment;

    builder.ConfigureServices();

    builder.Services.AddScoped<JwtTokenService>();
    builder.Services.AddScoped<TemperatureValidationService>();

    var app = builder.Build();

    // Apply EF Core migrations on startup when explicitly enabled (e.g. in the
    // Kubernetes cluster, where no developer runs `dotnet ef database update`).
    if (builder.Configuration.GetValue<bool>("RunMigrations"))
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Infrastructure.AppDbContext>();
        db.Database.Migrate();
    }

    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var logger = context.RequestServices
                .GetRequiredService<ILogger<Program>>();

            var exception = context.Features
                ?.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()
                ?.Error;

            if (exception != null)
            {
                logger.LogError(exception, "UNHANDLED EXCEPTION");
            }

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal Server Error");
        });
    });

    app.ConfigurePipeline();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of an exception");
    throw;
}
finally
{
    logger.Info("App finished.");
    LogManager.Flush();
    LogManager.Shutdown();
}
