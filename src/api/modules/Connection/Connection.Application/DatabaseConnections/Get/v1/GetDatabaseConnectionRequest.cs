using MediatR;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Get.v1;
public class GetDatabaseConnectionRequest : IRequest<DatabaseConnectionResponse>
{
    public Guid Id { get; set; }
    public GetDatabaseConnectionRequest(Guid id) => Id = id;
}
