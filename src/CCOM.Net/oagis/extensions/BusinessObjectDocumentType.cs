using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Ccom.Xml.Serialization;

namespace Oagis;

public partial class BusinessObjectDocumentType
{
    /// <summary>
    /// Deserialize a BOD to the given type (TBOD). An optional factory function is used to
    /// obtain an instance of a pre-configured XmlSerializer.
    /// </summary>
    /// <typeparam name="TBOD">Concrete type of BusinessObjectDocumentType expected to be deserialized</typeparam>
    /// <param name="document">The XML Document to deserialize</param>
    /// <param name="serializerFactory">Factory function used to obtain a pre-configured XmlSerializer</param>
    /// <returns>A BOD of the appopriate type or null</returns>
    public static TBOD? Deserialize<TBOD>(XDocument document, Func<XmlSerializer>? serializerFactory = null)
        where TBOD : BusinessObjectDocumentType, new()
    {
        if (serializerFactory is null) return Deserialize<TBOD>(document, new TBOD().GetSerializer);

        using (var reader = document.CreateReader())
        {
            return serializerFactory().Deserialize(reader) as TBOD;
        }
    }

    public XDocument SerializeToDocument()
    {
        var doc = new XDocument();
        using (var writer = doc.CreateWriter())
        {
            GetSerializer().Serialize(writer, this, Namespaces);
        }
        return doc;
    }

    public string SerializeToString(bool pretty = true)
    {
        return SerializeToDocument().ToString(pretty ? SaveOptions.None : SaveOptions.DisableFormatting);
    }

    virtual public XmlSerializer GetSerializer()
    {
        return new XmlCallbackSerializer(this.GetType(), Namespace);
    }

    [JsonIgnore]
    [XmlIgnore]
    virtual public XmlSerializerNamespaces Namespaces
    {
        get => new XmlSerializerNamespaces(new[] {
            new XmlQualifiedName("", Namespace),
            new XmlQualifiedName("oa", Oagis.Namespace.URI)
        });
    }

    [JsonIgnore]
    [XmlIgnore]
    virtual public string Namespace
    {
        get => this.GetType().GetCustomAttributes(true).Where(a => a is XmlTypeAttribute).Cast<XmlTypeAttribute>().FirstOrDefault()?.Namespace ?? "";
        init => throw new Exception("Unable to assign explicit Namespace");
    }
}