#nullable enable
namespace imediatus.Blazor.Client.Components.FileManagerMud;

public class FileManagerItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
    public string ParentPath { get; set; } = "";
    public bool IsFolder { get; set; }
    public long Size { get; set; }
    public string? ContentType { get; set; }
    public DateTimeOffset? Modified { get; set; }
    public string? Url { get; set; }
}
