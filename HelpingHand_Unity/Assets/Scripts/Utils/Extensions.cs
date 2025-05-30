public static partial class Extensions
{
    public static string Truncate(this string value, int maxChars)
    {
        return value.Length <= maxChars ? value : value[..maxChars] + "...";
    }

    public static int Mod(this int value, int mod)
    {
        return (value % mod + mod) % mod;
    }

    public static int Mod(this float value, int mod)
    {
        return (int)(value % mod + mod) % mod;
    }
}