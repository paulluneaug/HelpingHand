using System;
using System.Threading;

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

    public static CancellationTokenSource LinkWith(this CancellationToken token, params CancellationToken[] otherTokens)
    {
        CancellationToken[] copy = new CancellationToken[otherTokens.Length + 1];
        copy[0] = token;
        Array.Copy(otherTokens, 0, copy, 1, otherTokens.Length);
        return CancellationTokenSource.CreateLinkedTokenSource(copy);
    }
}