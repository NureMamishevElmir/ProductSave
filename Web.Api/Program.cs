using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using Web.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var logger = LogManager.Setup().LoadConfigurationFromSection(builder.Configuration).GetCurrentClassLogger();

try
{
    var env = builder.Environment;

    builder.ConfigureServices();

    var app = builder.Build();
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
