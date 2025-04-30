namespace imediatus.Framework.Core.Domain.Contracts;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class IgnoreAuditTrailAttribute : Attribute
{
}
