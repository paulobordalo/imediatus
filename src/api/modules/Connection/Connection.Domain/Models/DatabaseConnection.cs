using imediatus.Framework.Core.Domain;
using imediatus.Framework.Core.Domain.Contracts;
using imediatus.WebApi.Connection.Domain.Events.DatabaseConnections;

namespace imediatus.WebApi.Connection.Domain.Models;

public class DatabaseConnection : AuditableEntity, IAggregateRoot
{
    public string ApplicationKey { get; private set; } = string.Empty;
    public string Server { get; private set; } = string.Empty;
    public string DatabaseName { get; private set; } = string.Empty;
    public int Port { get; private set; } = 1433;
    public string Username { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public bool UseIntegratedSecurity { get; private set; } = true;

    private DatabaseConnection() { }

    private DatabaseConnection(Guid id, string applicationKey, string server, string databaseName, int port, string username, string password, bool useIntegratedSecurity)
    {
        Id = id;
        ApplicationKey = applicationKey;
        Server = server;
        DatabaseName = databaseName;
        Port = port;
        Username = username;
        Password = password;
        UseIntegratedSecurity = useIntegratedSecurity;
        QueueDomainEvent(new DatabaseConnectionCreated { DatabaseConnection = this });
    }

    public static DatabaseConnection Create(string applicationKey, string server, string databaseName, int port, string username, string password, bool useIntegratedSecurity)
    {
        return new DatabaseConnection(Guid.NewGuid(), applicationKey, server, databaseName, port, username, password, useIntegratedSecurity);
    }

    public DatabaseConnection Update(string applicationKey, string server, string databaseName, int port, string username, string password, bool useIntegratedSecurity)
    {
        bool isUpdated = false;

        if (!string.IsNullOrWhiteSpace(applicationKey) && !string.Equals(ApplicationKey, applicationKey, StringComparison.OrdinalIgnoreCase))
        {
            ApplicationKey = applicationKey;
            isUpdated = true;
        }

        if (!string.IsNullOrWhiteSpace(server) && !string.Equals(Server, server, StringComparison.OrdinalIgnoreCase))
        {
            Server = server;
            isUpdated = true;
        }

        if (!string.IsNullOrWhiteSpace(databaseName) && !string.Equals(DatabaseName, databaseName, StringComparison.OrdinalIgnoreCase))
        {
            DatabaseName = databaseName;
            isUpdated = true;
        }

        if (!int.Equals(Port, port))
        {
            DatabaseName = databaseName;
            isUpdated = true;
        }

        if (!string.IsNullOrWhiteSpace(username) && !string.Equals(Username, username, StringComparison.OrdinalIgnoreCase))
        {
            Username = username;
            isUpdated = true;
        }

        if (!string.IsNullOrWhiteSpace(password) && !string.Equals(Password, password, StringComparison.OrdinalIgnoreCase))
        {
            Password = password;
            isUpdated = true;
        }

        if (!bool.Equals(UseIntegratedSecurity, useIntegratedSecurity))
        {
            UseIntegratedSecurity = useIntegratedSecurity;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new DatabaseConnectionUpdated { DatabaseConnection = this });
        }

        return this;
    }
}
