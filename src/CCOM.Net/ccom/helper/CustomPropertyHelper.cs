using Ccom;

namespace Ccom;

public class CustomPropertyHelper
{
    public static Property CreateNumericProperty(string shortName, object value)
    {
        var property = new Property
        {
            UUID = new Ccom.UUID(autogen: true),
            ShortName = new[] {
                new Ccom.TextType() {
                    Value = shortName
                    }},
            ValueContent = new ValueContent
            {
                Item = new NumericType
                {
                    Value = value.ToString()
                }
            }
        };

        return property;
    }

    public static Property CreateTextProperty(string shortName, object value)
    {
        var property = new Property
        {
            UUID = new Ccom.UUID(autogen: true),
            ShortName = new[] {
                new Ccom.TextType() {
                    Value = shortName
                } },
            ValueContent = new ValueContent
            {
                Item = new TextType
                {
                    Value = value != null ? value.ToString() : ""
                }
            }
        };

        return property;
    }

    public static string GetTextPropertyValue(object property)
    {
        return ((Ccom.TextType) property).Value; 
    }

    public static string GetNumericPropertyValue(object property)
    {
        return ((Ccom.NumericType) property).Value; 
    }
}