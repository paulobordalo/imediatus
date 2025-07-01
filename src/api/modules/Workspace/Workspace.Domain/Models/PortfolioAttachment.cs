namespace imediatus.WebApi.Workspace.Domain.Models;

public sealed record PortfolioAttachment(string FileName, string Base64Content, string? ContentType);
