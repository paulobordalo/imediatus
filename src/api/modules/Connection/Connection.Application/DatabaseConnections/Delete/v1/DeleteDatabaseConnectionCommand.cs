using MediatR;

namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Delete.v1;
public sealed record DeleteDatabaseConnectionCommand(
    Guid Id) : IRequest;
