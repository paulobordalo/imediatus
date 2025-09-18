using System.Globalization;
using Blazored.LocalStorage;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Blazor.Infrastructure.Auth.Jwt;
using imediatus.Blazor.Infrastructure.Notifications;
using imediatus.Blazor.Infrastructure.Preferences;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.CodeGator.Adapter;
using MudBlazor.Services;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

namespace imediatus.Blazor.Infrastructure;

public static class Extensions
{
    private const string ClientName = "imediatus.API";
    public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration config)
    {
        //Register SyncFusion License
        SyncfusionLicenseProvider.RegisterLicense("MzI1NzE1QDMxMzgyZTMzMmUzMENleXlYTEFVbmRwY05jK0VzT0lhSXkvK2liSWlUcDBXbjFDbkJzYkZ4eFk9"); // 18.3.0.53
        services.AddSyncfusionBlazor();
        services.AddMudServices(configuration =>
        {
            configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            configuration.SnackbarConfiguration.HideTransitionDuration = 100;
            configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
            configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
            configuration.SnackbarConfiguration.ShowCloseIcon = false;
        });
        services.AddMudExObjectEditCGBlazorFormsAdapter();
        services.AddMudExtensions();
        services.AddBlazoredLocalStorage();

        services.AddAuthentication(config);

        // HttpClient nomeado com BaseAddress seguro (fallback local se não houver ApiBaseUrl)
        services.AddHttpClient(ClientName, client =>
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
            var apiBaseUrl = config["ApiBaseUrl"] ?? "https://localhost:7000/";
            client.BaseAddress = new Uri(apiBaseUrl, UriKind.Absolute);
            client.Timeout = TimeSpan.FromHours(2);
        })
        .AddHttpMessageHandler<JwtAuthenticationHeaderHandler>()
        .Services
        .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName));

        // REGISTO CORRETO DO ApiClient (evita resolver string pelo DI)
        services.AddScoped<IApiClient>(sp =>
        {
            var http = sp.GetRequiredService<HttpClient>();
            var cfg = sp.GetRequiredService<IConfiguration>();
            var baseUrl = cfg["ApiBaseUrl"] ?? http.BaseAddress?.ToString() ?? "https://localhost:7000/";
            return new ApiClient(baseUrl, http);
        });

        services.AddTransient<IClientPreferenceManager, ClientPreferenceManager>();
        services.AddTransient<IPreference, ClientPreference>();
        services.AddNotifications();

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 5L * 1024 * 1024 * 1024; // 5 GB
        });

        return services;
    }
}
