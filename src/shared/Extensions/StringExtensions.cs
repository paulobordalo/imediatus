namespace imediatus.Shared.Extensions;
public static class StringExtensions
{
    public static string NullToString(this object? Value)
        => Value?.ToString() ?? string.Empty;

    public static Stream String64ToStream(this string base64String)
    {
        ArgumentNullException.ThrowIfNull(base64String);

        // Remove eventuais prefixos tipo "data:application/pdf;base64,"  
        var base64 = base64String.Contains(',', StringComparison.Ordinal)
            ? base64String[(base64String.IndexOf(',', StringComparison.Ordinal) + 1)..]
            : base64String;

        byte[] bytes = Convert.FromBase64String(base64);
        return new MemoryStream(bytes);
    }

    public static byte[] FromBase64ToBytes(this string base64String)
    {
        ArgumentNullException.ThrowIfNull(base64String);

        var base64 = base64String.Contains(',', StringComparison.Ordinal)
            ? base64String[(base64String.IndexOf(',', StringComparison.Ordinal) + 1)..]
            : base64String;

        return Convert.FromBase64String(base64);
    }
}
