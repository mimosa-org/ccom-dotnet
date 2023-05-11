using System.Xml.Serialization;

namespace Ccom;

/// <summary>
/// Example CCOM Noun for Segments in, e.g., SyncSegments.
/// </summary>
[XmlType(Namespace = Ccom.Namespace.URI)]
public class Segments
{
    [XmlElement("RegistrationSite")]
    public Site[] RegistrationSite { get; set; } = new Site[] { };

    [XmlElement("Segment")]
    public Segment[] Segment { get; set; } = new Segment[] { };
}