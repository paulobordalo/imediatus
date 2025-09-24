# FileManagerMud (Blazor WASM + MudBlazor)

A production-ready file manager component for MudBlazor with Azure Blob Storage integration via your existing `IApiClient`.

## Highlights
- **Toolbar**: New Folder, Upload, Refresh, Sort By
- **UX**: Breadcrumbs, search box, Grid/List toggle
- **Listing**: Name, Size, Type, Modified, inline Actions
- **Context Menu**: Open, Details, Rename, Delete, Copy, Cut, Paste
- **Selection**: Multi-select with checkbox
- **Preview**: Images, PDF, Text, JSON, XML, Markdown
- **Uploads**: Multiple files, progress, per-file cancel, cancel-all (JSON Base64)
- **Perf**: Client-side search/sort, virtual-friendly layout
- **A11y**: ARIA roles, keyboard friendly
- **i18n**: English by default; override via `TFunc`
- **API**: Uses `EnsureContainer`, `EnsureFolder`, `ListFolderContents`, `UploadBlobs`, `DownloadBlob`, `RenameBlob`, `DeleteBlob`, `DeleteFolder` from `IApiClient`

## Install
Add files to your Blazor WASM project:
Components/FileManagerMud/FileManagerMud.razor
Components/FileManagerMud/FileManagerMudBase.cs
Components/FileManagerMud/FileManagerMudService.cs
Components/FileManagerMud/IFileManagerService.cs
Components/FileManagerMud/Models/FileManagerItem.cs
wwwroot/js/fileManagerMud.js

css
Copiar código

Register the service:
```csharp
builder.Services.AddScoped<IFileManagerService, FileManagerMudService>();
Include JS in wwwroot/index.html:

html
Copiar código
<script src="js/fileManagerMud.js"></script>
Usage
razor
Copiar código
<FileManagerMud RootPrefix="projects/demo" PageSize="100"
                OnFileOpened="@(f => Console.WriteLine($"Opened {f.Name}"))" />
Notes
Uploads are sent as JSON Base64 to UploadBlobsEndpointAsync, matching NSwag-generated DTOs.

If your backend exposes SAS URLs, you can tweak TryGetSasUrlAsync for streaming previews.

All UI strings are in English by design. Provide a TFunc for custom localization if needed.

csharp
Copiar código

---

# 8) **Testes** — `tests/FileManagerMud.Tests/FileManagerMudRenderTests.cs` (bUnit)

```csharp
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using imediatus.Blazor.Client.Components.FileManagerMud;
using imediatus.Blazor.Infrastructure.Api;
using NSubstitute;

namespace FileManagerMud.Tests;

public class FileManagerMudRenderTests : TestContext
{
    public FileManagerMudRenderTests()
    {
        Services.AddMudServices();
        var api = Substitute.For<IApiClient>();
        var svc = Substitute.For<IFileManagerService>();

        // Basic stubs
        svc.ListAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<FileManagerItem>>(new[]
            {
                new FileManagerItem{ Name="docs", IsFolder=true, ParentPath="" },
                new FileManagerItem{ Name="readme.pdf", ContentType="application/pdf", Size=1024, ParentPath="" }
            }));

        Services.AddSingleton(api);
        Services.AddSingleton(svc);
    }

    [Fact]
    public void RendersToolbarAndTable()
    {
        var cut = RenderComponent<FileManagerMud>(parameters => parameters
            .Add(p => p.RootPrefix, "")
        );

        // Toolbar buttons
        cut.Markup.Should().Contain("New Folder");
        cut.Markup.Should().Contain("Upload");
        cut.Markup.Should().Contain("Refresh");
        cut.Markup.Should().Contain("Sort By");

        // Items rendered
        cut.Markup.Should().Contain("docs");
        cut.Markup.Should().Contain("readme.pdf");
    }
}
Directory.Build.props / csproj de testes devem referenciar bunit/FluentAssertions/NSubstitute conforme o seu setup.

Integração com o seu IApiClient
Endpoints utilizados:
EnsureContainerEndpointAsync, EnsureFolderEndpointAsync, ListFolderContentsEndpointAsync, DownloadBlobEndpointAsync, DeleteFolderEndpointAsync, DeleteBlobEndpointAsync, RenameBlobEndpointAsync, UploadBlobsEndpointAsync — todos presentes no seu ApiClient.cs gerado ApiClient.

A base e o contrato seguem o que já tem no projeto, garantindo consistência arquitetural .