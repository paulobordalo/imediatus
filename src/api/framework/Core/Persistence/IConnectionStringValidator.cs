﻿namespace imediatus.Framework.Core.Persistence;
public interface IConnectionStringValidator
{
    bool TryValidate(string connectionString, string? dbProvider = null);
}
