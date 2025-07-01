namespace imediatus.Shared.Extensions;

public static class ByteExtensions
{
    public static string ToBase64(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return Convert.ToBase64String(data);
    }
}
