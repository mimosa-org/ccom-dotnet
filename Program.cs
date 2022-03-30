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
using ccom;

Console.WriteLine("Hello, World!");

Asset asset = new Asset() {
    UUID = new UUID() { Value = Guid.NewGuid().ToString() }
};
// asset.UUID = new UUID();
// asset.UUID.Value = Guid.NewGuid().ToString();

Console.WriteLine("The Asset UUID is {0}", asset.UUID.Value);

// Percentage percent = new Percentage() { Value = "80" };
// XmlSerializer serializer = new XmlSerializer(typeof(Percentage));
// XmlSerializer serializer = new XmlSerializer(typeof(Asset), "http://www.mimosa.org/ccom4");

// TextWriter writer = new StreamWriter("test.xml");
// serializer.Serialize(writer, asset);
// writer.Close();

// TextReader reader = new StreamReader("test.xml");
// Asset? readAsset = serializer.Deserialize(reader) as Asset;
// reader.Close();

// if (readAsset != null) {
//     Console.WriteLine("The READ Asset UUID is {0} which matches? {1}", readAsset.UUID.Value, readAsset.UUID.Value == asset.UUID.Value);
// }
// else {
//     Console.WriteLine("The READ Asset failed to read the entity");
// }

asset = new Asset {
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
            EffectiveStatus = new Entity[] {
                new EffectiveStatusType { UUID = new UUID { Value = Guid.NewGuid().ToString() } }
            }
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
    LifecycleStatusHistory = new LifecycleStatusHistory[] {
        new LifecycleStatusHistory {
            UUID = new UUID { Value = Guid.NewGuid().ToString() },
            Type = new LifecycleStatusType { UUID = new UUID{ Value = Guid.NewGuid().ToString() } },
            StatusFromDate = new UTCDateTime { Value = DateTime.UtcNow.ToString("o")     } // iso8601 formatted date/time
        }
    },
    Properties = new Property[] {
        new Property { 
            UUID = new UUID{ Value = Guid.NewGuid().ToString() },
            ShortName = new TextType[] { new TextType{ Value = "Propertyy 1"} },
            ValueContent = new ValueContent { Item = new TextType{ Value = "Property Value" } }
        },
        new Property { 
            UUID = new UUID{ Value = Guid.NewGuid().ToString() },
            ShortName = new TextType[] { new TextType{ Value = "Propertyy 2"} },
            ValueContent = new ValueContent { Item = new TextType{ Value = "Another Property Value" } }
        }
    },
    PropertiesElementName = new Items1ChoiceType1[]{ Items1ChoiceType1.Attribute, Items1ChoiceType1.Attribute },
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
                SetPropertiesName = new ItemsChoiceType6[] { ItemsChoiceType6.SetAttribute },
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
                        SetPropertiesName = new ItemsChoiceType7[] { ItemsChoiceType7.SetAttribute },
                    }
                }
            },
            PropertySetName = ItemChoiceType2.AttributeSet
        }
    },
    PropertySetsName = new Items2ChoiceType1[] { Items2ChoiceType1.AttributeSetForEntity }
};

XmlSerializer s = new XmlSerializer(typeof(CCOMData), "http://www.mimosa.org/ccom4");
MemoryStream mem = new MemoryStream();
TextWriter w = new StreamWriter(mem);
s.Serialize(w, new CCOMData { Entity = new Entity[] { asset, asset.EffectiveStatus.First() } });
using (StreamReader r = new StreamReader(mem)) {
    mem.Position = 0;
    Console.WriteLine("Serialized content\n{0}", r.ReadToEnd());
}
