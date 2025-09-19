using System.Reflection;
using Asp.Versioning.Conventions;
using FluentValidation;
using imediatus.Framework.Core;
using imediatus.Framework.Core.Origin;
using imediatus.Framework.Infrastructure.Auth;
using imediatus.Framework.Infrastructure.Auth.Jwt;
using imediatus.Framework.Infrastructure.Behaviours;
using imediatus.Framework.Infrastructure.Caching;
using imediatus.Framework.Infrastructure.Cors;
using imediatus.Framework.Infrastructure.Exceptions;
using imediatus.Framework.Infrastructure.Identity;
using imediatus.Framework.Infrastructure.Jobs;
using imediatus.Framework.Infrastructure.Logging.Serilog;
using imediatus.Framework.Infrastructure.Mail;
using imediatus.Framework.Infrastructure.OpenApi;
using imediatus.Framework.Infrastructure.Persistence;
using imediatus.Framework.Infrastructure.RateLimit;
using imediatus.Framework.Infrastructure.SecurityHeaders;
using imediatus.Framework.Infrastructure.Storage.Files;
using imediatus.Framework.Infrastructure.Storage.Azure;
using imediatus.Framework.Infrastructure.Tenant;
using imediatus.Framework.Infrastructure.Tenant.Endpoints;
using imediatus.Aspire.ServiceDefaults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace imediatus.Framework.Infrastructure;

public static class Extensions
{
    public static WebApplicationBuilder ConfigureImediatusFramework(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.AddServiceDefaults();
        builder.ConfigureSerilog();
        builder.ConfigureDatabase();
        builder.Services.ConfigureMultitenancy();
        builder.Services.ConfigureIdentity();
        builder.Services.AddCorsPolicy(builder.Configuration);
        builder.Services.ConfigureAzureStorage(builder.Configuration);
        builder.Services.ConfigureFileStorage();
        builder.Services.ConfigureJwtAuth();
        builder.Services.ConfigureOpenApi();
        builder.Services.ConfigureJobs(builder.Configuration);
        builder.Services.ConfigureMailing();
        builder.Services.ConfigureCaching(builder.Configuration);
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddHealthChecks();
        builder.Services.AddOptions<OriginOptions>().BindConfiguration(nameof(OriginOptions));

        // Define module assemblies
        var assemblies = new Assembly[]
        {
            typeof(ImediatusCore).Assembly,
            typeof(ImediatusInfrastructure).Assembly
        };

        // Register validators
        builder.Services.AddValidatorsFromAssemblies(assemblies);

        // Register MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzg1NTQyNDAwIiwiaWF0IjoiMTc1NDA2MTk3NCIsImFjY291bnRfaWQiOiIwMTk4NjYzZDM0NTk3ZDU1ODc2NzgyODgwMTViZDE0NSIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazFrM3ZiNjFyZzFwbjMxZXhoM2twYmc0Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.m9zjJWHp1z_5eQ05X5iun_YnwtVMmE-7LY8giFfGmk8hQplpHln0itSSUv9Nmxyd5pksRwrWLzLXxSxIefG7ROybhrmKjYddrB_sai0velmYnbxnVhO2f6GBuKxQbJAmMLD0s8Pzjs0JKgdANhpONj2zOSm24eb1OttMnu5KLhZH7iNfow3DL0wNzwrQoLsrQ9Nh0DE8WZ-XeIaPeTAnLdBwCCTWOpvQRAKoH5QFd2pQeWiInh26c0kh02ostyFv_OorkkOZkVckhMqhUoZ6Lgr4a1-EyVGeNgEXsR6k9LVOo3CcCSEvl0FJ4it8OlzvYv0JTEqchHOIJoCXDXWTtA";
        });

        builder.Services.ConfigureRateLimit(builder.Configuration);
        builder.Services.ConfigureSecurityHeaders(builder.Configuration);

        return builder;
    }

    public static WebApplication UseImediatusFramework(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.UseRateLimit();
        app.UseSecurityHeaders();
        app.UseMultitenancy();
        app.UseExceptionHandler();
        app.UseCorsPolicy();
        app.UseOpenApi();
        app.UseJobDashboard(app.Configuration);
        app.UseRouting();
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "assets")),
            RequestPath = new PathString("/assets")
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapTenantEndpoints();
        app.MapIdentityEndpoints();

        // Current user middleware
        app.UseMiddleware<CurrentUserMiddleware>();

        // Register API versions
        var versions = app.NewApiVersionSet()
                    .HasApiVersion(1)
                    .HasApiVersion(2)
                    .ReportApiVersions()
                    .Build();

        // Map versioned endpoint root
        app.MapGroup("api/v{version:apiVersion}").WithApiVersionSet(versions);

        // Storage minimal APIs
        app.MapAzureStorageEndpoints();

        return app;
    }
}
