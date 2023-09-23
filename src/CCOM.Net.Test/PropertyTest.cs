
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
}