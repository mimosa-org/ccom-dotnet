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

    [Fact]
    public void PropertySetFromDefinitionExample()
    {
        var propertyDefintionXml = @"<?xml version=""1.0"" encoding=""utf-16""?>
            <PropertySetDefinition xmlns=""http://www.mimosa.org/ccom4"">
                <UUID>8453c9c0-1dc2-0136-db82-080027cce040</UUID>
                <ShortName>Model RFI for Asset, Property Set</ShortName>
                <Type>
                    <UUID>c8414102-95a6-41f3-a7be-c2ef4e370f10</UUID>
                    <ShortName>Undetermined</ShortName>
                </Type>
                <Group>
                    <UUID>20891b50-1dc3-0136-db82-080027cce040</UUID>
                    <ShortName>Request Info</ShortName>
                    <MinOccurs>1</MinOccurs>
                    <MaxOccurs>1</MaxOccurs>
                    <Order>1</Order>
                    <Group>
                        <UUID>5fc6f560-1dc6-0136-db82-080027cce040</UUID>
                        <ShortName>Functional Requirements Datasheets</ShortName>
                        <MinOccurs>0</MinOccurs>
                        <MaxOccurs>*</MaxOccurs>
                        <Order>1</Order>
                        <PropertyDefinition>
                            <UUID>7cc8f410-1dc6-0136-db82-080027cce040</UUID>
                            <ShortName>Functional Requirements Datasheet ID</ShortName>
                            <Type>
                                <UUID>9f8f6ad0-1dc3-0136-db82-080027cce040</UUID>
                                <ShortName>CCOM Entity UUID</ShortName>
                            </Type>
                            <IsRequired>true</IsRequired>
                            <Order>1</Order>
                        </PropertyDefinition>
                    </Group>
                    <Group>
                        <UUID>8f20b160-1dc6-0136-db82-080027cce040</UUID>
                        <ShortName>Asset Properties</ShortName>
                        <MinOccurs>0</MinOccurs>
                        <MaxOccurs>*</MaxOccurs>
                        <Order>2</Order>
                        <PropertyDefinition>
                            <UUID>a5fbfb20-1dc6-0136-db82-080027cce040</UUID>
                            <ShortName>Asset Properties Datasheet ID</ShortName>
                            <Type>
                                <UUID>9f8f6ad0-1dc3-0136-db82-080027cce040</UUID>
                                <ShortName>CCOM Entity UUID</ShortName>
                            </Type>
                            <IsRequired>true</IsRequired>
                            <Order>1</Order>
                        </PropertyDefinition>
                    </Group>
                    <PropertyDefinition>
                        <UUID>3dc244c0-1dc5-0136-db82-080027cce040</UUID>
                        <ShortName>Asset Installation Date</ShortName>
                        <Type>
                            <UUID>f6e6b644-3472-4f6a-a727-7a9e05ab3b80</UUID>
                            <ShortName>Installation Date, Most Recent</ShortName>
                        </Type>
                        <Order>2</Order>
                    </PropertyDefinition>
                    <PropertyDefinition>
                        <UUID>05226b40-1dc5-0136-db82-080027cce040</UUID>
                        <ShortName>Functional Location</ShortName>
                        <Type>
                            <UUID>9f8f6ad0-1dc3-0136-db82-080027cce040</UUID>
                            <ShortName>CCOM Entity UUID</ShortName>
                        </Type>
                        <Order>1</Order>
                    </PropertyDefinition>
                </Group>
            </PropertySetDefinition>";
        var serializer = new XmlSerializer(typeof(PropertySetDefinition), Namespace.URI);
        var reader = new StringReader(propertyDefintionXml);
        var propertySetDefinition = serializer.Deserialize(reader) as PropertySetDefinition;

        Dictionary<Guid, UUID> propertyUuidMappings = new()
        {
            { Guid.Parse("7cc8f410-1dc6-0136-db82-080027cce040"), UUID.Create("4f976e7d-b79b-4a61-b06f-833ffda7f7fa") },
            { Guid.Parse("a5fbfb20-1dc6-0136-db82-080027cce040"), UUID.Create("677bc1fc-00c6-4c58-9fc2-e13a240998c7") },
            { Guid.Parse("3dc244c0-1dc5-0136-db82-080027cce040"), UUID.Create("e3c19f96-2832-4784-94f4-f348b0b1d708") },
            { Guid.Parse("05226b40-1dc5-0136-db82-080027cce040"), UUID.Create("1d1108ea-b800-41d4-a47f-20e2494b28ed") },
        };
        UUID propertyUuidProvider(PropertyDefinition defn, Entity? parent) => propertyUuidMappings[(Guid)defn.UUID];

        Dictionary<Guid, UUID> groupUuidMappings = new()
        {
            { Guid.Parse("20891b50-1dc3-0136-db82-080027cce040"), UUID.Create("e565b8c1-fc71-49fe-908f-5fcd2d509063")},
            { Guid.Parse("5fc6f560-1dc6-0136-db82-080027cce040"), UUID.Create("18bdf752-6733-42ee-8c2b-aa9e047a88c4") },
            { Guid.Parse("8f20b160-1dc6-0136-db82-080027cce040"), UUID.Create("6d9ab83d-fc24-47e9-880b-22bcb98bd258") },
        };
        UUID groupUuidProvider(PropertyGroupDefinition defn, Entity? parent) => groupUuidMappings[(Guid)defn.UUID];

        Dictionary<Guid, ValueContent> valueMappings = new()
        {
            { Guid.Parse("4f976e7d-b79b-4a61-b06f-833ffda7f7fa"), UUID.Create("2843a9d4-f1b5-49c2-8619-98509f838341")},
            { Guid.Parse("677bc1fc-00c6-4c58-9fc2-e13a240998c7"), UUID.Create("fb9d217a-d690-4233-a7d6-e5a221fe8809") },
            { Guid.Parse("e3c19f96-2832-4784-94f4-f348b0b1d708"), DateTime.UtcNow },
            { Guid.Parse("1d1108ea-b800-41d4-a47f-20e2494b28ed"), UUID.Create("db06f266-58d2-4b05-8b35-e446c4ef2209") },
        };
        ValueContent valueProvider(PropertyDefinition defn, UUID? uuid, Entity? parent) => valueMappings[(Guid)uuid!];

        var setUuid = UUID.Create();
        var infoSource = new InfoSource
        {
            UUID = UUID.Create(),
            ShortName = new TextType[] { "Example Info Source" }
        };
        var propertySet = propertySetDefinition?.InstantiatePropertySet(
            setUuid,
            infoSource,
            uuidProvider: propertyUuidProvider,
            groupUUIDProvider: groupUuidProvider,
            valueProvider: valueProvider
        );

        Assert.Equal(setUuid.Value, propertySet?.UUID.Value);
        Assert.Equal(infoSource.UUID.Value, propertySet?.InfoSource.UUID.Value);
        // Top group
        Assert.Equal(groupUuidMappings[(Guid)propertySetDefinition?[0]?.UUID!].Value, propertySet?[0]?.UUID.Value);

        // Sub-groups
        Assert.Equal(groupUuidMappings[(Guid)(propertySetDefinition?[0] as PropertyGroupDefinition)?[2]?.UUID!].Value, 
                    (propertySet?[0] as PropertyGroup)?.GetChildren().Where(c => c is PropertyGroup).First().UUID.Value);
        Assert.Equal(groupUuidMappings[(Guid)(propertySetDefinition?[0] as PropertyGroupDefinition)?[3]?.UUID!].Value, 
                    (propertySet?[0] as PropertyGroup)?.GetChildren().Where(c => c is PropertyGroup).Last().UUID.Value);

        // Properties of top group
        Assert.Equal(propertyUuidMappings[(Guid)(propertySetDefinition?[0] as PropertyGroupDefinition)?[0]?.UUID!].Value, 
                    (propertySet?[0] as PropertyGroup)?.GetChildren().Where(c => c is Property).First().UUID.Value);
        Assert.Equal(propertyUuidMappings[(Guid)(propertySetDefinition?[0] as PropertyGroupDefinition)?[1]?.UUID!].Value, 
                    (propertySet?[0] as PropertyGroup)?.GetChildren().Where(c => c is Property).Last().UUID.Value);
    }
}