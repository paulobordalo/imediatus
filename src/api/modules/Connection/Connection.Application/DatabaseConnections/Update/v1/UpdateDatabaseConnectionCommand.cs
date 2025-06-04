using MediatR;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Update.v1;
public sealed record UpdateDatabaseConnectionCommand(
    Guid Id,
    string ApplicationKey,
    string Server,
    string DatabaseName,
    int Port,
    string Username,
    string Password,
    bool UseIntegratedSecurity) : IRequest<UpdateDatabaseConnectionResponse>;
