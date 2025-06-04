using imediatus.Framework.Core.Exceptions;

namespace imediatus.WebApi.Connection.Domain.Exceptions;

public sealed class DatabaseConnectionNotFoundException : NotFoundException
{
    public DatabaseConnectionNotFoundException(Guid id)
        : base($"database connection with id {id} not found")
    {
    }
}
