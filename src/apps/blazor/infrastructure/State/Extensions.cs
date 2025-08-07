using Microsoft.Extensions.DependencyInjection;

namespace imediatus.Blazor.Infrastructure.State;

public static class Extensions
{
    public static IServiceCollection AddStates(this IServiceCollection services)
    {
        services.AddScoped<AppState>(); // Mantém durante a sessão do usuário
        return services;
    }
}
