﻿namespace imediatus.Framework.Core.Domain.Contracts;

public interface ISoftDeletable
{
    DateTimeOffset? Deleted { get; set; }
    Guid? DeletedBy { get; set; }
}
