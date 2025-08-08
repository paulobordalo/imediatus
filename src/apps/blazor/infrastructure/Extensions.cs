using System.Globalization;
using Blazored.LocalStorage;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Blazor.Infrastructure.Auth.Jwt;
using imediatus.Blazor.Infrastructure.Notifications;
using imediatus.Blazor.Infrastructure.Preferences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.CodeGator.Adapter;
using MudExtensions.Services;
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
        services.AddMudServicesWithExtensions(configuration =>
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
        services.AddTransient<IApiClient, ApiClient>();
        services.AddHttpClient(ClientName, client =>
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
            client.BaseAddress = new Uri(config["ApiBaseUrl"]!);
        })
           .AddHttpMessageHandler<JwtAuthenticationHeaderHandler>()
           .Services
           .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName));
        services.AddTransient<IClientPreferenceManager, ClientPreferenceManager>();
        services.AddTransient<IPreference, ClientPreference>();
        services.AddNotifications();
        return services;

    }
}
