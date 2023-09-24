
using System.Numerics;
using Ccom;
using CommonBOD;

namespace CCOM.Net.Test;

public class PropertyTest
{
    [Fact]
    public void PropertyCreationExplicitTest()
    {
        var propertyName = "My Property";
        // Boolean is the simplest value content
        var property = Property.Create(propertyName, new ValueContent() { Item = true });
        Assert.Equal(propertyName, Assert.Single(property.ShortName).Value);
        Assert.Equal(true, property.ValueContent.Item);
        Assert.Null(property.TypeOrDefinition);

        property = Property.Create(propertyName, new ValueContent() { Item = true }, type: new PropertyType());
        Assert.Equal(propertyName, Assert.Single(property.ShortName).Value);
        Assert.Equal(true, property.ValueContent.Item);
        Assert.IsType<PropertyType>(property.TypeOrDefinition);

        property = Property.Create(propertyName, new ValueContent() { Item = true }, definition: new PropertyDefinition());
        Assert.Equal(propertyName, Assert.Single(property.ShortName).Value);
        Assert.Equal(true, property.ValueContent.Item);
        Assert.IsType<PropertyDefinition>(property.TypeOrDefinition);
    }

    [Fact]
    public void PropertyCreationWithImplicitConversionTest()
    {
        var propertyName = "My Property";

        var booleanProperty = Property.Create(propertyName, true);
        Assert.Equal(propertyName, Assert.Single(booleanProperty.ShortName).Value);
        Assert.Equal(true, booleanProperty.ValueContent.Item);

        var numericProperty = Property.Create(propertyName, 10);
        Assert.Equal(propertyName, Assert.Single(numericProperty.ShortName).Value);
        Assert.Equal(10, int.Parse(((NumericType)numericProperty.ValueContent.Item).Value));

        var doubleProperty = Property.Create(propertyName, 10.1);
        Assert.Equal(propertyName, Assert.Single(doubleProperty.ShortName).Value);
        Assert.Equal(10.1, double.Parse(((NumericType)doubleProperty.ValueContent.Item).Value));

        var textProperty = Property.Create(propertyName, "string value");
        Assert.Equal(propertyName, Assert.Single(textProperty.ShortName).Value);
        Assert.Equal("string value", ((TextType)textProperty.ValueContent.Item).Value);

        var uriProperty = Property.Create(propertyName, new Uri("https://example.com/"));
        Assert.Equal(propertyName, Assert.Single(uriProperty.ShortName).Value);
        Assert.Equal("https://example.com/", ((URI)uriProperty.ValueContent.Item).Value);

        var guid = Guid.NewGuid();
        var uuidProperty = Property.Create(propertyName, guid);
        Assert.Equal(propertyName, Assert.Single(uuidProperty.ShortName).Value);
        Assert.Equal(guid.ToString(), ((UUID)uuidProperty.ValueContent.Item).Value);

        var dateTime = DateTime.UtcNow;
        var dateProperty = Property.Create(propertyName, dateTime);
        Assert.Equal(propertyName, Assert.Single(dateProperty.ShortName).Value);
        Assert.Equal(dateTime.ToXsDateTimeString(), ((UTCDateTime)dateProperty.ValueContent.Item).Value);

        var dateTimeOffset = DateTimeOffset.Now;
        var dateOffsetProperty = Property.Create(propertyName, dateTimeOffset);
        Assert.Equal(propertyName, Assert.Single(dateOffsetProperty.ShortName).Value);
        Assert.Equal(dateTimeOffset.ToXsDateTimeString(), ((UTCDateTime)dateOffsetProperty.ValueContent.Item).Value);
    }

    [Fact]
    public void GetValueNullTest()
    {
        var property = Property.Create("test");
        Assert.False(property.IsNumber);
        Assert.False(property.IsBoolean); // we are not bother to check all testing properties
        Assert.Null(property.GetValue<NumericType>());
        Assert.Equal(0, property.GetValue<int>());
        Assert.Null(property.GetValue<int?>());
        Assert.False(property.GetValue<bool>());
        Assert.Null(property.GetValue<bool?>());
    }

    [Fact]
    public void GetValueInvalidCastTest()
    {
        var property = Property.Create("test", "text type");
        Assert.False(property.IsNumber);
        Assert.True(property.IsText);
        Assert.NotNull(property.GetValue<TextType>());
        Assert.Null(property.GetValue<NumericType>());
        Assert.Null(property.GetValue<int?>());
        Assert.Equal(0, property.GetValue<int>());
        Assert.Null(property.GetValue<double?>());
        Assert.Equal(0D, property.GetValue<double>());
        Assert.False(property.GetValue<bool>());
        Assert.Null(property.GetValue<bool?>());
        Assert.Null(property.GetValue<Guid?>());
        Assert.Equal(Guid.Empty, property.GetValue<Guid>());
        Assert.Null(property.GetValue<DateTime?>());
        Assert.Equal(DateTime.MinValue, property.GetValue<DateTime>());
        Assert.Null(property.GetValue<DateTimeOffset?>());
        Assert.Equal(DateTimeOffset.MinValue, property.GetValue<DateTimeOffset>());
    }

    [Fact]
    public void GetValueIntegerTest()
    {
        var property = Property.Create("test", 10);
        Assert.True(property.IsNumber);
        Assert.False(property.IsBoolean);
        Assert.False(property.IsText); // we are not bothering to check all testing properties
        Assert.NotNull(property.GetValue<NumericType>());
        Assert.Equal((short)10, property.GetValue<short>());
        Assert.Equal(10, property.GetValue<int>());
        Assert.Equal(10L, property.GetValue<long>());
        Assert.Equal(10, property.GetValue<BigInteger>());
        Assert.Equal(10F, property.GetValue<float>()); // We can convert to other number types, but we should check explicitly if we need to know for sure
        Assert.Equal(10D, property.GetValue<double>());
        Assert.Equal(10M, property.GetValue<decimal>());
    }

    [Fact]
    public void GetValueFloatTest()
    {
        var property = Property.Create("test", 10.1);
        Assert.True(property.IsNumber);
        Assert.False(property.IsBoolean);
        Assert.False(property.IsText); // we are not bothering to check all testing properties
        Assert.NotNull(property.GetValue<NumericType>());
        Assert.Equal((short)10, property.GetValue<short>());
        Assert.Equal(10, property.GetValue<int>());
        Assert.Equal(10L, property.GetValue<long>());
        Assert.Equal(10, property.GetValue<BigInteger>());
        Assert.Equal(10.1F, property.GetValue<float>()); // We can convert to other number types, but we should check explicitly if we need to know for sure
        Assert.Equal(10.1D, property.GetValue<double>());
        Assert.Equal(10.1M, property.GetValue<decimal>());
    }

    [Fact]
    public void GetValueFloatNegativeExponentTest()
    {
        var property = Property.Create("test negative exponent", new ValueContent { Item = new NumericType { Value = "1.0E-2" } });
        Assert.True(property.IsNumber);
        Assert.False(property.IsBoolean);
        Assert.False(property.IsText); // we are not bothering to check all testing properties
        Assert.NotNull(property.GetValue<NumericType>());
        Assert.Equal((short)0, property.GetValue<short>());
        Assert.Equal(0, property.GetValue<int>());
        Assert.Equal(0L, property.GetValue<long>());
        Assert.Equal(0, property.GetValue<BigInteger>());
        Assert.Equal(1.0E-2F, property.GetValue<float>()); // We can convert to other number types, but we should check explicitly if we need to know for sure
        Assert.Equal(1.0E-2D, property.GetValue<double>());
        Assert.Equal(1.0E-2M, property.GetValue<decimal>());
    }

    [Fact]
    public void GetValueFloatPositiveExponentTest()
    {
        var property = Property.Create("test positive exponent", new ValueContent { Item = new NumericType { Value = "1.0E2" } });
        Assert.True(property.IsNumber);
        Assert.False(property.IsBoolean);
        Assert.False(property.IsText); // we are not bothering to check all testing properties
        Assert.NotNull(property.GetValue<NumericType>());
        Assert.Equal((short)100, property.GetValue<short>());
        Assert.Equal(100, property.GetValue<int>());
        Assert.Equal(100L, property.GetValue<long>());
        Assert.Equal(100, property.GetValue<BigInteger>());
        Assert.Equal(1.0E2F, property.GetValue<float>()); // We can convert to other number types, but we should check explicitly if we need to know for sure
        Assert.Equal(1.0E2D, property.GetValue<double>());
        Assert.Equal(1.0E2M, property.GetValue<decimal>());
    }

    [Fact]
    public void GetValueFloatPositiveExponentWithExtraFractionalTest()
    {
        var property = Property.Create("test positive exponent", new ValueContent { Item = new NumericType { Value = "1.012E2" } });
        Assert.True(property.IsNumber);
        Assert.False(property.IsBoolean);
        Assert.False(property.IsText); // we are not bothering to check all testing properties
        Assert.NotNull(property.GetValue<NumericType>());
        Assert.Equal((short)101, property.GetValue<short>());
        Assert.Equal(101, property.GetValue<int>());
        Assert.Equal(101L, property.GetValue<long>());
        Assert.Equal(101, property.GetValue<BigInteger>());
        Assert.Equal(1.012E2F, property.GetValue<float>()); // We can convert to other number types, but we should check explicitly if we need to know for sure
        Assert.Equal(1.012E2D, property.GetValue<double>());
        Assert.Equal(1.012E2M, property.GetValue<decimal>());
    }

    [Fact]
    public void GetBooleanTest()
    {
        var property = Property.Create("test", true);
        Assert.False(property.IsNumber);
        Assert.True(property.IsBoolean);
        Assert.NotNull(property.GetValue<bool?>());
        Assert.True(property.GetValue<bool>());
    }

    [Fact]
    public void GetValueTextTest()
    {
        var property = Property.Create("test", "text type");
        Assert.False(property.IsNumber);
        Assert.True(property.IsText);
        Assert.NotNull(property.GetValue<TextType>());
        Assert.Null(property.GetValue<NumericType>());
        Assert.Equal("text type", property.GetValue<TextType>()?.Value);
        Assert.Equal("text type", property.GetValue<string>());
    }

    [Fact]
    public void GetValueURITest()
    {
        var property = Property.Create("test", URI.Create("https://example.com/"));
        Assert.True(property.IsURI);
        Assert.False(property.IsText);
        Assert.NotNull(property.GetValue<URI>());
        Assert.Null(property.GetValue<TextType>());
        Assert.Equal("https://example.com/", property.GetValue<URI>()?.Value);
        Assert.Equal(new Uri("https://example.com/"), property.GetValue<Uri>());
    }

    [Fact]
    public void GetValueUUIDTest()
    {
        var guid = Guid.NewGuid();
        var property = Property.Create("test", UUID.Create(guid));
        Assert.True(property.IsUUID);
        Assert.False(property.IsText);
        Assert.NotNull(property.GetValue<UUID>());
        Assert.Null(property.GetValue<TextType>());
        Assert.Equal(guid.ToString(), property.GetValue<UUID>()?.Value);
        Assert.Equal(guid, property.GetValue<Guid>());
    }

    [Fact]
    public void GetValueUTCDateTimeTest()
    {
        var utcDateTime = DateTime.UtcNow;
        var property = Property.Create("test", utcDateTime);
        Assert.True(property.IsUTCDateTime);
        Assert.False(property.IsText);
        Assert.NotNull(property.GetValue<UTCDateTime>());
        Assert.Null(property.GetValue<TextType>());
        Assert.Equal(utcDateTime.ToXsDateTimeString(), property.GetValue<UTCDateTime>()?.Value);
        Assert.Equal(0, property.GetValue<UTCDateTime>()?.locHrDeltaFromUTC);
        Assert.Equal(DateTimeKind.Utc, property.GetValue<DateTime>().Kind);
        Assert.Equal(utcDateTime, property.GetValue<DateTime>());
        Assert.Equal((DateTimeOffset)utcDateTime, property.GetValue<DateTimeOffset>());

        var dateTimeOffset = DateTimeOffset.Now;
        property = Property.Create("test offset", dateTimeOffset);
        Assert.True(property.IsUTCDateTime);
        Assert.False(property.IsText);
        Assert.NotNull(property.GetValue<UTCDateTime>());
        Assert.Null(property.GetValue<TextType>());
        Assert.Equal(dateTimeOffset.ToXsDateTimeString(), property.GetValue<UTCDateTime>()?.Value);
        Assert.Equal(dateTimeOffset.Offset.Hours, property.GetValue<UTCDateTime>()?.locHrDeltaFromUTC);
        Assert.Equal(dateTimeOffset, property.GetValue<DateTimeOffset>());
        Assert.Equal(DateTimeKind.Utc, property.GetValue<DateTime>().Kind);
        Assert.Equal(dateTimeOffset.UtcDateTime, property.GetValue<DateTime>());

        // This demonstrates that passing a Local DateTime in comes back as UTC
        // In that sense, retrieving DateTime is lossy as the offsets are not preserved
        // due to the name of the DateTime class.
        // Unspecified Kind can be used for arbitary (but unknown) offsets, Local is
        // always interpreted as the CurrentCulture local time.
        // Use of DateTimeOffset type is preferred.
        var localDateTime = DateTime.Now;
        var localTimeZone = TimeZoneInfo.Local;
        property = Property.Create("test", localDateTime);
        Assert.True(property.IsUTCDateTime);
        Assert.Equal(localDateTime.ToXsDateTimeString(), property.GetValue<UTCDateTime>()?.Value);
        Assert.Equal(localTimeZone.GetUtcOffset(localDateTime).Hours, property.GetValue<UTCDateTime>()?.locHrDeltaFromUTC);
        Assert.Equal(DateTimeKind.Utc, property.GetValue<DateTime>().Kind);
        Assert.Equal(localDateTime.ToUniversalTime(), property.GetValue<DateTime>());

        // Just ensures that Unspecified kind is treated consistently (i.e., as UTC)
        // Note that the standard ToUniversalTime and ToLocalTime methods assume
        // that an Unspecified DateTime needs go through the Local conversion.
        var unspecifiedDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Unspecified);
        property = Property.Create("test", unspecifiedDateTime);
        Assert.True(property.IsUTCDateTime);
        Assert.Equal(unspecifiedDateTime.ToXsDateTimeString(), property.GetValue<UTCDateTime>()?.Value);
        Assert.Equal(DateTimeKind.Utc, property.GetValue<DateTime>().Kind);
        Assert.Equal(unspecifiedDateTime.Ticks, property.GetValue<DateTime>().Ticks);
        Assert.Equal(unspecifiedDateTime, property.GetValue<DateTime>());
    }
}