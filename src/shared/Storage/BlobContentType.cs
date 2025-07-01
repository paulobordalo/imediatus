using imediatus.Shared.Extensions;
using Microsoft.AspNetCore.StaticFiles;
using MimeDetective;
using HeyRed.Mime;

namespace imediatus.Shared.Storage;

public static class BlobContentType
{
    public static string DetectContentType(string fileName, Stream fileStream)
    {
        var provider = new FileExtensionContentTypeProvider();

        // 1. Tenta pela extensão do ficheiro  
        if (provider.TryGetContentType(fileName, out var contentType))
        {
            return contentType;
        }

        // 2. Se não conseguir, tenta pela extensão do ficheiro
        string mimeType = MimeTypesMap.GetMimeType(fileName);
        if (!string.IsNullOrEmpty(mimeType))
        {
            return mimeType;
        }

        // 4. Usa Mime-Detective para detetar o content-type
        var inspector = new ContentInspectorBuilder
        {
            Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
        }.Build();

        var results = inspector.Inspect(fileStream);

        // 5. Tenta obter o content-type a partir dos resultados
        var mime = results.ByMimeType().FirstOrDefault()?.MimeType;

        // 6. Se nada detetado, usa o default
        return mime ?? "application/octet-stream";
    }
}
