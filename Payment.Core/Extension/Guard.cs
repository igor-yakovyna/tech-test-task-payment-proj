using System;

namespace Payment.Core.Extension;

public static class Guard
{
    public static void ThrowIfNull<T>(this T obj, string paramName = null) where T : class
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName ?? nameof(obj), "The argument cannot be null.");
        }
    }
    
    public static void ThrowIfNullOrEmpty(this string str, string paramName = null)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentNullException(paramName ?? nameof(str), "The argument cannot be null or empty.");
        }
    }

    public static void ThrowIfNotGreaterThanZero<T>(this T value, string paramName = null) where T : struct, IComparable<T>
    {
        if (value.CompareTo(default(T)) > 0)
        {
            throw new ArgumentException("The value must be greater than zero.", paramName ?? nameof(value));
        }
    }
    
    public static void ThrowIfNotGreaterThanZero(this uint value, string paramName = null)
    {
        if (value <= 0)
        {
            throw new ArgumentException("The value must be greater than zero.", paramName ?? nameof(value));
        }
    }
}