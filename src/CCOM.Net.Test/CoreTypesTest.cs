
using System.Numerics;
using System.Xml.Linq;
using System.Xml.Serialization;
using Ccom;

namespace CCOM.Net.Test;

public class CoreTypesTest
{
    [Fact]
    public void NumbersConvertToNumericTypesTest()
    {
        NumericType shortValue = (short)10;
        NumericType intValue = 10;
        NumericType longValue = 10L;
        NumericType floatValue = 10.1F;
        NumericType doubleValue = 10.1D;
        NumericType decimalValue = 10.1M;
        NumericType bigIntegerValue = new BigInteger(10);

        Assert.Equal("10", shortValue.Value);
        Assert.Equal("integer", shortValue.format);
        Assert.Equal("10", intValue.Value);
        Assert.Equal("integer", intValue.format);
        Assert.Equal("10", longValue.Value);
        Assert.Equal("integer", longValue.format);
        Assert.Equal("10", bigIntegerValue.Value);
        Assert.Equal("integer", bigIntegerValue.format);
        Assert.Equal("10.1000004", floatValue.Value); // precision error
        Assert.Equal("decimal", floatValue.format);
        Assert.Equal("10.1", doubleValue.Value);
        Assert.Equal("decimal", doubleValue.format);
        Assert.Equal("10.1", decimalValue.Value);
        Assert.Equal("real", decimalValue.format);
    }

    [Fact]
    public void FloatConvertToNumericFormatTest()
    {
        NumericType wholeFloatValue = 10.0F;
        NumericType wholeDoubleValue = 10.0D;
        NumericType wholeDecimalValue = 10.0M;
        NumericType fractionalFloatValue = 10.123456789F;
        NumericType fractionalDoubleValue = 10.123456789D;
        NumericType fractionalDecimalValue = 10.123456789M;

        Assert.Equal("10", wholeFloatValue.Value);
        Assert.Equal("decimal", wholeFloatValue.format);
        Assert.Equal("10", wholeDoubleValue.Value);
        Assert.Equal("decimal", wholeDoubleValue.format);
        Assert.Equal("10.0", wholeDecimalValue.Value);
        Assert.Equal("real", wholeDecimalValue.format);
        Assert.Equal("10.123457", fractionalFloatValue.Value);
        Assert.Equal("decimal", fractionalFloatValue.format);
        Assert.Equal("10.123456789", fractionalDoubleValue.Value);
        Assert.Equal("decimal", fractionalDoubleValue.format);
        Assert.Equal("10.123456789", fractionalDecimalValue.Value);
        Assert.Equal("real", fractionalDecimalValue.format);
    }

    [Fact]
    public void StringsConvertToTextTypeTest()
    {
        TextType text = "a simple string";

        Assert.Equal("a simple string", text.Value);
        Assert.Null(text.languageID);
        Assert.Null(text.languageLocaleID);
    }

    [Fact]
    public void GuidsConvertToUUIDTest()
    {
        var guid = Guid.NewGuid();
        UUID uuid = guid;
        Assert.Equal(guid.ToString(), uuid.Value);
    }

    [Fact]
    public void UrisConvertToURITypeTest()
    {
        var uriString = "https://example.com";
        var uriStringWithSpaces = "http://example.com/a path/with( )spaces";
        var relativeUriString = "./just/a/path";
        var relativeUriStringWithSpaces = "./just/a path";

        URI fromUri = new Uri(uriString);
        URI fromString = URI.Create(uriString);
        URI fromUriWithSpaces = new Uri(uriStringWithSpaces);
        URI fromStringWithSpaces = URI.Create(uriStringWithSpaces);
        URI fromRelativeUriString = URI.Create(relativeUriString);

        // Note the inclusion of the '/' for an empty path since path
        // is non-optional and inferred. This may impact equality checks
        // for Ccom.URI if the origin of the string is different
        Assert.Equal($"{uriString}/", fromUri.Value);
        Assert.Null(fromUri.resourceName);
        Assert.Equal($"{uriString}/", fromString.Value);
        Assert.Null(fromString.resourceName);
        
        // Note that escaping is performed
        Assert.Equal(uriStringWithSpaces.Replace(" ", "%20"), fromStringWithSpaces.Value);
        Assert.Null(fromStringWithSpaces.resourceName);
        Assert.Equal(uriStringWithSpaces, new Uri(fromStringWithSpaces.Value).ToString());

        Assert.Equal(uriStringWithSpaces.Replace(" ", "%20"), fromUriWithSpaces.Value);
        Assert.Null(fromUriWithSpaces.resourceName);
        Assert.Equal(uriStringWithSpaces, new Uri(fromUriWithSpaces.Value).ToString());

        // Supports relative URIs as well, but use the Uri class directly if more control is needed.
        Assert.Equal(relativeUriString, fromRelativeUriString.Value);
        Assert.Null(fromRelativeUriString.resourceName);
        // But relative URI strings must be fully escaped beforehand.
        // Be aware that almost anything is a valid relative URL, so it makes validation difficult
        Assert.Throws<UriFormatException>(() => URI.Create(relativeUriStringWithSpaces));

        Assert.Throws<UriFormatException>(() => URI.Create("::not-a-valid-uri"));
    }

    [Fact]
    public void DateTimeConvertsToUTCDateTime()
    {
        var universalTime = new DateTime(2023, 09, 23, 14, 19, 33, DateTimeKind.Utc);
        var localTime = new DateTime(2023, 09, 23, 14, 19, 33, DateTimeKind.Local);
        var localTimeZone = TimeZoneInfo.Local;
        var localOffset = localTimeZone.GetUtcOffset(localTime);

        var timeWithOffset = new DateTimeOffset(2023, 09, 23, 14, 19, 33, new TimeSpan(10, 0, 0));

        UTCDateTime fromUniversalTime = universalTime;
        UTCDateTime fromLocalTime = localTime;
        UTCDateTime fromTimeWithOffset = timeWithOffset;

        Assert.Equal("2023-09-23T14:19:33Z", fromUniversalTime.Value);
        Assert.False(fromUniversalTime.locHrDeltaFromUTCSpecified);
        Assert.Null(fromUniversalTime.locMinDeltaFromUTC);

        Assert.Equal($"2023-09-{(14 - localOffset.TotalHours < 0 ? 22 : 23)}T{Math.Floor(14 - localOffset.TotalHours):00}:{(19 - localOffset.Minutes + 60) % 60:00}:33Z", fromLocalTime.Value);
        Assert.Equal(localOffset.Hours, fromLocalTime.locHrDeltaFromUTC);
        Assert.True(fromLocalTime.locHrDeltaFromUTCSpecified);
        Assert.Equal(localOffset.Minutes.ToString(), fromLocalTime.locMinDeltaFromUTC);

        Assert.Equal("2023-09-23T04:19:33Z", fromTimeWithOffset.Value);
        Assert.Equal(timeWithOffset.Offset.Hours, fromTimeWithOffset.locHrDeltaFromUTC);
        Assert.True(fromTimeWithOffset.locHrDeltaFromUTCSpecified);
        Assert.Equal(timeWithOffset.Offset.Minutes.ToString(), fromTimeWithOffset.locMinDeltaFromUTC);
    }

    [Fact]
    public void ValueContentAssignmentTest()
    {
        ValueContent trueValue = true;
        ValueContent falseValue = false;
        Assert.IsType<bool>(trueValue.Item);
        Assert.IsType<bool>(falseValue.Item);

        ValueContent shortValue = (short)10;
        ValueContent intValue = 10;
        ValueContent longValue = 10L;
        ValueContent bigIntValue = new BigInteger(10);
        ValueContent floatValue = 10.1F;
        ValueContent doubleValue = 10.1D;
        ValueContent decimalValue = 10.1M;
        ValueContent numericValue = new NumericType();
        ValueContent percentageValue = new Percentage();
        ValueContent probabilityValue = new Probability();

        Assert.IsType<NumericType>(shortValue.Item);
        Assert.IsType<NumericType>(intValue.Item);
        Assert.IsType<NumericType>(longValue.Item);
        Assert.IsType<NumericType>(bigIntValue.Item);
        Assert.IsType<NumericType>(floatValue.Item);
        Assert.IsType<NumericType>(doubleValue.Item);
        Assert.IsType<NumericType>(decimalValue.Item);
        Assert.IsType<NumericType>(numericValue.Item);
        Assert.IsType<Percentage>(percentageValue.Item);
        Assert.IsType<Probability>(probabilityValue.Item);

        ValueContent textValue = "a simple string";
        ValueContent textTypeValue = new TextType();
        Assert.IsType<TextType>(textValue.Item);
        Assert.IsType<TextType>(textTypeValue.Item);

        ValueContent uriValue = new Uri("https://example.com");
        ValueContent ccomUriValue = new URI();
        Assert.IsType<URI>(uriValue.Item);
        Assert.IsType<URI>(ccomUriValue.Item);

        ValueContent dateTimeValue = DateTime.UtcNow;
        ValueContent dateTimeWithOffsetValue = DateTimeOffset.Now;
        ValueContent utcDateTimeValue = new UTCDateTime();

        Assert.IsType<UTCDateTime>(dateTimeValue.Item);
        Assert.IsType<UTCDateTime>(dateTimeWithOffsetValue.Item);
        Assert.IsType<UTCDateTime>(utcDateTimeValue.Item);

        ValueContent binaryData = new BinaryData();
        ValueContent binaryObject = new BinaryObject();
        ValueContent coordinate = new Coordinate();
        ValueContent enumerationItem = new EnumerationItem();
        ValueContent measure = Measure.Create(10, new UnitOfMeasure());
        ValueContent multiparameter = new MultiParameter();
        ValueContent uuid = UUID.Create();
        ValueContent uuidImplicit = Guid.NewGuid();
        ValueContent xml = new XML();

        Assert.IsType<BinaryData>(binaryData.Item);
        Assert.IsType<BinaryObject>(binaryObject.Item);
        Assert.IsType<Coordinate>(coordinate.Item);
        Assert.IsType<EnumerationItem>(enumerationItem.Item);
        Assert.IsType<Measure>(measure.Item);
        Assert.IsType<MultiParameter>(multiparameter.Item);
        Assert.IsType<UUID>(uuid.Item);
        Assert.IsType<UUID>(uuidImplicit.Item);
        Assert.IsType<XML>(xml.Item);
    }

    [Fact]
    public void ValueContentSerializationTest()
    {
        var expectations = new (ValueContent Value, string ElementName)[]
        {
            (true, "Boolean"),
            (10, "Number"),
            (10.1D, "Number"),
            (new Percentage(), "Percentage"),
            (new Probability(), "Probability"),
            ("a simple string", "Text"),
            (new Uri("https://example.com"), "URI"),
            (DateTime.UtcNow, "UTCDateTime"),
            (DateTimeOffset.Now, "UTCDateTime"),
            (Guid.NewGuid(), "UUID"),
            (new BinaryData(), "BinaryData"),
            (new BinaryObject(), "BinaryObject"),
            (new Coordinate(), "Coordinate"),
            (new EnumerationItem(), "EnumerationItem"),
            (Measure.Create(10, new UnitOfMeasure()), "Measure"),
            (new MultiParameter(), "MultiParameter"),
            (new XML(), "XML"),
            (new ValueContent { Item = "string" }, ""), // Invalid content (check throws)
        };

        var serializer = new XmlSerializer(typeof(ValueContent));
        foreach (var (Value, ElementName) in expectations)
        {
            var doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                if (string.IsNullOrEmpty(ElementName))
                {
                    Assert.Throws<InvalidOperationException>(() => serializer.Serialize(writer, Value));
                    continue;
                }
                else
                {
                    serializer.Serialize(writer, Value);
                }
            }
            Assert.Equal(ElementName, Assert.Single(doc.Root?.Elements() ?? Enumerable.Empty<XElement>()).Name.LocalName);
        }
    }
}