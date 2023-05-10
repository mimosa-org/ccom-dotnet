using CommonBOD;
using Oagis;
using System.Xml;
using System.Xml.Linq;

namespace CCOM.Net.Test;

public class GenericBodTypeTest
{

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
        var bodId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var creationDateTime = DateTime.UtcNow;
        var expected = XDocument.Parse(ConfirmBODExample(bodId, senderId, creationDateTime));

        var bod = ConfirmBODTypeExample(bodId, senderId, creationDateTime);

        Assert.Equal(expected, bod.SerializeToDocument(), new XNodeEqualityComparer());
    }

    [Fact]
    public void SerializeToStringTest()
    {
        var bodId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var creationDateTime = DateTime.UtcNow;
        var expected = ConfirmBODExample(bodId, senderId, creationDateTime);

        var bod = ConfirmBODTypeExample(bodId, senderId, creationDateTime);

        Assert.Equal(expected, bod.SerializeToString());
    }

    [Fact]
    public void SerializeToDocumentWithNounNameTest()
    {
        var bodId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var creationDateTime = DateTime.UtcNow;
        var expected = XDocument.Parse(ConfirmBODNounExample(bodId, senderId, creationDateTime));

        var bod = ConfirmBODTypeExample(bodId, senderId, creationDateTime, nounName: "BODResult");

        Assert.Equal(expected, bod.SerializeToDocument(), new XNodeEqualityComparer());
    }

    [Fact]
    public void SerializeToStringWithNounNameTest()
    {
        var bodId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var creationDateTime = DateTime.UtcNow;
        var expected = ConfirmBODNounExample(bodId, senderId, creationDateTime);

        var bod = ConfirmBODTypeExample(bodId, senderId, creationDateTime, nounName: "BODResult");

        Assert.Equal(expected, bod.SerializeToString());
    }

    [Fact]
    public void DeserializeTest()
    {
        var bodId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var creationDateTime = DateTime.UtcNow;
        var doc = XDocument.Parse(ConfirmBODExample(bodId, senderId, creationDateTime));

        var expected = ConfirmBODTypeExample(bodId, senderId, creationDateTime);

        var bod = new GenericBodType<ConfirmType, List<BODType>>(doc, "ConfirmBOD", Oagis.Namespace.URI);

        Assert.NotEmpty(bod.DataArea.Noun);
        // Assert.Equivalent(expected, bod); // Cannot use, does not appear to handle overridden properties correctly
        Assert.Equal(doc.ToString(), bod.SerializeToString());
    }

    [Fact]
    public void DeserializeWithNounNameTest()
    {
        var bodId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var creationDateTime = DateTime.UtcNow;
        var doc = XDocument.Parse(ConfirmBODNounExample(bodId, senderId, creationDateTime));

        var expected = ConfirmBODTypeExample(bodId, senderId, creationDateTime);

        var bod = new GenericBodType<ConfirmType, List<BODType>>(doc, "ConfirmBOD", Oagis.Namespace.URI, nounName: "BODResult");

        Assert.NotEmpty(bod.DataArea.Noun);
        // Assert.Equivalent(expected, bod); // Cannot use, does not appear to handle overridden properties correctly
        Assert.Equal(doc.ToString(), bod.SerializeToString());
    }

    public GenericBodType<ConfirmType, List<BODType>> ConfirmBODTypeExample(string bodid, string senderId, DateTime creationTime, string? nounName = null)
    {
        return new GenericBodType<ConfirmType, List<BODType>>("ConfirmBOD", Oagis.Namespace.URI, nounName: nounName)
        {
            releaseID = "9.0",
            languageCode = "en-AU",
            ApplicationArea = new ApplicationAreaType
            {
                BODID = new IdentifierType { Value = bodid },
                CreationDateTime = creationTime.ToXsDateTimeString(),
                Sender = new SenderType
                {
                    LogicalID = new IdentifierType { Value = senderId }
                }
            },
            DataArea = new GenericDataAreaType<ConfirmType, List<BODType>>
            {
                Verb = new ConfirmType(),
                Noun = new List<BODType>()
                {
                    new BODType
                    {
                        Description = new[] {
                            new DescriptionType { Value = "Example" }
                        }
                    }
                }
            }
        };
    }

    public string ConfirmBODExample(string bodid, string senderId, DateTime creationTime)
    {
        return $@"<oa:ConfirmBOD releaseID=""9.0"" languageCode=""en-AU"" xmlns:oa=""http://www.openapplications.org/oagis/9"">
  <oa:ApplicationArea>
    <oa:Sender>
      <oa:LogicalID>{senderId}</oa:LogicalID>
    </oa:Sender>
    <oa:CreationDateTime>{creationTime.ToXsDateTimeString()}</oa:CreationDateTime>
    <oa:BODID>{bodid}</oa:BODID>
  </oa:ApplicationArea>
  <oa:DataArea>
    <oa:Confirm />
    <oa:BOD>
      <oa:Description>Example</oa:Description>
    </oa:BOD>
  </oa:DataArea>
</oa:ConfirmBOD>";
    }

    public string ConfirmBODNounExample(string bodid, string senderId, DateTime creationTime)
    {
        return $@"<oa:ConfirmBOD releaseID=""9.0"" languageCode=""en-AU"" xmlns:oa=""http://www.openapplications.org/oagis/9"">
  <oa:ApplicationArea>
    <oa:Sender>
      <oa:LogicalID>{senderId}</oa:LogicalID>
    </oa:Sender>
    <oa:CreationDateTime>{creationTime.ToXsDateTimeString()}</oa:CreationDateTime>
    <oa:BODID>{bodid}</oa:BODID>
  </oa:ApplicationArea>
  <oa:DataArea>
    <oa:Confirm />
    <oa:BODResult>
      <oa:Description>Example</oa:Description>
    </oa:BODResult>
  </oa:DataArea>
</oa:ConfirmBOD>";
    }
}
