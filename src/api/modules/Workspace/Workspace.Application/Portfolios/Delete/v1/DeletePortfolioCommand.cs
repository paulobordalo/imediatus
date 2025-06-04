using MediatR;

namespace imediatus.WebApi.Workspace.Application.Portfolios.Delete.v1;
public sealed record DeletePortfolioCommand(Guid Id) : IRequest;
