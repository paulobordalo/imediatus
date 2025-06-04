using System.ComponentModel;
using MediatR;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Create.v1;
public sealed record CreateDatabaseConnectionCommand(
    [property: DefaultValue("Connection name")] string ApplicationKey,
    [property: DefaultValue("Server name")] string Server,
    [property: DefaultValue("Database name")] string DatabaseName,
    [property: DefaultValue("Server Port")] int Port,
    [property: DefaultValue("Authentication username")] string Username,
    [property: DefaultValue("Authentication password")] string Password,
    [property: DefaultValue("Integrated Security")] bool UseIntegratedSecurity) : IRequest<CreateDatabaseConnectionResponse>;

