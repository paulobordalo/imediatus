using MediatR;

namespace imediatus.WebApi.Catalog.Application.Portfolios.Delete.v1;
public sealed record DeletePortfolioCommand(Guid Id) : IRequest;
