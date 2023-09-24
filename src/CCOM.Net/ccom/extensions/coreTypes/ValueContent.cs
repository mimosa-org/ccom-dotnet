using System.Numerics;
using Ccom;
using Microsoft.CSharp.RuntimeBinder;

namespace Ccom;

public partial class ValueContent
{
    public bool IsBinaryData => Item is BinaryData;
    public bool IsBinaryObject => Item is BinaryObject;
    public bool IsBoolean => Item is bool;
    public bool IsCoordinate => Item is Coordinate;
    public bool IsEnumerationItem => Item is EnumerationItem;
    public bool IsMeasure => Item is Measure;
    public bool IsMultiParameter => Item is MultiParameter;
    public bool IsNumber => Item?.GetType() == typeof(NumericType); // exclude the subclasses
    public bool IsPercentage => Item is Percentage;
    public bool IsProbability => Item is Probability;
    public bool IsText => Item is TextType;
    public bool IsURI => Item is URI;
    public bool IsUTCDateTime => Item is UTCDateTime;
    public bool IsUUID => Item is UUID;
    public bool IsXML => Item is XML;

    /// <summary>
    /// Returns the value (i.e., ValueContent.Item) as the specified type or null
    /// if the type does not match (applying conversion operators if available,
    /// such as TextType -> string).
    /// </summary>
    /// <typeparam name="T">
    /// The type to be returned (as defined by the ValueContent.Item and possible
    /// conversion operators)
    /// </typeparam>
    public T? As<T>()
    {
        if (Item is null) return default;
        if (Item is T i) return i;
        try
        {
            // Try explicit runtime cast (e.g., for int, string, etc.)
            dynamic item = Item;
            return (T)item;
        }
        catch (RuntimeBinderException)
        {
            return default;
        }
    }

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