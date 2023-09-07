using System.Xml.Serialization;
using System.Xml.Linq;
using Ccom;
using Oagis;

namespace CCOM.Net.Test;

public class OagisSerializationTest
{
    const string DATA_PATH = "./../../../../../data";

    const string BOD_SCHEMA_PATH = "./../../../../../XSD/BOD/Messages";

    [Fact]
    public void ReadApplicationBOD()
    {
        var expected = new ApplicationAreaType()
        {
            BODID = new IdentifierType() { Value = "11c696bc-2a2c-4c13-bff5-1c97d55494ba" },
            CreationDateTime = "2019-09-13T04:21:21Z",
            Sender = new SenderType()
            {
                LogicalID = new IdentifierType() { Value = "ba201587-fd83-4d8a-acb3-426ac0c0b9f3" }
            }
        };

        // XmlSerializer s = new XmlSerializer(typeof(CCOMData), Ccom.Namespace.URI);
        XElement? bodXml = null;
        using (TextReader reader = new StreamReader($"{DATA_PATH}/example_bod_sync_segments.xml"))
        {
            bodXml = XElement.Load(reader);
            // bodXml = s.Deserialize(reader) as CCOMData;
            Assert.NotNull(bodXml);
        }

        Assert.Equal("SyncSegments", bodXml.Name.LocalName);

        var o = new XmlSerializer(typeof(ApplicationAreaType)).Deserialize(bodXml.Elements().First().CreateReader()) as ApplicationAreaType;
        Assert.Equal(expected.BODID.Value, o?.BODID.Value);
        Assert.Equal(expected.CreationDateTime, o?.CreationDateTime);
        Assert.Equal(expected.Sender.LogicalID.Value, o?.Sender.LogicalID.Value);

        var dataArea = bodXml.Element(Ccom.Namespace.XNAMESPACE + "DataArea");
        Assert.NotNull(dataArea);
        Assert.Equal(Ccom.Namespace.XNAMESPACE, dataArea.Name.Namespace);

        var verb = new XmlSerializer(typeof(SyncType)).Deserialize(dataArea.Elements().First().CreateReader()) as VerbType;
        Assert.IsType<SyncType>(verb);

        var nouns = from noun in dataArea.Elements()
                    where noun.Name.NamespaceName != Oagis.Namespace.URI
                    select noun;

        // Have not yet decided how to dynamically handle Noun content
        Assert.True(nouns.Count() == 1);
    }

    [Fact]
    public void ReadWriteConfirmBOD()
    {
        ConfirmBODType confirmBOD = new ConfirmBODType()
        {
            languageCode = "en",
            releaseID = "9.0",
            ApplicationArea = new ApplicationAreaType()
            {
                BODID = new IdentifierType()
                {
                    Value = "an Identifier"
                },
                CreationDateTime = DateTime.UtcNow.ToString()
            },
            DataArea = new ConfirmBODDataAreaType()
            {
                Confirm = new ConfirmType(),
                BOD = new BODType[] {
                    new BODType() {
                        BODSuccessMessage = new BODSuccessMessageType()
                        {
                            NounSuccessMessage = new SuccessMessageType[] {
                                new SuccessMessageType()
                                {
                                    ProcessMessage = new MessageType[] {
                                        new MessageType() {
                                            Description = new DescriptionType[] {
                                                new DescriptionType() {Value = "The BOD was processed successfully"}
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
        };

        var serializer = new XmlSerializer(typeof(ConfirmBODType));
        var xml = new StringWriter();
        serializer.Serialize(xml, confirmBOD);

        var readBOD = serializer.Deserialize(new StringReader(xml.GetStringBuilder().ToString())) as ConfirmBODType;
        Assert.NotNull(readBOD);
        Assert.Equal(confirmBOD.languageCode, readBOD.languageCode);
        Assert.Equal(
            confirmBOD.DataArea.BOD[0].BODSuccessMessage.NounSuccessMessage[0].ProcessMessage[0].Description[0].Value,
            readBOD.DataArea.BOD[0].BODSuccessMessage.NounSuccessMessage[0].ProcessMessage[0].Description[0].Value
        );
    }

    [Fact]
    public void ReadApplicationBODWithConfirmationCodeSerializationTest()
    {
        XElement? bodXml = null;
        using (TextReader reader = new StreamReader($"{DATA_PATH}/example_bod_sync_segments.xml"))
        {
            bodXml = XElement.Load(reader);
            Assert.NotNull(bodXml);
        }
        var expected = new ApplicationAreaType()
        {
            BODID = new IdentifierType() { Value = "11c696bc-2a2c-4c13-bff5-1c97d55494ba" },
            CreationDateTime = "2019-09-13T04:21:21Z",
            Sender = new SenderType()
            {
                LogicalID = new IdentifierType() { Value = "ba201587-fd83-4d8a-acb3-426ac0c0b9f3" },
                ConfirmationCode = new ConfirmationResponseCodeType() { Value = "Always"}
            }
        };

        var o = new XmlSerializer(typeof(ApplicationAreaType)).Deserialize(bodXml.Elements().First().CreateReader()) as ApplicationAreaType;

        Assert.Equal(expected.BODID.Value, o?.BODID.Value);
        Assert.Equal(expected.CreationDateTime, o?.CreationDateTime);
        Assert.Equal(expected.Sender.LogicalID.Value, o?.Sender.LogicalID.Value);
        Assert.Equal(expected.Sender.ConfirmationCode.Value, o?.Sender.ConfirmationCode.Value);
    }

}