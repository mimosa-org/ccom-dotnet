using CommonBOD;
using Oagis;
using System.Xml;
using System.Xml.Linq;
using CCOM.Net.Test.Fixture;

namespace CCOM.Net.Test;

public class GenericBodTypeTest : IClassFixture<BODExamples>
{
    BODExamples examples;

    public GenericBodTypeTest(BODExamples fixture)
    {
        this.examples = fixture;
    }

    [Theory]
    [InlineData("")]
    [InlineData("fake")]
    public void NamespacesTest(string expectedPrefix)
    {
        var expected = "https://example.com/GetBOD";
        var expectedNamespaces = new HashSet<XmlQualifiedName>(new[] {
            new XmlQualifiedName(expectedPrefix, expected),
            new XmlQualifiedName("oa", Oagis.Namespace.URI)
        });
        
        var bod = new GenericBodType<GetType, BODType>("GetBOD", expected, expectedPrefix);

        Assert.Equal(expected, bod.Namespace);
        Assert.Equal(expectedNamespaces, bod.Namespaces.ToArray().ToHashSet());
    }

    [Fact]
    public void SerializeToDocumentTest()
    {
        var (bodId, senderId, creationDateTime) = examples.GenerateApplicationAreaFields();
        var expected = XDocument.Parse(examples.ConfirmBOD(bodId, senderId, creationDateTime));

        var bod = examples.ConfirmBODAsGenericValue(bodId, senderId, creationDateTime);

        Assert.Equal(expected, bod.SerializeToDocument(), new XNodeEqualityComparer());
    }

    [Fact]
    public void SerializeToStringTest()
    {
        var (bodId, senderId, creationDateTime) = examples.GenerateApplicationAreaFields();
        var expected = examples.ConfirmBOD(bodId, senderId, creationDateTime);

        var bod = examples.ConfirmBODAsGenericValue(bodId, senderId, creationDateTime);

        Assert.Equal(expected, bod.SerializeToString());
    }

    [Fact]
    public void SerializeToDocumentWithNounNameTest()
    {
        var (bodId, senderId, creationDateTime) = examples.GenerateApplicationAreaFields();
        var expected = XDocument.Parse(examples.ConfirmBODNounRenamed(bodId, senderId, creationDateTime));

        var bod = examples.ConfirmBODAsGenericValue(bodId, senderId, creationDateTime, nounName: "BODResult");

        Assert.Equal(expected, bod.SerializeToDocument(), new XNodeEqualityComparer());
    }

    [Fact]
    public void SerializeToStringWithNounNameTest()
    {
        var (bodId, senderId, creationDateTime) = examples.GenerateApplicationAreaFields();
        var expected = examples.ConfirmBODNounRenamed(bodId, senderId, creationDateTime);

        var bod = examples.ConfirmBODAsGenericValue(bodId, senderId, creationDateTime, nounName: "BODResult");

        Assert.Equal(expected, bod.SerializeToString());
    }

    [Fact]
    public void DeserializeTest()
    {
        var (bodId, senderId, creationDateTime) = examples.GenerateApplicationAreaFields();
        var doc = XDocument.Parse(examples.ConfirmBOD(bodId, senderId, creationDateTime));

        var expected = examples.ConfirmBODAsGenericValue(bodId, senderId, creationDateTime);

        var bod = new GenericBodType<ConfirmType, List<BODType>>(doc, "ConfirmBOD", Oagis.Namespace.URI);

        Assert.NotEmpty(bod.DataArea.Noun);
        // Assert.Equivalent(expected, bod); // Cannot use, does not appear to handle overridden properties correctly
        Assert.Equal(doc.ToString(), bod.SerializeToString());
    }

    [Fact]
    public void DeserializeWithNounNameTest()
    {
        var (bodId, senderId, creationDateTime) = examples.GenerateApplicationAreaFields();
        var doc = XDocument.Parse(examples.ConfirmBODNounRenamed(bodId, senderId, creationDateTime));

        var expected = examples.ConfirmBODAsGenericValue(bodId, senderId, creationDateTime);

        var bod = new GenericBodType<ConfirmType, List<BODType>>(doc, "ConfirmBOD", Oagis.Namespace.URI, nounName: "BODResult");

        Assert.NotEmpty(bod.DataArea.Noun);
        // Assert.Equivalent(expected, bod); // Cannot use, does not appear to handle overridden properties correctly
        Assert.Equal(doc.ToString(), bod.SerializeToString());
    }

    [Fact]
    public void DeserializeSubclassConstructorTest()
    {
        var doc = XDocument.Parse(examples.SyncSegments());

        var bod = new SyncSegmentsBODType(doc);

        Assert.Single(bod.Segments);
        Assert.Equal(3, bod.Segments.First().Segment.Count());
        Assert.Equal("e766da80-9453-0137-32bf-22000b499058", bod.DataArea.Noun.First().Segment.First().UUID.Value);
    }

    [Fact]
    public void DeserializeSubclassStaticMethodTest()
    {
        var doc = XDocument.Parse(examples.SyncSegments());

        var bod = BusinessObjectDocumentType.Deserialize<SyncSegmentsBODType>(doc);

        Assert.NotNull(bod);
        Assert.Single(bod.Segments);
        Assert.Equal(3, bod.Segments.First().Segment.Count());
        Assert.Equal("e766da80-9453-0137-32bf-22000b499058", bod.DataArea.Noun.First().Segment.First().UUID.Value);
    }
}
