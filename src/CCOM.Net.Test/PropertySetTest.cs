using Ccom;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace CCOM.Net.Test;

public class PropertySetTest
{
    [Fact]
    public void PropertySetExample()
    {
        PropertySet propertySet = new PropertySet()
        {
            UUID = new UUID(true),
            ShortName = new TextType[] {
                new TextType { Value = "Example Property Set" }
            },
            // Generally we put the properties into groups
            Group = new PropertyGroup[]
            {
                new PropertyGroup
                {
                    UUID = new UUID(true),
                    ShortName = new TextType[] {
                        new TextType { Value = "Example Property Group" }
                    },
                    // Rather than setting it manually here, can use some
                    // general code (see below) to set them all at once.
                    // SetPropertiesName = new ItemsChoiceType7[] {
                    //     ItemsChoiceType7.SetProperty
                    // },
                    // We use set properties for the properties that are part of
                    // the property set. The inherited Properties attribute is for
                    // meta-data about the current entity: the property group
                    // itself at this level.
                    SetProperties = new Property[]
                    {
                        new Property
                        {
                            ValueContent = new ValueContent()
                            {
                                // There are different types of content.
                                // TODO: helpers to create/set/access content of different types.
                                Item = new TextType()
                                {
                                    Value = "Example Text Value"
                                }
                            }
                        }
                    }
                }
            }
        };

        // Set the choice for every SetProperty to use the element name 'SetProperty'
        // Use 'SetAttribute' for compatibility with CCOM 4.0 if desired
        foreach (var g in propertySet.Group)
        {
            g.SetPropertiesName = new ItemsChoiceType7[g.SetProperties.Length];
            Array.Fill(g.SetPropertiesName, ItemsChoiceType7.SetProperty);
        }

        var serializer = new XmlSerializer(typeof(PropertySet));
        var doc = new XDocument();
        using (var writer = doc.CreateWriter()) {
            serializer.Serialize(writer, propertySet);
        }
        Assert.Equal("PropertySet", doc.Root?.Name.LocalName);
    }
}