using System.Numerics;
using Ccom;

namespace Ccom;

public partial class ValueContent
{
    public static implicit operator ValueContent(BinaryData value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(BinaryObject value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(bool value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(Coordinate value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(EnumerationItem value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(Measure value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(MultiParameter value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(short value)
    {
        return new ValueContent() { Item = (NumericType)value };
    }

    public static implicit operator ValueContent(int value)
    {
        return new ValueContent() { Item = (NumericType)value };
    }

    public static implicit operator ValueContent(long value)
    {
        return new ValueContent() { Item = (NumericType)value };
    }

    public static implicit operator ValueContent(BigInteger value)
    {
        return new ValueContent() { Item = (NumericType)value };
    }

    public static implicit operator ValueContent(float value)
    {
        return new ValueContent() { Item = (NumericType)value };
    }

    public static implicit operator ValueContent(double value)
    {
        return new ValueContent() { Item = (NumericType)value };
    }

    public static implicit operator ValueContent(decimal value)
    {
        return new ValueContent() { Item = (NumericType)value };
    }

    // Includes Percentage and Probability types
    public static implicit operator ValueContent(NumericType value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(string value)
    {
        return new ValueContent() { Item = (TextType)value };
    }

    public static implicit operator ValueContent(TextType value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(System.Uri value)
    {
        return new ValueContent() { Item = (Ccom.URI)value };
    }

    public static implicit operator ValueContent(Ccom.URI value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(DateTime value)
    {
        return new ValueContent() { Item = (UTCDateTime)value };
    }

    public static implicit operator ValueContent(DateTimeOffset value)
    {
        return new ValueContent() { Item = (UTCDateTime)value };
    }

    public static implicit operator ValueContent(UTCDateTime value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(Guid value)
    {
        return new ValueContent() { Item = (UUID)value };
    }

    public static implicit operator ValueContent(UUID value)
    {
        return new ValueContent() { Item = value };
    }

    public static implicit operator ValueContent(XML value)
    {
        return new ValueContent() { Item = value };
    }
}