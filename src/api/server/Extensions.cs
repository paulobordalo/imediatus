using System.Reflection;
using Asp.Versioning.Conventions;
using Carter;
using FluentValidation;
using imediatus.Framework.Infrastructure.Behaviours;
using imediatus.WebApi.Catalog.Application;
using imediatus.WebApi.Catalog.Infrastructure;
using imediatus.WebApi.Todo;
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
            typeof(TodoModule).Assembly
        ];

        //register validators
        _ = builder.Services.AddValidatorsFromAssemblies(assemblies);

        //register mediatr
        _ = builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        //register module services
        _ = builder.RegisterCatalogServices();
        _ = builder.RegisterTodoServices();

        //add carter endpoint modules
        _ = builder.Services.AddCarter(configurator: config =>
        {
            config.WithModule<CatalogModule.Endpoints>();
            config.WithModule<TodoModule.Endpoints>();
        });

        return builder;
    }

    public static WebApplication UseModules(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        //register modules
        _ = app.UseCatalogModule();
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
