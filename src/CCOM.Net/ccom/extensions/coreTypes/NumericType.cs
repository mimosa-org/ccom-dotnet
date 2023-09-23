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
/// </remarks>
public partial class NumericType
{
    public static implicit operator NumericType(short value)
    {
        return new NumericType() { Value = value.ToString(), format = "integer" };
    }

    public static implicit operator NumericType(int value)
    {
        return new NumericType() { Value = value.ToString(), format = "integer" };
    }

    public static implicit operator NumericType(long value)
    {
        return new NumericType() { Value = value.ToString(), format = "integer" };
    }

    public static implicit operator NumericType(BigInteger value)
    {
        return new NumericType() { Value = value.ToString("R"), format = "integer" };
    }

    // <remarks>
    // NOTE: in CCT2 parlance, 'decimal' types are restricted real numbers that
    // have a representation of special values such as infinity, as in IEEE 754.
    // </remarks>
    public static implicit operator NumericType(float value)
    {
        return new NumericType() { Value = value.ToString("G9"), format = "decimal" };
    }

    // <remarks>
    // NOTE: in CCT2 parlance, 'decimal' types are restricted real numbers that
    // have a representation of special values such as infinity, as in IEEE 754.
    // </remarks>
    public static implicit operator NumericType(double value)
    {
        return new NumericType() { Value = value.ToString("G17"), format = "decimal" };
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
        return new NumericType() { Value = value.ToString(), format = "real" };
    }
}