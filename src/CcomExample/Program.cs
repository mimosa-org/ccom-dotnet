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
using System.Xml.Linq;
using Ccom;

const string CCOM_NAMESPACE = "http://www.mimosa.org/ccom4";

// Serialiser for the CCOMData root element, so basically everything.
XmlSerializer s = new XmlSerializer(typeof(CCOMData), CCOM_NAMESPACE);

Console.WriteLine("Reading/Writing example CCOM data file");

CCOMData? readData = null;
using (TextReader reader = new StreamReader("./data/example_ccom_seg_asset_props.xml")) {
    readData = s.Deserialize(reader) as CCOMData;
}

if (readData != null) {
    Console.WriteLine("The READ data includes {0} entities", readData.Entity.Length);

    // LINQ-based query/search. XPath style would be much more convenient.
    Segment[] result = readData.Entity.Where(e => e.GetType().IsAssignableTo(typeof(Segment)))
                   .Select(e => (Segment)e )
                   .Where(s => s.FullName.Any(n => n.Value.Contains("Transmitter")))
                   .Where(s => s.PropertySets.SelectMany(ps => new PropertySet[] { ps.PropertySet } )
                        .Any(ps => ps.ShortName.Any(n => n.Value == "TIT-11122 Functional Requirements")))
                   .ToArray();
    Console.WriteLine(result.First());

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
    UUID = new UUID(true),
    ShortName = new TextType[] { new TextType{Value = "Active"} },
    InfoSource = new InfoSource {
        UUID = new UUID(true),
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
    UUID = new UUID(true),
    ShortName = new TextType[] { new TextType{Value = "Asset Tag"} },
    EffectiveStatus = new EffectiveStatus[] {
        new EffectiveStatus() { 
            UUID = new UUID(true),
            InfoSource = new InfoSource() { 
                UUID = new UUID { Value = Guid.NewGuid().ToString() },
                ShortName = new TextType[] { new TextType{Value = "Info Source Example"} }
            },
            Type = new EffectiveStatusType() {
                UUID = new UUID(true),
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

// = BOD Processing Example (using the LINQ XML API)
// General approach would be:
// - encapsulate the generic BOD header processing in a module of the CCOM library
// - use dependency injection to insert the BOD specific processing method or class to process the content of each noun.
// - this processing can leverage the CCOM seralisation/deserialisation to access the CCOM elements.
Console.WriteLine("Reading/Writing example CCOM BOD data");


// some constants and helpers
XElement null_element = new XElement("Null");
XNamespace XSI = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
XName XSI_TYPE = XSI + "type";
XNamespace OA_NAMESPACE = XNamespace.Get("http://www.openapplications.org/oagis/9");
XName OA_BODID = OA_NAMESPACE + "BODID";
XName OA_SENDER = OA_NAMESPACE + "Sender";
XName OA_LOGICAL_ID = OA_NAMESPACE + "LogicalID";
XName OA_APPLICATION_AREA = OA_NAMESPACE + "ApplicationArea";
XNamespace CCOM_XNAMESPACE = XNamespace.Get("http://www.mimosa.org/ccom4");
XName CCOM_BOD_DATA_AREA = CCOM_XNAMESPACE + "DataArea";

Dictionary<string, string> knownSourceSystems = new Dictionary<string, string>(
    new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("ba201587-fd83-4d8a-acb3-426ac0c0b9f3", "Owner/Operator SDAIR") }
);

// read and process the BOD
XElement? bodXml = null;
using (TextReader reader = new StreamReader("./data/example_bod_sync_segments.xml")) {
    bodXml = XElement.Load(reader);
}

// Is there a BOD (identified by having an oa:ApplicationArea)?
XElement? applicationArea = bodXml.Element(OA_APPLICATION_AREA);
if (applicationArea == null) {
    Console.WriteLine("No BOD found: {0}", bodXml.Name);
    return;
}
string bodID = applicationArea.Elements(OA_BODID).FirstOrDefault(null_element).Value;
string senderId = applicationArea.Elements(OA_SENDER).FirstOrDefault(null_element).Elements(OA_LOGICAL_ID).FirstOrDefault(null_element).Value;

Console.WriteLine("BOD '{0}' (ID: {1}) received from '{2}' (ID: {3})", bodXml.Name, bodID, knownSourceSystems[senderId], senderId);

// Is the BOD valid? I.e., does it have a DataArea?
XElement? dataArea = bodXml.Element(CCOM_BOD_DATA_AREA);
if (dataArea == null) {
    Console.WriteLine("BOD invaild: no data area");
    return;
}

// Note: this transformation function of an entity element to a CCOMData document
// is a quick hack as I have not yet processed the CCOM BOD schema to support the 
// top-level CCOM elements such as <Segment></Segment>. Normal CCOM schema only
// allows the generic <Entity xsi:type="Segment">, for example, at the top-level.
Func<XElement, XElement> transform = element => 
    new XElement(CCOM_XNAMESPACE + "CCOMData",
        new XAttribute(XNamespace.Xmlns + "ccom", CCOM_NAMESPACE),
        new XElement(CCOM_XNAMESPACE + "Entity", 
            new XAttribute(XSI_TYPE, "ccom:" + element.Name.LocalName),
            element.Elements()
        )
    );
CCOMData none = new CCOMData();

// An example, fake, 'Processing Function'
Action<Entity> updateEntity = entity => {
    Console.WriteLine("Updating entity: {0}({1} => {2})", entity.GetType(), entity.UUID.Value, entity.IDInInfoSource.Value);
    // Do some real processing here.
    if (entity.GetType().Equals(typeof(Segment))) {
        Segment? s = entity as Segment;
        Console.WriteLine("\tShort Name: {0}", s?.ShortName.Select((text, i) => text.Value).ToArray());
    }
};

// Extract the Verb from the BOD
XElement verb = dataArea.Elements().First();

// Extract the Nouns from the BOD
IEnumerable<XElement> nouns = from noun in dataArea.Elements()
                    where noun.Name.NamespaceName != OA_NAMESPACE
                    select noun;

// Process the content of each 
foreach (XElement noun in nouns) {
    Console.WriteLine("Processing noun: {0}", noun.Name);
    Console.WriteLine("Would defer to some kind of processing method or class {0}", noun.Elements().First().Name.LocalName);

    IEnumerable<Entity> entities = from e in noun.Elements()
                                   select (s.Deserialize(transform(e).CreateReader()) as CCOMData ?? none).Entity.First();
    foreach (Entity entity in entities) {
        // Some means of selecting or having been passed in the processing method
        switch (verb.Name.LocalName) {
        case "Sync":
            updateEntity(entity);
            break;
        default:
            Console.WriteLine("No registered action");
            break;
        }
    }
}
