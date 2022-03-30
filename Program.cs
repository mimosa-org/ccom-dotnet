// The following is the same as:
// namespace CcomExample
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             Console.WriteLine("Hello World!");
//             ...
//         }
//     }
// }
// Revert to above for older C# versions.

using System.Xml.Serialization;
using Ccom;

// Serialiser for the CCOMData root element, so basically everything.
XmlSerializer s = new XmlSerializer(typeof(CCOMData), "http://www.mimosa.org/ccom4");

Console.WriteLine("Reading/Writing example CCOM data file");

CCOMData? readData = null;
using (TextReader reader = new StreamReader("./data/example_ccom_seg_asset_props.xml")) {
    readData = s.Deserialize(reader) as CCOMData;
}

if (readData != null) {
    Console.WriteLine("The READ data includes {0} entities", readData.Entity.Length);

    using (TextWriter writer = new StreamWriter("./data/example_writeout_test.xml")) {
        s.Serialize(writer, readData);
    }
}
else {
    Console.WriteLine("The READ CCOMData failed to read the entities");
}

// Simple example just initialising objects. Can also create and set properties individually.
Console.WriteLine("Creating example asset in code and writing it out.");

EffectiveStatusType activeStatusType = new EffectiveStatusType {
    UUID = new UUID() { Value = Guid.NewGuid().ToString() },
    ShortName = new TextType[] { new TextType{Value = "Active"} },
    InfoSource = new InfoSource {
        UUID = new UUID() { Value = Guid.NewGuid().ToString() },
        ShortName = new TextType[] { new TextType{Value = "Fake InfoSource"} },
    }
};
// Don't do this: circular references break the serialisation.
// activeStatusType.EffectiveStatus = new Entity[] { activeStatusType };

// A real SDK would provide a means of managing the references, particularly circular ones
// However, since this is just bare bones, need to manage them manually
activeStatusType.EffectiveStatus = new Entity[] {
    new EffectiveStatusType {
        // Object references in CCOM XML comprise at minimum UUID, but including ShortName is recommended.
        UUID = activeStatusType.UUID,
        ShortName = activeStatusType.ShortName
    }
};

Asset asset = new Asset {
    UUID = new UUID() { Value = Guid.NewGuid().ToString() },
    ShortName = new TextType[] { new TextType{Value = "Asset Tag"} },
    EffectiveStatus = new EffectiveStatus[] {
        new EffectiveStatus() { 
            UUID = new UUID() { Value = Guid.NewGuid().ToString() },
            InfoSource = new InfoSource() { 
                UUID = new UUID { Value = Guid.NewGuid().ToString() },
                ShortName = new TextType[] { new TextType{Value = "Info Source Example"} }
            },
            Type = new EffectiveStatusType() {
                UUID = new UUID() { Value = Guid.NewGuid().ToString() },
                ShortName = new TextType[] { new TextType{Value = "Test Type"} }
            },
            EffectiveStatus = new Entity[] { activeStatusType.EffectiveStatus.First() }
        }
    },
    PresentLifecycleStatus = new LifecycleStatusType[] { 
        new LifecycleStatusType { 
            UUID = new UUID { Value = Guid.NewGuid().ToString() }, 
            ShortName = new TextType[] { new TextType { Value = "New Status" } },
            Type = new LifecycleStatusKind {
                UUID = new UUID {Value = Guid.NewGuid().ToString()},
                ShortName = new TextType[] { new TextType{Value = "New Status Kind"} }
            }
        }
    },
    // This lets you choose between the old and new terminology as it is a 'choice' in the XSD
    // Always use the classes with the names 'Property...' though as the attribute variants are
    // just empty subclasses. Should always keep it consistent too.
    // PropertySetsName = new Items2ChoiceType1[] { Items2ChoiceType1.AttributeSetForEntity }
    PropertySetsName = new Items2ChoiceType1[] { Items2ChoiceType1.PropertySetForEntity },
    PropertySets = new PropertySetForEntity[] {
        new PropertySetForEntity() {
            UUID = new UUID{ Value = Guid.NewGuid().ToString() },
            PropertySet = new PropertySet {
                UUID = new UUID{ Value = Guid.NewGuid().ToString() },
                ShortName = new TextType[] { new TextType{ Value = "Propertyy SET 1"} },
                SetProperties = new Property[] {
                    new Property { 
                        UUID = new UUID{ Value = Guid.NewGuid().ToString() },
                        ShortName = new TextType[] { new TextType{ Value = "SET Property 1"} },
                        ValueContent = new ValueContent { Item = new TextType{ Value = "Property Value" } }
                    }
                },
                // SetPropertiesName = new ItemsChoiceType6[] { ItemsChoiceType6.SetAttribute },
                SetPropertiesName = new ItemsChoiceType6[] { ItemsChoiceType6.SetProperty },
                Group = new PropertyGroup[] {
                    new PropertyGroup {
                        UUID = new UUID{ Value = Guid.NewGuid().ToString() },
                        ShortName = new TextType[] { new TextType{ Value = "Propertyy SET 1 GROUP 1"} },
                        SetProperties = new Property[] {
                            new Property { 
                                UUID = new UUID{ Value = Guid.NewGuid().ToString() },
                                ShortName = new TextType[] { new TextType{ Value = "SET-GROUP Property 1"} },
                                ValueContent = new ValueContent { Item = new TextType{ Value = "Property Value" } }
                            }
                        },
                        // SetPropertiesName = new ItemsChoiceType7[] { ItemsChoiceType7.SetAttribute },
                        SetPropertiesName = new ItemsChoiceType7[] { ItemsChoiceType7.SetProperty },
                    }
                }
            },
            // PropertySetName = ItemChoiceType2.AttributeSet
            PropertySetName = ItemChoiceType2.PropertySet
        }
    }
};

MemoryStream mem = new MemoryStream();
TextWriter w = new StreamWriter(mem);
CCOMData writeData = new CCOMData { Entity = new Entity[] { activeStatusType, asset } };
s.Serialize(w, writeData);
using (StreamReader r = new StreamReader(mem)) {
    mem.Position = 0;
    Console.WriteLine("Serialized content\n{0}", r.ReadToEnd());

    mem.Position = 0;
    using (TextWriter writer = new StreamWriter("./data/simple_test.xml")) {
        writer.WriteLine(r.ReadToEnd());
    }
}
