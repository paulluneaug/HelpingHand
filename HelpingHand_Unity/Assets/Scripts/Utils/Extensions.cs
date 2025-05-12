public static partial class Extensions
{
    public static string Truncate(this string value, int maxChars)
    {
        return value.Length <= maxChars ? value : value[..maxChars] + "...";
    }
}
