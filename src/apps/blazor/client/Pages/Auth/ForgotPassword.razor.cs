using imediatus.Blazor.Client.Components;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Shared.Authorization;
using Microsoft.AspNetCore.Components;

namespace imediatus.Blazor.Client.Pages.Auth;

public partial class ForgotPassword
{
    private readonly ForgotPasswordCommand _forgotPasswordRequest = new();
    private ImediatusValidation? _customValidation;
    private bool BusySubmitting { get; set; }

    [Inject]
    private IApiClient UsersClient { get; set; } = default!;

    private string Tenant { get; set; } = TenantConstants.Root.Id;

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.ForgotPasswordEndpointAsync(Tenant, _forgotPasswordRequest),
            Toast,
            _customValidation);

        BusySubmitting = false;
    }
}
