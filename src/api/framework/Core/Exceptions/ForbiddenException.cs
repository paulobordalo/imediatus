using System.Collections.ObjectModel;
using System.Net;

namespace imediatus.Framework.Core.Exceptions;
public class ForbiddenException : ImediatusException
{
    public ForbiddenException()
        : base("unauthorized", [], HttpStatusCode.Forbidden)
    {
    }
    public ForbiddenException(string message)
       : base(message, [], HttpStatusCode.Forbidden)
    {
    }
}
