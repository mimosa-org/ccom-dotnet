using System.Globalization;
using System.Numerics;
using Ccom;

namespace Ccom;

/// <summary>
/// Represenation of NumericTypes according to UNCEFACT's CoreComponentTypes v2.
/// </summary>
/// <remarks>
/// Implicit operators mostly depend on the default ToString formatting of the
/// number types, with the exception of float, double, and BigInteger
/// which follow the recommendations for round-trip formatting.
/// See: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
/// 
/// If more control is needed over the formatting, the explict constructors should
/// be used instead. This is of particular importance to floating point types as
/// the number of decimal places may not be desired.
/// 
/// Invariant culture is used for the default conversions.
/// </remarks>
/// <remarks>
/// Explicit operators allow extracting the integer portions from floating point
/// values. This is to remain somewhat consistent with the fact that explciit
/// casts can be made between any primitive numeric type.
/// 
/// Other errors in the parsing string will still cause cast failures.
/// </remarks
public partial class NumericType
{
    public static implicit operator NumericType(short value)
    {
        return new NumericType() { Value = value.ToString(NumberFormatInfo.InvariantInfo), format = "integer" };
    }

    public static implicit operator NumericType(int value)
    {
        return new NumericType() { Value = value.ToString(NumberFormatInfo.InvariantInfo), format = "integer" };
    }

    public static implicit operator NumericType(long value)
    {
        return new NumericType() { Value = value.ToString(NumberFormatInfo.InvariantInfo), format = "integer" };
    }

    public static implicit operator NumericType(BigInteger value)
    {
        return new NumericType() { Value = value.ToString("R", NumberFormatInfo.InvariantInfo), format = "integer" };
    }

    // <remarks>
    // NOTE: in CCT2 parlance, 'decimal' types are restricted real numbers that
    // have a representation of special values such as infinity, as in IEEE 754.
    // </remarks>
    public static implicit operator NumericType(float value)
    {
        return new NumericType() { Value = value.ToString("G9", NumberFormatInfo.InvariantInfo), format = "decimal" };
    }

    // <remarks>
    // NOTE: in CCT2 parlance, 'decimal' types are restricted real numbers that
    // have a representation of special values such as infinity, as in IEEE 754.
    // </remarks>
    public static implicit operator NumericType(double value)
    {
        return new NumericType() { Value = value.ToString("G17", NumberFormatInfo.InvariantInfo), format = "decimal" };
    }

    // <remarks>
    // NOTE: in CCT2 parlance, 'decimal' types are restricted real numbers that
    // have a representation of special values such as infinity, as in IEEE 754.
    // Since C# decimals do not have such a representation, they are mapped to
    // the format 'real' number instead. This is also potentially useful for
    // probabilities as C# decimals have an improved precision to the right
    // of the decimal point.
    // </remarks>
    public static implicit operator NumericType(decimal value)
    {
        return new NumericType() { Value = value.ToString(NumberFormatInfo.InvariantInfo), format = "real" };
    }


    public static explicit operator short?(NumericType? value) => value is null ? null : (short)value;
    public static explicit operator short(NumericType value)
    {
        if (short.TryParse(value.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var val))
            return val;
        // A negative exponent means 0 < abs(value) < 1, so return zero
        if (ContainsInvalidFractional(value.Value, out var newValue))
            return short.Parse(newValue, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
        // Not a negative exponent interferring, so raise the normal exception
        return short.Parse(value.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
    }
    public static explicit operator int?(NumericType? value) => value is null ? null : (int)value;
    public static explicit operator int(NumericType value)
    {
        if (int.TryParse(value.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var val))
            return val;
        // A negative exponent means 0 < abs(value) < 1, so return zero
        if (ContainsInvalidFractional(value.Value, out var newValue))
            return int.Parse(newValue, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
        // Not a negative exponent interferring, so raise the normal exception
        return int.Parse(value.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
    }
    public static explicit operator long?(NumericType? value) => value is null ? null : (long)value;
    public static explicit operator long(NumericType value)
    {
        if (long.TryParse(value.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var val))
            return val;
        // A negative exponent means 0 < abs(value) < 1, so return zero
        if (ContainsInvalidFractional(value.Value, out var newValue))
            return long.Parse(newValue, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
        // Not a negative exponent interferring, so raise the normal exception
        return long.Parse(value.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
    }
    public static explicit operator BigInteger?(NumericType? value) => value is null ? null : (BigInteger)value;
    public static explicit operator BigInteger(NumericType value)
    {
        if (BigInteger.TryParse(value.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var val))
            return val;
        // A negative exponent means 0 < abs(value) < 1, so return zero
        if (ContainsInvalidFractional(value.Value, out var newValue))
            return BigInteger.Parse(newValue, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
        // Not a negative exponent interferring, so raise the normal exception
        return BigInteger.Parse(value.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
    }
    public static explicit operator float?(NumericType? value) => value is null ? null : (float)value;
    public static explicit operator float(NumericType value) => float.Parse(value.Value, NumberFormatInfo.InvariantInfo);
    public static explicit operator double?(NumericType? value) => value is null ? null : (double)value;
    public static explicit operator double(NumericType value) => double.Parse(value.Value, NumberFormatInfo.InvariantInfo);
    public static explicit operator decimal?(NumericType? value) => value is null ? null : (decimal)value;
    public static explicit operator decimal(NumericType value) => decimal.Parse(value.Value, NumberStyles.Number | NumberStyles.AllowExponent , NumberFormatInfo.InvariantInfo);

    private static readonly char[] EXPONENT_SEPARATORS = new [] { 'e', 'E' };
    private static bool ContainsInvalidFractional(string value, out string newValue)
    {
        // This version protects against variants where there are multiple digits
        // before the decimal point, which may mean that a negative exponent does
        // not necessarily result in a 0 < value < 1 such as 10.0E-1 == 1)
        // It is probably overkill though, going to assume that there is only one
        // digit before the decimal point.
        //
        // int digitsBeforeDecimal = 0;
        // _ = value.SkipWhile(c => char.IsWhiteSpace(c)).SkipWhile(c => c != '.' && (++digitsBeforeDecimal > 0)).First();
        // int exponentIndex = value.IndexOfAny(EXPONENT_SEPARATORS, digitsBeforeDecimal);
        // int exponent = int.Parse(value.AsSpan(exponentIndex + 1));
        // return exponent < 0 && Math.Abs(exponent) >= digitsBeforeDecimal;

        int periodIndex = value.IndexOf('.');
        int exponentIndex = value.IndexOfAny(EXPONENT_SEPARATORS, Math.Max(0, periodIndex));
        int exponent;
        if (value.Length > exponentIndex && value[exponentIndex + 1] == '-')
        {
            newValue = "0";
            return true;
        }
        else if (periodIndex >= 0 && exponentIndex < 0)
        {
            // Fractional without exponent, drop the fractional portion
            newValue = value[..periodIndex];
            return true;
        }
        else if (periodIndex >= 0 && (exponent = int.Parse(value[(exponentIndex + 1)..])) < (exponentIndex - periodIndex - 1))
        {
            // E.g., 1.23E1 == 12.3 causes an overflow when parsing integers.
            // so we keep only enough digits to match the exponent
            newValue = value[..(periodIndex + exponent + 1)] + value[exponentIndex..];
            return true;
        }
        else
        {
            newValue = value;
            return false;
        }
    }
}