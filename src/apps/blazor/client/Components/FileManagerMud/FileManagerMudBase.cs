#nullable enable
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using imediatus.Blazor.Infrastructure.Api;

namespace imediatus.Blazor.Client.Components.FileManagerMud;

public class FileManagerMudBase : ComponentBase, IDisposable
{
    [Inject] protected IFileManagerService Service { get; set; } = default!;
    [Inject] protected IApiClient Api { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    // Config
    [Parameter] public string Container { get; set; } = "";
    [Parameter] public string RootPrefix { get; set; } = "";
    [Parameter] public int PageSize { get; set; } = 100;
    [Parameter] public long MaxUploadBytes { get; set; } = 2L * 1024 * 1024 * 1024; // 2 GB client cap
    [Parameter] public int ChunkSizeBytes { get; set; } = 1024 * 1024; // 1 MB
    [Parameter] public int MaxConcurrentUploads { get; set; } = 3;

    // Events
    [Parameter] public EventCallback<FileManagerItem> OnFileOpened { get; set; }
    [Parameter] public EventCallback<IReadOnlyList<FileManagerItem>> OnSelectionChanged { get; set; }
    [Parameter] public EventCallback<IReadOnlyList<FileManagerItem>> OnUploadCompleted { get; set; }
    [Parameter] public EventCallback<string> OnError { get; set; }
    [Parameter] public EventCallback<string> OnPathChanged { get; set; }
    [Parameter] public Func<string, string>? TFunc { get; set; }
    protected string T(string s) => (TFunc?.Invoke(s)) ?? s;

    // State
    protected readonly List<FileManagerItem> Items = new();
    protected readonly HashSet<string> SelectedIds = new(StringComparer.OrdinalIgnoreCase);
    protected bool Busy;
    protected bool GridMode;
    protected string CurrentPath = "";
    protected string SearchText = "";
    protected IReadOnlyList<string> CurrentSegments => SplitPath(CurrentPath);

    // Sorting
    protected enum SortField { Name, Size, Type, Modified }
    protected SortField CurrentSort = SortField.Name;
    protected bool SortAscending = true;
    protected string SortLabel => $"{CurrentSort} {(SortAscending ? "asc" : "desc")}";

    // Clipboard (Copy/Cut/Paste)
    protected record ClipboardEntry(FileManagerItem Item, bool Cut);
    protected List<ClipboardEntry> Clipboard { get; } = new();
    protected bool CanPaste => Clipboard.Count > 0;

    // Context menu
    protected class ContextMenuState { public bool Visible; public double X; public double Y; public FileManagerItem? Target; }
    protected ContextMenuState ContextMenu { get; } = new();

    // Preview
    protected bool _previewVisible;
    protected string _previewTitle = "";
    protected string? _previewUrl;
    protected string? _previewText;
    protected PreviewType _previewType = PreviewType.Unknown;
    protected enum PreviewType { Unknown, Image, Pdf, Text, Json, Xml, Markdown }

    // Upload jobs (progress/cancel)
    protected class UploadJob
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string FileName { get; init; } = "";
        public string ContentType { get; init; } = "application/octet-stream";
        public long FileSize { get; init; }
        public DateTimeOffset StartedAt { get; } = DateTimeOffset.UtcNow;
        public CancellationTokenSource Cts { get; } = new();
        public double Progress { get; set; } // 0..100
        public bool Completed { get; set; }
        public bool Canceled { get; set; }
        public string? Error { get; set; }
    }
    protected readonly ConcurrentDictionary<Guid, UploadJob> ActiveUploads = new();

    // Lifecycle
    protected override async Task OnInitializedAsync()
    {
        CurrentPath = NormalizePath(RootPrefix);
        await EnsureContainerAsync();
        await LoadAsync();
    }

    protected async Task EnsureContainerAsync()
    {
        await GuardAsync(Api.EnsureContainerEndpointAsync);
    }

    protected async Task LoadAsync()
    {
        Busy = true;
        try
        {
            Items.Clear();
            var listed = await GuardAsync(() => ListAsync(CurrentPath));
            if (listed is not null)
                Items.AddRange(listed);
        }
        finally { Busy = false; StateHasChanged(); }
    }

    protected internal async Task<IReadOnlyList<FileManagerItem>> ListAsync(string path, CancellationToken ct = default)
    {
        var normalized = NormalizePath(path);
        var raw = await Service.ListAsync(normalized, ct) ?? Array.Empty<FileManagerItem>();

        // Ensure required fields and parent path consistency
        foreach (var i in raw)
        {
            i.ParentPath = normalized;
            if (string.IsNullOrWhiteSpace(i.Id))
                i.Id = Guid.NewGuid().ToString("N");
        }

        // Apply current client-side search/sort before returning
        return ApplyClientSearchAndSort(raw);
    }

    protected async Task ReloadAsync()
    {
        SearchText = "";
        await LoadAsync();
    }

    // Toolbar actions
    protected async Task CreateFolderAsync()
    {
        var folderName = await JS.InvokeAsync<string?>("fileManagerMud.prompt", "New folder name:");
        if (string.IsNullOrWhiteSpace(folderName)) return;

        var full = CombinePath(CurrentPath, folderName.Trim());
        await GuardAsync(() => Service.EnsureFolderAsync(full));
        await LoadAsync();
    }

    // Breadcrumb helpers
    public record BreadcrumbItemDto(string Text, string Href, string Icon = "", bool Disabled = false);
    protected IEnumerable<BreadcrumbItemDto> GetBreadcrumbItems()
    {
        yield return new BreadcrumbItemDto("Home", "#/");
        var agg = "";
        foreach (var s in CurrentSegments)
        {
            agg = CombinePath(agg, s);
            yield return new BreadcrumbItemDto(s, "#/" + Uri.EscapeDataString(agg), Icons.Material.Filled.Folder);
        }
    }
    protected async Task OnBreadcrumbClick(MouseEventArgs e, BreadcrumbItemDto item)
    {
        // e.PreventDefault(); // Removed: MouseEventArgs does not have PreventDefault in Blazor
        var href = item.Href;
        if (href.StartsWith("#/"))
        {
            var path = Uri.UnescapeDataString(href[2..]);
            CurrentPath = NormalizePath(path);
            await OnPathChanged.InvokeAsync(CurrentPath);
            await LoadAsync();
        }
    }

    // Table/grid interactions
    protected void ToggleSelection(FileManagerItem item, bool isChecked)
    {
        if (isChecked) SelectedIds.Add(item.Id);
        else SelectedIds.Remove(item.Id);
        _ = OnSelectionChanged.InvokeAsync(Items.Where(x => SelectedIds.Contains(x.Id)).ToList());
    }

    // Row CSS class resolver: highlights selected rows and distinguishes folders/files.
    protected string RowClass(FileManagerItem item, int rowNumber)
    {
        var classes = new List<string>(4);

        // Selected state
        if (SelectedIds.Contains(item.Id))
            classes.Add("fm-row-selected");

        // Folder vs File
        classes.Add(item.IsFolder ? "fm-row-folder" : "fm-row-file");

        // Zebra striping (optional visual hint)
        if ((rowNumber % 2) == 1)
            classes.Add("fm-row-alt");

        return string.Join(' ', classes);
    }

    // Row inline style resolver: tiny visual cues (e.g., dim 0-byte files).
    protected string? RowStyle(FileManagerItem item, int rowNumber)
    {
        // Dim zero-byte files a little to hint “empty”.
        if (!item.IsFolder && item.Size == 0)
            return "opacity:.85;";

        return null; // No extra style
    }

    // Row click handler: open on single click; Ctrl+click toggles selection.
    protected async Task OnRowClick(TableRowClickEventArgs<FileManagerItem> args)
    {
        if (args?.Item is null)
            return;

        // Ctrl+Click toggles selection without opening
        if (args.MouseEventArgs?.CtrlKey == true)
        {
            var isSelected = SelectedIds.Contains(args.Item.Id);
            ToggleSelection(args.Item, !isSelected);
            StateHasChanged();
            return;
        }

        // Open folder/file (folder navigates; file shows preview or triggers download)
        await OnOpenAsync(args.Item);
    }

    protected async Task OnOpenAsync(FileManagerItem item)
    {
        if (item.IsFolder)
        {
            CurrentPath = CombinePath(CurrentPath, item.Name);
            await OnPathChanged.InvokeAsync(CurrentPath);
            await LoadAsync();
            return;
        }

        // Preview or binary open
        var dl = await GuardAsync(() => Service.GetDownloadAsync(CurrentPath, item.Name));
        if (dl is null) return;

        // Try inline data URL first
        var inlineUrl = await GuardAsync(() => Service.TryGetInlineDataUrlAsync(CurrentPath, item.Name));
        if (!string.IsNullOrEmpty(inlineUrl))
        {
            _previewTitle = item.Name;
            _previewUrl = inlineUrl;
            _previewType = DetectPreviewType(item);
            if (_previewType is PreviewType.Text or PreviewType.Json or PreviewType.Xml or PreviewType.Markdown)
            {
                _previewText = await Service.ReadTextAsync(CurrentPath, item.Name) ?? "(empty)";
                _previewUrl = null;
            }
            _previewVisible = true;
            StateHasChanged();
            await OnFileOpened.InvokeAsync(item);
            return;
        }

        // Fallback: trigger browser download via JS
        await JS.InvokeVoidAsync("fileManagerMud.saveAsBytes", item.Name, item.ContentType ?? "application/octet-stream", dl.Content);
    }

    protected void OpenDetails(FileManagerItem item)
    {
        var info = $"Name: {item.Name}\nType: {(item.IsFolder ? "Folder" : item.ContentType ?? "-")}\nSize: {item.Size} bytes\nModified: {item.Modified}";
        Snackbar.Add(info, Severity.Info);
    }

    protected bool CanRename => SelectedIds.Count <= 1;
    protected async Task BeginRename(FileManagerItem item)
    {
        var newName = await JS.InvokeAsync<string?>("fileManagerMud.prompt", $"Rename \"{item.Name}\" to:");
        if (string.IsNullOrWhiteSpace(newName) || newName.Trim() == item.Name) return;

        var ok = await GuardAsync(() => Service.RenameAsync(CurrentPath, item.Name, newName.Trim()));
        if (ok) await LoadAsync();
    }

    protected void Copy(FileManagerItem item) => SetClipboard(item, cut: false);
    protected void Cut(FileManagerItem item) => SetClipboard(item, cut: true);
    protected void SetClipboard(FileManagerItem item, bool cut)
    {
        Clipboard.Clear();
        Clipboard.Add(new ClipboardEntry(item, cut));
        StateHasChanged();
    }

    protected async Task Paste()
    {
        if (!CanPaste) return;
        var entry = Clipboard[0];
        // Simple rename-based copy/cut semantics via download+upload (client-side)
        if (!entry.Item.IsFolder)
        {
            var bytesResp = await Service.GetDownloadAsync(entry.Item.ParentPath, entry.Item.Name);
            if (bytesResp is not null)
            {
                var destName = entry.Item.Name;
                // Convert base64 string to byte[] before uploading
                var bytes = Convert.FromBase64String(bytesResp.Content);
                await UploadBytesAsync(destName, bytes, entry.Item.ContentType ?? "application/octet-stream");
                if (entry.Cut)
                    await Service.DeleteFileAsync(entry.Item.ParentPath, entry.Item.Name);
            }
        }
        Clipboard.Clear();
        await LoadAsync();
    }

    protected async Task DeleteAsync(FileManagerItem item)
    {
        var ok = item.IsFolder
            ? await GuardAsync(() => Service.DeleteFolderAsync(CombinePath(item.ParentPath, item.Name)))
            : await GuardAsync(() => Service.DeleteFileAsync(item.ParentPath, item.Name));

        if (ok) await LoadAsync();
    }

    // Sorting, search
    protected void SetSort(SortField field) { CurrentSort = field; SortAscending = true; _ = LoadAsync(); }
    protected void ToggleSortDir() { SortAscending = !SortAscending; _ = LoadAsync(); }
    protected void OnSearchKeyUp(KeyboardEventArgs e) { _ = LoadAsync(); }

    protected IReadOnlyList<FileManagerItem> ApplyClientSearchAndSort(IReadOnlyList<FileManagerItem> data)
    {
        IEnumerable<FileManagerItem> q = data;
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var s = SearchText.Trim();
            q = q.Where(i => i.Name.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        q = CurrentSort switch
        {
            SortField.Name => (SortAscending ? q.OrderBy(i => i.IsFolder ? 0 : 1).ThenBy(i => i.Name) : q.OrderByDescending(i => i.IsFolder ? 0 : 1).ThenByDescending(i => i.Name)),
            SortField.Size => (SortAscending ? q.OrderBy(i => i.IsFolder ? -1 : i.Size) : q.OrderByDescending(i => i.IsFolder ? -1 : i.Size)),
            SortField.Type => (SortAscending ? q.OrderBy(i => i.IsFolder ? "" : i.ContentType).ThenBy(i => i.Name) : q.OrderByDescending(i => i.IsFolder ? "" : i.ContentType).ThenByDescending(i => i.Name)),
            SortField.Modified => (SortAscending ? q.OrderBy(i => i.Modified ?? DateTimeOffset.MinValue) : q.OrderByDescending(i => i.Modified ?? DateTimeOffset.MinValue)),
            _ => q
        };

        return q.ToList();
    }

    // Context menu helpers
    protected async Task OnContextMenu(MouseEventArgs e, FileManagerItem item)
    {
        // e.PreventDefault(); // Removed: MouseEventArgs does not have PreventDefault in Blazor
        ContextMenu.Visible = true;
        ContextMenu.X = e.ClientX;
        ContextMenu.Y = e.ClientY;
        ContextMenu.Target = item;
        await JS.InvokeVoidAsync("fileManagerMud.bindContextMenuClose");
        StateHasChanged();
    }

    [JSInvokable] public void CloseContextMenu() { ContextMenu.Visible = false; StateHasChanged(); }

    // Upload
    protected async Task UploadBytesAsync(string fileName, byte[] bytes, string contentType)
    {
        // JSON base64 upload through API (UploadBlobsEndpointAsync) as requested.
        var cmd = new UploadBlobCommand
        {
            FolderName = CurrentPath,
            Files = new System.Collections.Generic.List<UploadBlobFile>
            {
                new UploadBlobFile
                { 
                    FileName = fileName,
                    FileData = Convert.ToBase64String(bytes),
                    ContentType = contentType
                }
            }
        };

        var resp = await GuardAsync(() => Api.UploadBlobsEndpointAsync(cmd));
        if (resp is null) return;

        Snackbar.Add($"Uploaded: {fileName}", Severity.Success);
        await LoadAsync();
    }

    public BreadcrumbItemDto MapToBreadcrumbItemDto(FileManagerItem item)
    {
        return new BreadcrumbItemDto(
            item.Name,
            "#/" + Uri.EscapeDataString(CombinePath(item.ParentPath, item.Name)),
            item.IsFolder ? Icons.Material.Filled.Folder : "",
            false
        );
    }
    
    // Utilities
    protected static string NormalizePath(string? path)
        => string.IsNullOrWhiteSpace(path) ? "" : path.Trim().Trim('/');

    protected static IReadOnlyList<string> SplitPath(string path)
        => string.IsNullOrWhiteSpace(path) ? Array.Empty<string>() : path.Split('/', StringSplitOptions.RemoveEmptyEntries);

    protected static string CombinePath(string left, string right)
        => NormalizePath(string.Join('/', new[] { NormalizePath(left), NormalizePath(right) }.Where(s => !string.IsNullOrEmpty(s))));

    protected PreviewType DetectPreviewType(FileManagerItem item)
    {
        if (item.IsFolder) return PreviewType.Unknown;
        var ct = item.ContentType?.ToLowerInvariant() ?? "";
        if (ct.StartsWith("image/")) return PreviewType.Image;
        if (ct == "application/pdf") return PreviewType.Pdf;
        if (ct.Contains("json")) return PreviewType.Json;
        if (ct.Contains("xml")) return PreviewType.Xml;
        if (item.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase)) return PreviewType.Markdown;
        if (ct.StartsWith("text/")) return PreviewType.Text;
        return PreviewType.Unknown;
    }

    protected async Task<T?> GuardAsync<T>(Func<Task<T>> action)
    {
        try { return await action(); }
        catch (ApiException ex)
        {
            var msg = ex.Message ?? "Server error";
            Snackbar.Add(msg, Severity.Error);
            await OnError.InvokeAsync(msg);
            return default;
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            Snackbar.Add(msg, Severity.Error);
            await OnError.InvokeAsync(msg);
            return default;
        }
    }

    protected async Task GuardAsync(Func<Task> action)
    {
        try { await action(); }
        catch (ApiException ex)
        {
            var msg = ex.Message ?? "Server error";
            Snackbar.Add(msg, Severity.Error);
            await OnError.InvokeAsync(msg);
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            Snackbar.Add(msg, Severity.Error);
            await OnError.InvokeAsync(msg);
        }
    }

    public void Dispose()
    {
        foreach (var u in ActiveUploads.Values) u.Cts.Cancel();
        ActiveUploads.Clear();
    }
}
