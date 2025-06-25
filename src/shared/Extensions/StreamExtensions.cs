namespace imediatus.Shared.Extensions;

public static class StreamExtensions
{
    public static string StreamToString64(this Stream stream)
    {
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();
        }

        return Convert.ToBase64String(bytes);
    }
}
