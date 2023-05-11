using Ccom;
using CommonBOD;
using Oagis;
using System.Xml.Linq;

namespace CCOM.Net.Test;

public class SyncSegmentsBODType : GenericBodType<SyncType, List<Segments>>
{
    public Segments[] Segments { get => DataArea.Noun.ToArray(); }

    public SyncSegmentsBODType() : base("SyncSegments", Ccom.Namespace.URI, "", "Segments")
    {
    }

    public SyncSegmentsBODType(XDocument source) : base(source, "SyncSegments", Ccom.Namespace.URI, "", "Segments")
    {
    }
}