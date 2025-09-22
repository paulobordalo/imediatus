using Asp.Versioning;
using imediatus.Framework.Core.Storage.Azure;
using imediatus.Framework.Core.Storage.Azure.Features.DownloadBlob;
using imediatus.Framework.Core.Storage.Azure.Features.SearchBlob;
using imediatus.Framework.Core.Storage.Azure.Features.UploadBlob;
using imediatus.Framework.Core.Storage.Azure.Features.RenameBlob;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace imediatus.Framework.Infrastructure.Storage.Azure;

public static class AzureStorageEndpoints
{
    public static IEndpointRouteBuilder MapAzureStorageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("api/v{version:apiVersion}/storage")
            .WithTags("Storage")
            .WithOpenApi()
            .HasApiVersion(1);

        // Ensure container (tenant)
        group.MapPost("container/ensure", async (IStorageAzureService storage, CancellationToken ct) =>
        {
            await storage.EnsureTenantContainerAsync(ct);
            return Results.NoContent();
        })
        .WithName("EnsureContainerEndpoint")
        .Produces(StatusCodes.Status204NoContent);

        // Ensure folder (PortfolioId como folderName)
        group.MapPost("folders/{folderName}/ensure", async (string folderName, IStorageAzureService storage, CancellationToken ct) =>
        {
            await storage.EnsureFolderAsync(folderName, ct);
            return Results.NoContent();
        })
        .WithName("EnsureFolderEndpoint")
        .Produces(StatusCodes.Status204NoContent);

        // Upload blobs
        group.MapPost("blobs/upload", async (UploadBlobCommand request, IStorageAzureService storage, CancellationToken ct) =>
        {
            var result = await storage.UploadBlobsAsync(request, ct);
            return Results.Ok(result);
        })
        .WithName("UploadBlobsEndpoint")
        .Accepts<UploadBlobCommand>("application/json")
        .Produces<UploadBlobResponse>(StatusCodes.Status200OK);

        // Search blobs by prefix
        group.MapGet("blobs/search", async (string prefix, IStorageAzureService storage, CancellationToken ct) =>
        {
            var result = await storage.SearchBlobsAsync(new SearchBlobCommand(prefix), ct);
            return Results.Ok(result);
        })
        .WithName("SearchBlobsEndpoint")
        .Produces<List<SearchBlobResponse>>(StatusCodes.Status200OK);

        // Download blob
        group.MapGet("blobs/{folderName}/{fileName}", async (string folderName, string fileName, IStorageAzureService storage, CancellationToken ct) =>
        {
            var result = await storage.DownloadBlobAsync(new DownloadBlobCommand(folderName, fileName), ct);
            return Results.Ok(result);
        })
        .WithName("DownloadBlobEndpoint")
        .Produces<DownloadBlobResponse>(StatusCodes.Status200OK);

        // Delete folder
        group.MapDelete("folders/{folderName}", async (string folderName, IStorageAzureService storage, CancellationToken ct) =>
        {
            var ok = await storage.DeleteFolderAsync(folderName, ct);
            return Results.Ok(ok);
        })
        .WithName("DeleteFolderEndpoint")
        .Produces<bool>(StatusCodes.Status200OK);

        // Delete blob
        group.MapDelete("blobs/{folderName}/{blobName}", async (string folderName, string blobName, IStorageAzureService storage, CancellationToken ct) =>
        {
            var ok = await storage.DeleteBlobAsync(folderName, blobName, ct);
            return Results.Ok(ok);
        })
        .WithName("DeleteBlobEndpoint")
        .Produces<bool>(StatusCodes.Status200OK);

        // Rename blob
        group.MapPost("blobs/rename", async (RenameBlobRequest request, IStorageAzureService storage, CancellationToken ct) =>
        {
            var ok = await storage.RenameBlobAsync(request.FolderName, request.CurrentName, request.NewName, request.Overwrite, ct);
            return Results.Ok(ok);
        })
        .WithName("RenameBlobEndpoint")
        .Accepts<RenameBlobRequest>("application/json")
        .Produces<bool>(StatusCodes.Status200OK);

        // List folder contents
        group.MapGet("folders/{folderName}/list", async (string folderName, IStorageAzureService storage, CancellationToken ct) =>
        {
            var result = await storage.ListFolderContentsAsync(folderName, ct);
            return Results.Ok(result);
        })
        .WithName("ListFolderContentsEndpoint")
        .Produces<List<SearchBlobResponse>>(StatusCodes.Status200OK);

        return app;
    }
}
