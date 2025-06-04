using Carter;
using imediatus.Framework.Core.Persistence;
using imediatus.Framework.Infrastructure.Persistence;
using imediatus.WebApi.Workspace.Domain.Models;
using imediatus.WebApi.Workspace.Infrastructure.Endpoints.CostCenters.v1;
using imediatus.WebApi.Workspace.Infrastructure.Endpoints.Portfolios.v1;
using imediatus.WebApi.Workspace.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace imediatus.WebApi.Workspace.Infrastructure;
public static class WorkspaceModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("workspace") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            #region Portfolios
            var portfolioGroup = app.MapGroup("portfolios").WithTags("portfolios");
            portfolioGroup.MapPortfolioCreationEndpoint();
            portfolioGroup.MapGetPortfolioEndpoint();
            portfolioGroup.MapGetPortfolioListEndpoint();
            portfolioGroup.MapPortfolioUpdateEndpoint();
            portfolioGroup.MapPortfolioDeleteEndpoint();
            #endregion

            #region CostCenters
            var costCenterGroup = app.MapGroup("costcenters").WithTags("costcenters");
            costCenterGroup.MapCostCenterCreationEndpoint();
            costCenterGroup.MapGetCostCenterEndpoint();
            costCenterGroup.MapGetCostCenterListEndpoint();
            costCenterGroup.MapCostCenterUpdateEndpoint();
            costCenterGroup.MapCostCenterDeleteEndpoint();
            #endregion
        }
    }
    public static WebApplicationBuilder RegisterWorkspaceServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<WorkspaceDbContext>();
        builder.Services.AddScoped<IDbInitializer, WorkspaceDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Portfolio>, WorkspaceRepository<Portfolio>>("workspace:portfolios");
        builder.Services.AddKeyedScoped<IReadRepository<Portfolio>, WorkspaceRepository<Portfolio>>("workspace:portfolios");
        builder.Services.AddKeyedScoped<IRepository<CostCenter>, WorkspaceRepository<CostCenter>>("workspace:costcenters");
        builder.Services.AddKeyedScoped<IReadRepository<CostCenter>, WorkspaceRepository<CostCenter>>("workspace:costcenters");

        return builder;
    }
    public static WebApplication UseWorkspaceModule(this WebApplication app)
    {
        return app;
    }
}
