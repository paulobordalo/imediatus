using MediatR;

namespace imediatus.Framework.Core.Storage.Azure.Features.SearchBlob;

public sealed record SearchBlobCommand(string Prefix) : IRequest<List<SearchBlobResponse>>;
