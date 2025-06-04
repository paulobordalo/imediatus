namespace imediatus.WebApi.Connection.Application.DatabaseConnections.Get.v1;
public sealed record DatabaseConnectionResponse(Guid? Id, string applicationKey, string server, string databaseName, int port, string username, string password, bool useIntegratedSecurity);
