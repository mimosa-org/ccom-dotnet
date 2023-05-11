using Oagis;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CommonBOD;

/// <summary>
/// Marker interface that can be used when the generic parameters are inaccessible.
/// </summary>
public interface GenericBodType
{
}

/// <summary>
/// BOD Type that can represent any BOD using generics to specify the VerbType and 
/// Noun content type.
/// </summary>
/// <typeparam name="TVerb">The VerbType of the BOD (e.g., GetType, ShowType, SyncType)</typeparam>
/// <typeparam name="TNoun">The the type of content comprising the nouns of the DataArea</typeparam>
[Serializable]
public class GenericBodType<TVerb, TNoun> : BusinessObjectDocumentType, GenericBodType
    where TVerb : VerbType, new()
    where TNoun : class, new()
{
    [JsonIgnore]
    [XmlIgnore]
    public string Name { get; init; }
    [JsonIgnore]
    [XmlIgnore]
    public override string Namespace { get; init; }
    [JsonIgnore]
    [XmlIgnore]
    public string Prefix { get; init; }
    [JsonIgnore]
    [XmlIgnore]
    public string? NounName { get; init; }

    public GenericDataAreaType<TVerb, TNoun> DataArea { get; set; }

    public GenericBodType()
    {
        Name = "";
        Namespace = "";
        Prefix = "";
        NounName = null;
        DataArea = new GenericDataAreaType<TVerb, TNoun>();
    }

    public GenericBodType(string name, string ns, string prefix = "", string? nounName = null)
    {
        Name = name;
        Namespace = ns;
        Prefix = prefix;
        NounName = nounName;
        DataArea = new GenericDataAreaType<TVerb, TNoun>();
    }

    public GenericBodType(XDocument source, string name, string ns, string prefix = "", string? nounName = null)
        : this(name, ns, prefix, nounName)
    {
        var other = Deserialize<GenericBodType<TVerb, TNoun>>(source, GetSerializer);
        if (other is not null)
        {
            languageCode = other.languageCode;
            releaseID = other.releaseID;
            versionID = other.versionID;
            systemEnvironmentCode = other.systemEnvironmentCode;
            ApplicationArea = other.ApplicationArea;
            DataArea = other.DataArea;
        }
    }

    public override XmlSerializer GetSerializer()
    {
        return CreateSerializer();
    }

    public XmlSerializer CreateSerializer(Type[]? extraTypes = null)
    {
        return new XmlSerializer(this.GetType(), XmlAttributeOverrides, extraTypes, null, Namespace);
    }

    [JsonIgnore]
    [XmlIgnore]
    public XmlAttributeOverrides XmlAttributeOverrides
    {
        get
        {
            var attrOverrides = new XmlAttributeOverrides();
            var attributes = new XmlAttributes();
            attributes.XmlType = new XmlTypeAttribute(Name) { Namespace = Namespace };
            attrOverrides.Add(this.GetType(), "", attributes );

            attributes = new XmlAttributes();
            attributes.XmlElements.Add(
                new XmlElementAttribute("DataArea", DataArea.GetType()) { Namespace = Namespace }
            );
            attrOverrides.Add(this.GetType(), "DataArea", attributes );

            // Data Area overrides
            attributes = new XmlAttributes();
            attributes.XmlElements.Add(
                new XmlElementAttribute(typeof(TVerb).Name.Replace("Type", ""), typeof(TVerb)) { Namespace = Oagis.Namespace.URI }
            );
            attrOverrides.Add(DataArea.GetType(), "Verb", attributes);

            // Noun overrides
            attributes = new XmlAttributes();
            attributes.XmlElements.Add(
                typeof(TNoun).IsAssignableTo(typeof(System.Collections.IEnumerable)) ?
                new XmlElementAttribute(NounName ?? Name.Replace(typeof(TVerb).Name.Replace("Type", ""), ""), typeof(TNoun).GenericTypeArguments[0]) { Namespace = Namespace }
                : new XmlElementAttribute(NounName ?? Name.Replace(typeof(TVerb).Name.Replace("Type", ""), ""), typeof(TNoun)) { Namespace = Namespace }
            );
            attrOverrides.Add(DataArea.GetType(), "Noun", attributes);

            return attrOverrides;
        }
    }

    [JsonIgnore]
    [XmlIgnore]
    override public XmlSerializerNamespaces Namespaces
    {
        get
        {
            return new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("oa" == Prefix ? "oa1" : "oa", Oagis.Namespace.URI),
                new XmlQualifiedName(Prefix, Namespace),
            });
        }
    }
}
