using imediatus.Framework.Infrastructure;
using imediatus.Framework.Infrastructure.Logging.Serilog;
using imediatus.WebApi.Host;
using Serilog;

StaticLogger.EnsureInitialized();
Log.Information("server booting up..");
try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    _ = builder.ConfigureImediatusFramework();
    _ = builder.RegisterModules();

    WebApplication app = builder.Build();

    _ = app.UseImediatusFramework();
    _ = app.UseModules();
    await app.RunAsync();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex.Message, "unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("server shutting down..");
    await Log.CloseAndFlushAsync();
}
