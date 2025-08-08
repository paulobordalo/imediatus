using FluentValidation;
using imediatus.Blazor.Client.Components.EntityTable;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Shared.Enums;
using imediatus.Shared.Extensions;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudBlazor.Extensions.Components;


namespace imediatus.Blazor.Client.Components.Dialogs;

public partial class CreatePortfolio
{
    [Parameter] 
    public required UserInfo LoggedUser { get; set; }


    [CascadingParameter]
    private IMudDialogInstance? MudDialog { get; set; }

    [Inject]
    protected IApiClient Client { get; set; } = default!;

    private MudForm _form;
    private readonly List<CostCenterResponse> _costCenters = [];
    private readonly List<UserDetail> _users = [];
    private readonly List<PortfolioPriority> _priorities = [.. PortfolioPriority.List.OrderBy(o => o.Value)];
    private readonly List<PortfolioClassification> _classifications = [.. PortfolioClassification.List.OrderBy(o => o.Value)];
    private bool _saving = false;
    private bool _assigneeSet = false;
    private PortfolioModel _portfolioModel = new();
    private FileModelFluentValidator _validationRules = new();

    protected override async Task OnInitializedAsync()
    {
        #region Utilizadores

        if (await ApiHelper.ExecuteCallGuardedAsync(() => Client.GetUsersListEndpointAsync(), Toast, Navigation) is ICollection<UserDetail> users)
        {
            foreach (var user in users)
            {
                _users.Add(new UserDetail() { Id = user.Id, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, PhoneNumber = user.PhoneNumber, UserName = user.UserName, IsActive = user.IsActive });
            }
        }

        #endregion Utilizadores

        #region CostCenters

        var costCenterFilter = new SearchCostCentersCommand
        {
            PageSize = int.MaxValue
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(() => Client.SearchCostCentersEndpointAsync("1", costCenterFilter), Toast, Navigation) is CostCenterResponsePagedList response)
        {
            var costCenters = response.Adapt<PaginationResponse<CostCenterResponse>>();

            foreach (var costCenter in costCenters.Items)
            {
                _costCenters.Add(new CostCenterResponse() { Id = costCenter.Id.HasValue ? costCenter.Id.Value : Guid.NewGuid(), Name = costCenter.Name });
            }
        }

        #endregion CostCenters
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_assigneeSet && _users?.Count > 0 && LoggedUser != null)
        {
            SetReporterToLoggedUser();
            _assigneeSet = true;
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void SetReporterToLoggedUser()
    {
        var selectedUser = _users.FirstOrDefault(u => u.Id == new Guid(LoggedUser.UserId));
        _portfolioModel.Reporter = selectedUser ?? new UserDetail { Id = new Guid(LoggedUser.UserId) };
    }

    private async Task SaveAsync()
    {
        _saving = true;
        await _form.Validate();
        if (_form.IsValid)
        {
            CreatePortfolioCommand request = new()
            {
                AssigneeId = _portfolioModel.Assignee.Id,
                ClassificationId = _portfolioModel.Classification.Value,
                CostCenterId = _portfolioModel.CostCenter?.Id,
                PriorityId = _portfolioModel.Priority.Value,
                Summary = _portfolioModel.Summary,
                StatusId = _portfolioModel.Status.Value,
                ReporterId = _portfolioModel.Reporter.Id,
                Attachments = [.. _portfolioModel.Files.Select(file => new PortfolioAttachment
                {
                    FileName = file.FileName,
                    Base64Content = file.Data.ToBase64(),
                    ContentType = file.ContentType
                })]
            };

            if (MudDialog != null && await ApiHelper.ExecuteCallGuardedAsync(() => Client.CreatePortfolioEndpointAsync("1", request), Toast))
            {
                Toast.Add("Portfolio created successfully.", MudBlazor.Severity.Success);
                MudDialog.Close(DialogResult.Ok(_portfolioModel));
            }
            else
            { 
                Toast.Add("Failed to create portfolio.", MudBlazor.Severity.Error);
            }
        }

        _saving = false;
    }

    internal class PortfolioModel
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
            .NotEmpty()
            .Must(assignee => assignee.Id != Guid.Empty).WithMessage("Assignee must be a valid user.");
            RuleFor(x => x.File)
            .NotEmpty();
            RuleFor(x => x.Files)
                .NotEmpty()
                .Must(files => files.Count > 0).WithMessage("At least one file must be uploaded.");
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

