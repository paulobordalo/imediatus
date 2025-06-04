using imediatus.Blazor.Client.Components.EntityTable;
using imediatus.Blazor.Infrastructure.Api;
using imediatus.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace imediatus.Blazor.Client.Pages.Orchestrator;

public partial class Domains
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<BrandResponse, Guid, BrandViewModel> Context { get; set; } = default!;

    private EntityTable<BrandResponse, Guid, BrandViewModel> _table = default!;

    protected override void OnInitialized()
    {
        Context = new(
            entityName: "Domain",
            entityNamePlural: "Domains",
            entityResource: ImediatusResources.Domains,
            fields:
            [
                new(field => field.Id, "Id", "Id"),
                new(field => field.Name, "Name", "Name"),
                new(field => field.Description, "Description", "Description")
            ],
            enableAdvancedSearch: true,
            idFunc: field => field.Id!.Value,
            searchFunc: async filter =>
            {
                SearchBrandsCommand searchFilter = filter.Adapt<SearchBrandsCommand>();
                BrandResponsePagedList result = await _client.SearchBrandsEndpointAsync("1", searchFilter);
                return result.Adapt<PaginationResponse<BrandResponse>>();
            },
            createFunc: async entity => _ = await _client.CreateBrandEndpointAsync("1", entity.Adapt<CreateBrandCommand>()),
            updateFunc: async (id, entity) => _ = await _client.UpdateBrandEndpointAsync("1", id, entity.Adapt<UpdateBrandCommand>()),
            deleteFunc: async id => await _client.DeleteBrandEndpointAsync("1", id));
    }
}

public class BrandViewModel : UpdateBrandCommand
{
}
