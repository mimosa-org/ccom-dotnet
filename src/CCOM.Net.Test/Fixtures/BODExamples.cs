using CommonBOD;
using Oagis;

namespace CCOM.Net.Test.Fixture;

public class BODExamples : IDisposable
{
    const string DATA_PATH = "./../../../../../data";

    public void Dispose()
    {
        // Do nothing
    }

    public (string BodId, string SenderId, DateTime CreationDateTime) GenerateApplicationAreaFields()
    {
        return (Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.UtcNow);
    }

    public GenericBodType<ConfirmType, List<BODType>> ConfirmBODAsGenericValue(string bodid, string senderId, DateTime creationTime, string? nounName = null)
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

    public string ConfirmBOD(string bodid, string senderId, DateTime creationTime)
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

    public string ConfirmBODNounRenamed(string bodid, string senderId, DateTime creationTime)
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

    public string SyncSegments()
    {
        return File.ReadAllText($"{DATA_PATH}/example_bod_sync_segments.xml");
    }
}