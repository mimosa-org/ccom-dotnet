using System.Xml.Schema;
using CommonBOD;
using Oagis;

namespace CCOM.Net.Test;

public class BODReaderTest
{
    const string DATA_PATH = "./../../../../../data";
    const string BASE_SCHEMA_PATH = "./../../../../../XSD";
    const string BOD_SCHEMA_PATH = $"{BASE_SCHEMA_PATH}/BOD/Messages";

    readonly BODReaderSettings settings = new BODReaderSettings()
    {
        SchemaPath = BASE_SCHEMA_PATH
    };

    [Fact]
    public void BodSchemaIsValidTest()
    {
        BODReader reader = new BODReader(
            $"{DATA_PATH}/example_bod_sync_segments.xml",
            $"{BOD_SCHEMA_PATH}/Configuration/SyncSegments.xsd",
            settings
        );

        Assert.True(reader.IsValid);
        Assert.Empty(reader.ValidationErrors);
        Assert.Equal("SyncSegments", reader.SimpleName);
    }

    [Fact]
    public void BodSchemaInvalidTest()
    {
        var expectedError = new ValidationResult(XmlSeverityType.Error, "The 'http://www.mimosa.org/ccom4:SyncSegments' element is not declared.", 3, 2);

        BODReader reader = new BODReader(
            $"{DATA_PATH}/example_bod_sync_segments.xml",
            $"{BOD_SCHEMA_PATH}/Configuration/SyncAssets.xsd",
            settings
        );

        Assert.False(reader.IsValid);
        Assert.NotEmpty(reader.ValidationErrors);
        Assert.Equal(expectedError, reader.ValidationErrors.First());
        Assert.Equal("SyncSegments", reader.SimpleName);
    }

    [Fact]
    public void GenerateConfirmBODForValidBODTest()
    {
        var expected = new ConfirmBODType()
        {
            languageCode = "en-US",
            releaseID = "9.0",
            systemEnvironmentCode = "Production",
            versionID = null,
            ApplicationArea = new ApplicationAreaType()
            {
                BODID = new IdentifierType()
                {
                    Value = "an Identifier" // This won't match
                },
                CreationDateTime = DateTime.UtcNow.ToString() // now will this
            },
            DataArea = new ConfirmBODDataAreaType()
            {
                Confirm = new ConfirmType()
                {
                    OriginalApplicationArea = new ApplicationAreaType()
                    {
                        BODID = new IdentifierType() { Value = "11c696bc-2a2c-4c13-bff5-1c97d55494ba" },
                        CreationDateTime = "2019-09-13T04:21:21Z",
                        Sender = new SenderType()
                        {
                            LogicalID = new IdentifierType() { Value = "ba201587-fd83-4d8a-acb3-426ac0c0b9f3" }
                        }
                    }
                },
                BOD = new BODType[] {
                    new BODType() {
                        BODSuccessMessage = new BODSuccessMessageType()
                    }
                }
            },
        };

        BODReader reader = new BODReader(
            $"{DATA_PATH}/example_bod_sync_segments.xml",
            $"{BOD_SCHEMA_PATH}/Configuration/SyncSegments.xsd",
            settings
        );

        var confirmBOD = reader.GenerateConfirmBOD();

        Assert.Equal(expected.languageCode, confirmBOD.languageCode);
        Assert.Equal(expected.releaseID, confirmBOD.releaseID);
        Assert.Equal(expected.systemEnvironmentCode, confirmBOD.systemEnvironmentCode);
        Assert.Equal(expected.versionID, confirmBOD.versionID);
        Assert.Equivalent(expected.DataArea, confirmBOD.DataArea);
    }

    [Fact]
    public void GenerateConfirmBODForInvalidBODTest()
    {
        var expected = new ConfirmBODType()
        {
            languageCode = "en-US",
            releaseID = "9.0",
            systemEnvironmentCode = "Production",
            versionID = null,
            ApplicationArea = new ApplicationAreaType()
            {
                BODID = new IdentifierType()
                {
                    Value = "an Identifier" // This won't match
                },
                CreationDateTime = DateTime.UtcNow.ToString() // now will this
            },
            DataArea = new ConfirmBODDataAreaType()
            {
                Confirm = new ConfirmType()
                {
                    OriginalApplicationArea = new ApplicationAreaType()
                    {
                        BODID = new IdentifierType() { Value = "11c696bc-2a2c-4c13-bff5-1c97d55494ba" },
                        CreationDateTime = "2019-09-13T04:21:21Z",
                        Sender = new SenderType()
                        {
                            LogicalID = new IdentifierType() { Value = "ba201587-fd83-4d8a-acb3-426ac0c0b9f3" }
                        }
                    }
                },
                BOD = new BODType[] {
                    new BODType() {
                        BODFailureMessage = new BODFailureMessageType()
                        {
                            ErrorProcessMessage = new MessageType[]
                            {
                                new MessageType()
                                {
                                    Description = new DescriptionType[]
                                    {
                                        new DescriptionType()
                                        {
                                            languageID = "en-US",
                                            Value = "The 'http://www.mimosa.org/ccom4:SyncSegments' element is not declared. at Line: 3 Position: 2"
                                        }
                                    }
                                }
                            },
                            WarningProcessMessage = new MessageType[] {}
                        }
                    }
                }
            },
        };

        BODReader reader = new BODReader(
            $"{DATA_PATH}/example_bod_sync_segments.xml",
            $"{BOD_SCHEMA_PATH}/Configuration/SyncAssets.xsd",
            settings
        );

        var confirmBOD = reader.GenerateConfirmBOD();

        Assert.Equal(expected.languageCode, confirmBOD.languageCode);
        Assert.Equal(expected.releaseID, confirmBOD.releaseID);
        Assert.Equal(expected.systemEnvironmentCode, confirmBOD.systemEnvironmentCode);
        Assert.Equal(expected.versionID, confirmBOD.versionID);
        Assert.Equivalent(expected.DataArea, confirmBOD.DataArea, true);
    }

    [Fact]
    public void BODInvalidSyntaxTest()
    {
        var expectedError = new ValidationResult(XmlSeverityType.Error, "Name cannot begin with the '<' character, hexadecimal value 0x3C. Line 9, position 5.", 9, 5);

        BODReader reader = new BODReader(
            $"{DATA_PATH}/example_bod_syntax_error.xml",
            $"{BOD_SCHEMA_PATH}/Configuration/SyncSegments.xsd",
            settings
        );

        Assert.False(reader.IsValid);
        Assert.Equal(1, reader.ValidationErrors.Count());
        Assert.Equal(expectedError, reader.ValidationErrors.First());
    }

    [Fact]
    public void BODInvalidSyntaxAndSchemaTest()
    {
        var expectedError1 = new ValidationResult(XmlSeverityType.Error, "Name cannot begin with the '<' character, hexadecimal value 0x3C. Line 9, position 5.", 9, 5);
        var expectedError2 = new ValidationResult(XmlSeverityType.Error, "The 'http://www.mimosa.org/ccom4:SyncSegments' element is not declared.", 3, 2);

        BODReader reader = new BODReader(
            $"{DATA_PATH}/example_bod_syntax_error.xml",
            $"{BOD_SCHEMA_PATH}/Configuration/SyncAssets.xsd",
            settings
        );

        Assert.False(reader.IsValid);
        Assert.Equal(2, reader.ValidationErrors.Count());
        Assert.Equal(expectedError1, reader.ValidationErrors.First());
        Assert.Equal(expectedError2, reader.ValidationErrors.Last());
    }

    [Fact]
    public void GenerateConfirmBODForSyntaxErrorBODTest()
    {
        var expected = new ConfirmBODType()
        {
            languageCode = "en-US",
            releaseID = "9.0",
            systemEnvironmentCode = "Production",
            versionID = null,
            ApplicationArea = new ApplicationAreaType()
            {
                BODID = new IdentifierType()
                {
                    Value = "an Identifier" // This won't match
                },
                CreationDateTime = DateTime.UtcNow.ToString() // now will this
            },
            DataArea = new ConfirmBODDataAreaType()
            {
                Confirm = new ConfirmType()
                {
                    OriginalApplicationArea = new ApplicationAreaType()
                    {
                        BODID = new IdentifierType() { Value = "11c696bc-2a2c-4c13-bff5-1c97d55494ba" },
                        CreationDateTime = "2019-09-13T04:21:21Z",
                        Sender = new SenderType()
                        {
                            LogicalID = new IdentifierType() { Value = "ba201587-fd83-4d8a-acb3-426ac0c0b9f3" },
                            ConfirmationCode = new ConfirmationResponseCodeType() { Value = "Always" }
                        }
                    }
                },
                BOD = new BODType[] {
                    new BODType() {
                        BODFailureMessage = new BODFailureMessageType()
                        {
                            ErrorProcessMessage = new MessageType[]
                            {
                                new MessageType()
                                {
                                    Description = new DescriptionType[]
                                    {
                                        new DescriptionType()
                                        {
                                            languageID = "en-US",
                                            Value = "Name cannot begin with the '<' character, hexadecimal value 0x3C. Line 9, position 5. at Line: 9 Position: 5"
                                        }
                                    }
                                },
                                new MessageType()
                                {
                                    Description = new DescriptionType[]
                                    {
                                        new DescriptionType()
                                        {
                                            languageID = "en-US",
                                            Value = "The 'http://www.mimosa.org/ccom4:SyncSegments' element is not declared. at Line: 3 Position: 2"
                                        }
                                    }
                                }
                            },
                            WarningProcessMessage = new MessageType[] {}
                        }
                    }
                }
            },
        };

        BODReader reader = new BODReader(
            $"{DATA_PATH}/example_bod_syntax_error.xml",
            $"{BOD_SCHEMA_PATH}/Configuration/SyncAssets.xsd",
            settings
        );

        var confirmBOD = reader.GenerateConfirmBOD();

        Assert.Equal(expected.languageCode, confirmBOD.languageCode);
        Assert.Equal(expected.releaseID, confirmBOD.releaseID);
        Assert.Equal(expected.systemEnvironmentCode, confirmBOD.systemEnvironmentCode);
        Assert.Equal(expected.versionID, confirmBOD.versionID);
        Assert.Equivalent(expected.DataArea, confirmBOD.DataArea, true);
    }
}