using System.Collections.ObjectModel;
using System.Net;

namespace imediatus.Framework.Core.Exceptions;
public class UnauthorizedException : ImediatusException
{
    public UnauthorizedException()
        : base("authentication failed", new Collection<string>(), HttpStatusCode.Unauthorized)
    {
    }
    public UnauthorizedException(string message)
       : base(message, new Collection<string>(), HttpStatusCode.Unauthorized)
    {
    }
}
