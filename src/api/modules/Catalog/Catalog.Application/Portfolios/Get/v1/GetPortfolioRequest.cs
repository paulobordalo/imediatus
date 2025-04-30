using MediatR;

namespace imediatus.WebApi.Catalog.Application.Portfolios.Get.v1;
public class GetPortfolioRequest : IRequest<PortfolioResponse>
{
    public Guid Id { get; set; }
    public GetPortfolioRequest(Guid id) => Id = id;
}
