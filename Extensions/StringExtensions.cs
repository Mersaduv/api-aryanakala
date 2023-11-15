namespace ApiAryanakala.Extensions;

public static class StringExtensions
{
    public static bool IsNullEmpty(this string? value) => string.IsNullOrEmpty(value);
    public static bool Not(this bool value) => !value;
}