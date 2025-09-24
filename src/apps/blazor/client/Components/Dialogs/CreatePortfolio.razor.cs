using System.Net.Http.Headers;
using FluentValidation;
using imediatus.Blazor.Client.Components.EntityTable;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Blazor.Infrastructure.Auth;
using imediatus.Shared.Enums;
using imediatus.Shared.Extensions;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace imediatus.Blazor.Client.Components.Dialogs;

public partial class CreatePortfolio
{
    [Parameter] 
    public required UserInfo LoggedUser { get; set; }

    [CascadingParameter] 
    private IMudDialogInstance? MudDialog { get; set; }

    [Inject] 
    protected IApiClient Client { get; set; } = default!;

    private IBrowserFile[]? _selectedFiles;

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
        // Users
        if (await ApiHelper.ExecuteCallGuardedAsync(() => Client.GetUsersListEndpointAsync(), Toast, Navigation)
            is ICollection<UserDetail> users)
        {
            _users.AddRange(users);
            SetAssigneeReporter();
        }

        // Cost Centers
        var costCenterFilter = new SearchCostCentersCommand { PageSize = int.MaxValue };
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => Client.SearchCostCentersEndpointAsync("1", costCenterFilter), Toast, Navigation)
            is CostCenterResponsePagedList response)
        {
            var costCenters = response.Adapt<PaginationResponse<CostCenterResponse>>();
            foreach (var cc in costCenters.Items.Where(c => c.Id.HasValue))
                _costCenters.Add(new CostCenterResponse { Id = cc.Id!.Value, Name = cc.Name });
        }
    }

    protected void SetAssigneeReporter()
    {
        if (!_assigneeSet && _users.Count > 0 && LoggedUser is not null)
        {
            if (Guid.TryParse(LoggedUser.UserId, out var uid))
            {
                var me = _users.FirstOrDefault(u => u.Id == uid);
                if (me != null)
                {
                    _portfolioModel.Reporter ??= me;
                    _portfolioModel.Assignee ??= me;
                }
            }
            _assigneeSet = true;
        }
    }

    private async Task SaveAsync()
    {
        _saving = true;
        await _form.Validate();
        if (!_form.IsValid) { _saving = false; return; }

        var request = new CreatePortfolioCommand
        {
            AssigneeId = _portfolioModel.Assignee!.Id,
            ClassificationId = _portfolioModel.Classification.Value,
            CostCenterId = _portfolioModel.CostCenter?.Id,
            PriorityId = _portfolioModel.Priority.Value,
            Summary = _portfolioModel.Summary,
            StatusId = _portfolioModel.Status.Value,
            ReporterId = _portfolioModel.Reporter!.Id,
            Attachments = [.. _portfolioModel.Files.Select(f => new UploadBlobFile
            {
                FileName = f.FileName,
                FileData = f.Data.ToBase64(),
                ContentType = f.ContentType
            })]
        };

        try
        {
            // 1) Cria o portfólio (obter Id)
            var createResponse = await ApiHelper.ExecuteCallGuardedAsync(
                () => Client.CreatePortfolioEndpointAsync("1", request), Toast, Navigation);

            if (createResponse is not CreatePortfolioResponse created || created.Id == Guid.Empty)
            {
                Toast.Add("Failed to create portfolio.", MudBlazor.Severity.Error);
                _saving = false;
                return;
            }

            Toast.Add("Portfolio created successfully.", MudBlazor.Severity.Success);
            MudDialog?.Close(DialogResult.Ok(_portfolioModel));
        }
        catch (ApiException apiEx)
        {
            Toast.Add($"Azure upload failed: {apiEx.Response}", MudBlazor.Severity.Error);
        }
        catch (Exception ex)
        {
            Toast.Add($"Upload error: {ex.Message}", MudBlazor.Severity.Error);
        }
        finally
        {
            _saving = false;
        }
    }

    private readonly Converter<UserDetail> _userConverter = new()
    {
        SetFunc = u => u is null ? string.Empty : $"{u.LastName}, {u.FirstName}"
        // GetFunc não é necessário para MudSelect (seleção é por instância)
    };

    internal class PortfolioModel
    {
        public string Summary { get; set; } = string.Empty;
        public PortfolioStatus Status { get; set; } = PortfolioStatus.ToDo;
        public PortfolioPriority Priority { get; set; } = PortfolioPriority.Medium;
        public PortfolioClassification Classification { get; set; } = PortfolioClassification.Public;
        public CostCenterResponse? CostCenter { get; set; }
        public UserDetail? Assignee { get; set; }
        public UserDetail? Reporter { get; set; }
        public IList<UploadableFile> Files { get; set; } = new List<UploadableFile>();
    }

    internal class FileModelFluentValidator : AbstractValidator<PortfolioModel>
    {
        public FileModelFluentValidator()
        {
            RuleFor(x => x.Summary).NotEmpty().Length(1, 128);
            RuleFor(x => x.Priority).IsInEnum();
            RuleFor(x => x.Classification).IsInEnum();
            RuleFor(x => x.Status).IsInEnum();

            RuleFor(x => x.Assignee)
                .NotNull().WithMessage("Assignee is required.")
                .Must(a => a is not null && a.Id != Guid.Empty)
                .WithMessage("Assignee must be a valid user.");

            RuleFor(x => x.Reporter)
                .NotNull().WithMessage("Reporter is required.")
                .Must(r => r is not null && r.Id != Guid.Empty)
                .WithMessage("Reporter must be a valid user.");

            RuleFor(x => x.Files)
                .NotNull().WithMessage("Please upload at least one file.")
                .Must(files => files is { Count: > 0 })
                .WithMessage("At least one file must be uploaded.");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(
                ValidationContext<PortfolioModel>.CreateWithOptions(
                    (PortfolioModel)model, x => x.IncludeProperties(propertyName)));
            return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
