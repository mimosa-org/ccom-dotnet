using System.Xml;
using System.Xml.Linq;

namespace Ccom;
using Xml.Serialization;

public partial class XML
{
    private XmlNode[] anyContent = null!;

    /// <remarks>
    /// Added manually to support the anyContent of this element.
    /// </remarks>
    [System.Xml.Serialization.XmlAnyElement()]
    public XmlNode[] AnyContent
    {
        get
        {
            return this.anyContent;
        }
        set
        {
            this.anyContent = value;
        }
    }

    public IEnumerable<T> GetContent<T>() where T : class
    {
        var serializer = new XmlCallbackSerializer(typeof(T), Ccom.Namespace.URI);
        return AnyContent.Select(e =>
        {
            var reader = new XmlNodeReader(e);
            return serializer.Deserialize(reader);
        }).OfType<T>();
    }

    public XML SetContent<T>(IEnumerable<T> items) where T : class
    {
        var serializer = new XmlCallbackSerializer(typeof(T), Ccom.Namespace.URI);
        AnyContent = items.Select(e =>
        {
            var doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                serializer.Serialize(writer, e);
            }
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(doc.CreateReader());
            return xmlDoc.FirstChild;
        }).OfType<XmlNode>().ToArray();
        return this;
    }
}
