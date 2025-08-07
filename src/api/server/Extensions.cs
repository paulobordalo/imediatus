using System.Reflection;
using Asp.Versioning.Conventions;
using Carter;
using FluentValidation;
using imediatus.Framework.Infrastructure.Behaviours;
using imediatus.WebApi.Catalog.Application;
using imediatus.WebApi.Catalog.Infrastructure;
using imediatus.WebApi.Todo;
using imediatus.WebApi.Workspace.Application;
using imediatus.WebApi.Workspace.Infrastructure;
using MediatR;

namespace imediatus.WebApi.Host;

public static class Extensions
{
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        //define module assemblies
        Assembly[] assemblies =
        [
            typeof(CatalogMetadata).Assembly,
            typeof(WorkspaceMetadata).Assembly,
            typeof(TodoModule).Assembly
        ];

        //register validators
        _ = builder.Services.AddValidatorsFromAssemblies(assemblies);

        //register mediatr
        _ = builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzg1NTQyNDAwIiwiaWF0IjoiMTc1NDA2MTk3NCIsImFjY291bnRfaWQiOiIwMTk4NjYzZDM0NTk3ZDU1ODc2NzgyODgwMTViZDE0NSIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazFrM3ZiNjFyZzFwbjMxZXhoM2twYmc0Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.m9zjJWHp1z_5eQ05X5iun_YnwtVMmE-7LY8giFfGmk8hQplpHln0itSSUv9Nmxyd5pksRwrWLzLXxSxIefG7ROybhrmKjYddrB_sai0velmYnbxnVhO2f6GBuKxQbJAmMLD0s8Pzjs0JKgdANhpONj2zOSm24eb1OttMnu5KLhZH7iNfow3DL0wNzwrQoLsrQ9Nh0DE8WZ-XeIaPeTAnLdBwCCTWOpvQRAKoH5QFd2pQeWiInh26c0kh02ostyFv_OorkkOZkVckhMqhUoZ6Lgr4a1-EyVGeNgEXsR6k9LVOo3CcCSEvl0FJ4it8OlzvYv0JTEqchHOIJoCXDXWTtA";
        });

        //register module services
        _ = builder.RegisterCatalogServices();
        _ = builder.RegisterWorkspaceServices();
        _ = builder.RegisterTodoServices();

        //add carter endpoint modules
        _ = builder.Services.AddCarter(configurator: config =>
        {
            config.WithModule<CatalogModule.Endpoints>();
            config.WithModule<WorkspaceModule.Endpoints>();
            config.WithModule<TodoModule.Endpoints>();
        });

        return builder;
    }

    public static WebApplication UseModules(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        //register modules
        _ = app.UseCatalogModule();
        _ = app.UseWorkspaceModule();
        _ = app.UseTodoModule();

        //register api versions
        Asp.Versioning.Builder.ApiVersionSet versions = app.NewApiVersionSet()
                    .HasApiVersion(1)
                    .HasApiVersion(2)
                    .ReportApiVersions()
                    .Build();

        //map versioned endpoint
        RouteGroupBuilder endpoints = app.MapGroup("api/v{version:apiVersion}").WithApiVersionSet(versions);

        //use carter
        _ = endpoints.MapCarter();

        return app;
    }
}
