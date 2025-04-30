using System.Collections.ObjectModel;
using System.Net;

namespace imediatus.Framework.Core.Exceptions;
public class NotFoundException : ImediatusException
{
    public NotFoundException(string message)
        : base(message, new Collection<string>(), HttpStatusCode.NotFound)
    {
    }
}
