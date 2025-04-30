﻿using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace imediatus.Framework.Infrastructure.Logging.Serilog;

public static class Extensions
{
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Host.UseSerilog((context, logger) =>
        {
            logger.WriteTo.OpenTelemetry(options =>
            {
                try
                {
                    options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
                    var headers = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]?.Split(',') ?? [];
                    foreach (var header in headers)
                    {
                        var (key, value) = header.Split('=') switch
                        {
                        [string k, string v] => (k, v),
                            var v => throw new FormatException($"Invalid header format {v}")
                        };

                        options.Headers.Add(key, value);
                    }
                    options.ResourceAttributes.Add("service.name", "apiservice");
                    var (otelResourceAttribute, otelResourceAttributeValue) = builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]?.Split('=') switch
                    {
                    [string k, string v] => (k, v),
                        _ => throw new FormatException($"Invalid header format {builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]}")
                    };
                    options.ResourceAttributes.Add(otelResourceAttribute, otelResourceAttributeValue);
                }
                catch
                {
                    //ignore
                }
            });
            logger.ReadFrom.Configuration(context.Configuration);
            logger.Enrich.FromLogContext();
            logger.Enrich.WithCorrelationId();
            logger
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
                .MinimumLevel.Override("Finbuckle.MultiTenant", LogEventLevel.Warning)
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"));
        });
        return builder;
    }
}
