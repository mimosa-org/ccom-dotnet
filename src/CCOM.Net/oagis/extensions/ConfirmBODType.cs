using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Oagis;

public partial class ConfirmBODType : BusinessObjectDocumentType
{
    public override XmlSerializerNamespaces Namespaces
    {
        get => new XmlSerializerNamespaces(new[] {new XmlQualifiedName("", Oagis.Namespace.URI)});
    }
}