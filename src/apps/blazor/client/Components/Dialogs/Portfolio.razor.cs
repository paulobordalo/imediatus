using BlazorJS;
using FluentValidation;
using imediatus.Blazor.Client.Components.EntityTable;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Shared.Authorization;
using imediatus.Shared.Enums;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions.Components;
using Newtonsoft.Json;
using static imediatus.Blazor.Client.Pages.Workspace.Kanban;

namespace imediatus.Blazor.Client.Components.Dialogs;

public partial class Portfolio
{
    [Parameter]
    public Guid Id { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Inject]
    protected IApiClient _client { get; set; } = default!;

    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = default!;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    protected IAuthenticationService AuthService { get; set; } = default!;

    private MudForm _form;
    private readonly List<CostCenterResponse> _costCenters = new List<CostCenterResponse>();
    private readonly List<UserDetail> _users = new List<UserDetail>();
    private readonly List<PortfolioPriority> _priorities = [.. PortfolioPriority.List.OrderBy(o => o.Value)];
    private readonly List<PortfolioClassification> _classifications = [.. PortfolioClassification.List.OrderBy(o => o.Value)];
    private bool _saving = false;
    private PortfolioModel _portfolioModel = new();
    private FileModelFluentValidator ValidationRules = new();

    protected override async Task OnInitializedAsync()
    {
        #region CostCenters

        var costCenterFilter = new SearchCostCentersCommand
        {
            PageSize = 50
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(() => _client.SearchCostCentersEndpointAsync("1", costCenterFilter), Toast, Navigation) is CostCenterResponsePagedList response)
        {
            var costCenters = response.Adapt<PaginationResponse<CostCenterResponse>>();

            foreach (var costCenter in costCenters.Items)
            {
                _costCenters.Add(new CostCenterResponse() { Id = costCenter.Id.HasValue ? costCenter.Id.Value : Guid.NewGuid(), Name = costCenter.Name });
            }
        }

        #endregion CostCenters

        #region Utilizadores

        if (await ApiHelper.ExecuteCallGuardedAsync(() => _client.GetUsersListEndpointAsync(), Toast, Navigation) is ICollection<UserDetail> users)
        {
            foreach (var user in users)
            {
                _users.Add(new UserDetail() { Id = user.Id, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, PhoneNumber = user.PhoneNumber, UserName = user.UserName, IsActive = user.IsActive });
            }
        }

        var userLogged = (await AuthState).User;
        if (userLogged.Identity?.IsAuthenticated == true)
        {
            var userIdString = userLogged.GetUserId()?.ToString();
            if (!string.IsNullOrEmpty(userIdString))
            {
                var userDetail = _users.FirstOrDefault(w => w.Id.Equals(new Guid(userIdString)));
                if (userDetail != null)
                {
                    _portfolioModel.Reporter = userDetail;
                    _portfolioModel.Assignee = userDetail;
                }
            }
        }

        #endregion Utilizadores
    }

    private async Task SaveAsync()
    {
        _saving = true;
        await _form.Validate();
        if (_form.IsValid)
        {

            CreatePortfolioCommand request = new CreatePortfolioCommand()
            {
                AssigneeId = _portfolioModel.Assignee.Id,
                ClassificationId = _portfolioModel.Classification.Value,
                CostCenterId = _portfolioModel.CostCenter?.Id,
                PriorityId = _portfolioModel.Priority.Value,
                Summary = _portfolioModel.Summary,
                StatusId = _portfolioModel.Status.Value,
                ReporterId = _portfolioModel.Reporter.Id,
                Attachments = _portfolioModel.Files.Select(file => new UploadBlobCommand
                {
                    FileName = file.FileName,
                    Extension = file.Extension,
                    ContentType = file.ContentType,
                    Data = file.Data,
                    Url = file.Url,
                    Path = file.Path
                }).ToList()
            };

            foreach (var file in request.Attachments)
            {
                Toast.Add("Conteúdo do attachment " + file.FileName, MudBlazor.Severity.Info);
            }

            if (await ApiHelper.ExecuteCallGuardedAsync(() => _client.CreatePortfolioEndpointAsync("1", request), Toast))
            {
                Toast.Add("Portfolio created successfully.", MudBlazor.Severity.Success);
            }

            _saving = false;
            MudDialog.Close(DialogResult.Ok(_portfolioModel));
        }
        else
        {
            _saving = false;
        }
    }

    public class PortfolioModel
    {
        public string Summary { get; set; }
        public PortfolioStatus Status { get; set; } = PortfolioStatus.ToDo;
        public PortfolioPriority Priority { get; set; } = PortfolioPriority.Medium;
        public PortfolioClassification Classification { get; set; } = PortfolioClassification.Public;
        public CostCenterResponse? CostCenter { get; set; }
        public UserDetail Assignee { get; set; }
        public UserDetail Reporter { get; set; }
        public IBrowserFile File { get; set; }
        public IList<UploadableFile> Files { get; set; } = [];
    }

    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    internal class FileModelFluentValidator : AbstractValidator<PortfolioModel>
    {
        public FileModelFluentValidator()
        {
            RuleFor(x => x.Summary)
                .NotEmpty()
                .Length(1, 128);
            RuleFor(x => x.Priority)
            .NotEmpty();
            RuleFor(x => x.Classification)
            .NotEmpty();
            RuleFor(x => x.Assignee)
            .NotEmpty();
            RuleFor(x => x.File)
            .NotEmpty();
            RuleFor(x => x.Files)
                .NotEmpty();
            When(x => x.File != null, () =>
            {
                RuleFor(x => x.File.Size).LessThanOrEqualTo(10485760).WithMessage("The maximum file size is 10 MB");
            });
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<PortfolioModel>.CreateWithOptions((PortfolioModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }


}

