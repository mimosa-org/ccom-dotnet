using Oagis;
using System.Xml;
using System.Xml.Serialization;

namespace CommonBOD;

[Serializable]
public class GenericBodType<TVerb, TNoun> : BusinessObjectDocumentType
    where TVerb : VerbType, new()
    where TNoun : class, new()
{
    [XmlIgnore]
    public string Name { get; init; }
    [XmlIgnore]
    public string Namespace { get; init; }
    [XmlIgnore]
    public string Prefix { get; init; }

    public GenericDataAreaType<TVerb, TNoun> DataArea { get; set; }

    public GenericBodType()
    {
        // TODO: how to ensure we capture the bod info on Deserialization?
        Name = "";
        Namespace = "";
        Prefix = "";
        DataArea = new GenericDataAreaType<TVerb, TNoun>();
    }

    public GenericBodType(string name, string ns, string prefix = "")
    {
        Name = name;
        Namespace = ns;
        Prefix = prefix;
        DataArea = new GenericDataAreaType<TVerb, TNoun>();
    }

    public XmlSerializer CreateSerializer(Type[]? extraTypes = null)
    {
        return new XmlSerializer(this.GetType(), XmlAttributeOverrides, extraTypes, null, Namespace);
    }

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
                new XmlElementAttribute(typeof(TNoun).GenericTypeArguments[0].Name.Replace("Type", ""), typeof(TNoun).GenericTypeArguments[0]) { Namespace = Namespace }
                : new XmlElementAttribute(typeof(TNoun).Name.Replace("Type", ""), typeof(TNoun)) { Namespace = Namespace }
            );
            attrOverrides.Add(DataArea.GetType(), "Noun", attributes);

            return attrOverrides;
        }
    }

    public XmlSerializerNamespaces Namespaces
    {
        get
        {
            return new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("oa", Oagis.Namespace.URI),
                new XmlQualifiedName(Prefix, Namespace),
            });
        }
    }
}
