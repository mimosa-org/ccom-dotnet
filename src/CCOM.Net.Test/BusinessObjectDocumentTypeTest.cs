using CommonBOD;
using Oagis;
using System.Xml;
using System.Xml.Linq;

namespace CCOM.Net.Test;

public class BusinessObjectDocumentTypeTest
{

    [Fact]
    public void NamespacesTest()
    {
        var bod = new StubBOD();
        var expected = "http://example.com/StubBOD";
        var expectedNamespaces = new HashSet<XmlQualifiedName>(new[] {
            new XmlQualifiedName("", expected),
            new XmlQualifiedName("oa", Oagis.Namespace.URI)
        });

        Assert.Equal(expected, bod.Namespace);
        Assert.Equal(expectedNamespaces, bod.Namespaces.ToArray().ToHashSet());
    }

    [Fact]
    public void NamespaceIsImmutableTest()
    {
        Assert.Throws<Exception>(
            () => new StubBOD() { Namespace = "something else" }
        );
    }

    [Fact]
    public void ConfirmBODNamespacesTest()
    {
        var bod = new ConfirmBODType();
        var expected = Oagis.Namespace.URI;
        var expectedNamespaces = new HashSet<XmlQualifiedName>(new[] {
            new XmlQualifiedName("", expected),
        });

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

        var bod = new ConfirmBODType
        {
            releaseID = "9.0",
            languageCode = "en-AU",
            ApplicationArea = new ApplicationAreaType
            {
                BODID = new IdentifierType { Value = bodId },
                CreationDateTime = creationDateTime.ToXsDateTimeString(),
                Sender = new SenderType
                {
                    LogicalID = new IdentifierType { Value = senderId }
                }
            }
        };

        Assert.Equal(expected, bod.SerializeToDocument(), new XNodeEqualityComparer());
    }

    [Fact]
    public void SerializeToStringTest()
    {
        var bodId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var creationDateTime = DateTime.UtcNow;
        var expected = ConfirmBODExample(bodId, senderId, creationDateTime);

        var bod = new ConfirmBODType
        {
            releaseID = "9.0",
            languageCode = "en-AU",
            ApplicationArea = new ApplicationAreaType
            {
                BODID = new IdentifierType { Value = bodId },
                CreationDateTime = creationDateTime.ToXsDateTimeString(),
                Sender = new SenderType
                {
                    LogicalID = new IdentifierType { Value = senderId }
                }
            }
        };

        Assert.Equal(expected, bod.SerializeToString());
    }

    [Fact]
    public void DeserializeConfirmBODTest()
    {
        var bodId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var creationDateTime = DateTime.UtcNow;
        var doc = XDocument.Parse(ConfirmBODExample(bodId, senderId, creationDateTime));

        var expected = new ConfirmBODType
        {
            releaseID = "9.0",
            languageCode = "en-AU",
            ApplicationArea = new ApplicationAreaType
            {
                BODID = new IdentifierType { Value = bodId },
                CreationDateTime = creationDateTime.ToXsDateTimeString(),
                Sender = new SenderType
                {
                    LogicalID = new IdentifierType { Value = senderId }
                }
            }
        };

        var bod = BusinessObjectDocumentType.Deserialize<ConfirmBODType>(doc);

        Assert.Equivalent(expected, bod);
    }

    public string ConfirmBODExample(string bodid, string senderId, DateTime creationTime)
    {
        return $@"<ConfirmBOD releaseID=""9.0"" languageCode=""en-AU"" xmlns=""http://www.openapplications.org/oagis/9"">
  <ApplicationArea>
    <Sender>
      <LogicalID>{senderId}</LogicalID>
    </Sender>
    <CreationDateTime>{creationTime.ToXsDateTimeString()}</CreationDateTime>
    <BODID>{bodid}</BODID>
  </ApplicationArea>
</ConfirmBOD>";
    }
}
