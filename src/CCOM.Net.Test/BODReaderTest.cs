using CommonBOD;

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
        BODReader reader = new BODReader(
            $"{DATA_PATH}/example_bod_sync_segments.xml",
            $"{BOD_SCHEMA_PATH}/Configuration/SyncAssets.xsd",
            settings
        );

        Assert.False(reader.IsValid);
        Assert.NotEmpty(reader.ValidationErrors);
        Assert.Equal("SyncSegments", reader.SimpleName);
    }
}