using Ccom;

namespace CCOM.Net.Test;

public class EntityExtensionsTest
{
    [Fact]
    public void ToReferenceMinimalNameableTest()
    {
        Asset asset = new Asset
        {
            UUID = Guid.NewGuid(),
            IDInInfoSource = new IDType { Value = "IDInInfoSource" },
            InfoSource = new InfoSource
            {
                UUID = Guid.NewGuid(),
                ShortName = new TextType[] { "InfoSource short name" }
            },
            ShortName = new TextType[] { "Asset Name 1", "Asset Name 2" },
            FullName = new TextType[] { "Full Name 1", "Full Name 2" },
        };

        var assetAsReference = asset.ToReference(minimal: true);

        Assert.Equal(asset.UUID, assetAsReference.UUID);
        Assert.Null(assetAsReference.IDInInfoSource);
        Assert.Null(assetAsReference.InfoSource);
        Assert.Single(assetAsReference.ShortName, asset.ShortName[0]);
        Assert.Null(assetAsReference.FullName);
    }

    [Fact]
    public void ToReferenceMinimalNameableParentInfoSourceTest()
    {
        var parentInfoSource = new InfoSource
        {
            UUID = Guid.NewGuid(),
            ShortName = new TextType[] { "Parent InfoSource "}
        };

        Asset asset = new Asset
        {
            UUID = Guid.NewGuid(),
            IDInInfoSource = new IDType { Value = "IDInInfoSource" },
            InfoSource = new InfoSource
            {
                UUID = Guid.NewGuid(),
                ShortName = new TextType[] { "InfoSource short name" }
            },
            ShortName = new TextType[] { "Asset Name 1", "Asset Name 2" },
            FullName = new TextType[] { "Full Name 1", "Full Name 2" },
        };

        var assetAsReference = asset.ToReference(minimal: true, parentInfoSource: parentInfoSource);

        Assert.Equal(asset.UUID, assetAsReference.UUID);
        Assert.Null(assetAsReference.IDInInfoSource);
        Assert.Equal(asset.InfoSource.UUID, assetAsReference.InfoSource.UUID);
        Assert.Single(assetAsReference.ShortName, asset.ShortName[0]);
        Assert.Null(assetAsReference.FullName);
    }

    [Fact]
    public void ToReferenceMinimalNameableSameParentInfoSourceTest()
    {
        var parentInfoSource = new InfoSource
        {
            UUID = Guid.NewGuid(),
            ShortName = new TextType[] { "Parent InfoSource "}
        };

        Asset asset = new Asset
        {
            UUID = Guid.NewGuid(),
            IDInInfoSource = new IDType { Value = "IDInInfoSource" },
            InfoSource = parentInfoSource.ToReference(),
            ShortName = new TextType[] { "Asset Name 1", "Asset Name 2" },
            FullName = new TextType[] { "Full Name 1", "Full Name 2" },
        };

        var assetAsReference = asset.ToReference(minimal: true, parentInfoSource: parentInfoSource);

        Assert.Equal(asset.UUID, assetAsReference.UUID);
        Assert.Null(assetAsReference.IDInInfoSource);
        Assert.Null(assetAsReference.InfoSource);
        Assert.Single(assetAsReference.ShortName, asset.ShortName[0]);
        Assert.Null(assetAsReference.FullName);
    }

    [Fact]
    public void ToReferenceNameableTest()
    {
        Asset asset = new Asset
        {
            UUID = Guid.NewGuid(),
            IDInInfoSource = new IDType { Value = "IDInInfoSource" },
            InfoSource = new InfoSource
            {
                UUID = Guid.NewGuid(),
                ShortName = new TextType[] { "InfoSource short name" }
            },
            ShortName = new TextType[] { "Asset Name 1", "Asset Name 2" },
            FullName = new TextType[] { "Full Name 1", "Full Name 2" },
        };

        var assetAsReference = asset.ToReference(minimal: false);

        Assert.Equal(asset.UUID, assetAsReference.UUID);
        Assert.Equal(asset.IDInInfoSource, assetAsReference.IDInInfoSource);
        Assert.Equal(asset.InfoSource.UUID, assetAsReference.InfoSource.UUID);
        Assert.Single(assetAsReference.ShortName, asset.ShortName[0]);
        Assert.Null(assetAsReference.FullName);
    }

    [Fact]
    public void ToReferenceNameableNoIdInInfoSourceTest()
    {
        Asset asset = new Asset
        {
            UUID = Guid.NewGuid(),
            InfoSource = new InfoSource
            {
                UUID = Guid.NewGuid(),
                ShortName = new TextType[] { "InfoSource short name" }
            },
            ShortName = new TextType[] { "Asset Name 1", "Asset Name 2" },
            FullName = new TextType[] { "Full Name 1", "Full Name 2" },
        };

        var assetAsReference = asset.ToReference(minimal: false);

        Assert.Equal(asset.UUID, assetAsReference.UUID);
        Assert.Null(assetAsReference.IDInInfoSource);
        Assert.Null(assetAsReference.InfoSource);
        Assert.Single(assetAsReference.ShortName, asset.ShortName[0]);
        Assert.Null(assetAsReference.FullName);
    }

    [Fact]
    public void ToReferenceNameableNoIdInInfoSourceParentInfoSourceTest()
    {
        var parentInfoSource = new InfoSource
        {
            UUID = Guid.NewGuid(),
            ShortName = new TextType[] { "Parent InfoSource "}
        };

        Asset asset = new Asset
        {
            UUID = Guid.NewGuid(),
            InfoSource = new InfoSource
            {
                UUID = Guid.NewGuid(),
                ShortName = new TextType[] { "InfoSource short name" }
            },
            ShortName = new TextType[] { "Asset Name 1", "Asset Name 2" },
            FullName = new TextType[] { "Full Name 1", "Full Name 2" },
        };

        var assetAsReference = asset.ToReference(minimal: false, parentInfoSource: parentInfoSource);

        Assert.Equal(asset.UUID, assetAsReference.UUID);
        Assert.Null(assetAsReference.IDInInfoSource);
        Assert.Equal(asset.InfoSource.UUID, assetAsReference.InfoSource.UUID);
        Assert.Single(assetAsReference.ShortName, asset.ShortName[0]);
        Assert.Null(assetAsReference.FullName);
    }

    [Fact]
    public void ToReferenceNameableNoIdInInfoSourceSameParentInfoSourceTest()
    {
        var parentInfoSource = new InfoSource
        {
            UUID = Guid.NewGuid(),
            ShortName = new TextType[] { "Parent InfoSource "}
        };

        Asset asset = new Asset
        {
            UUID = Guid.NewGuid(),
            InfoSource = parentInfoSource.ToReference(),
            ShortName = new TextType[] { "Asset Name 1", "Asset Name 2" },
            FullName = new TextType[] { "Full Name 1", "Full Name 2" },
        };

        var assetAsReference = asset.ToReference(minimal: false, parentInfoSource: parentInfoSource);

        Assert.Equal(asset.UUID, assetAsReference.UUID);
        Assert.Null(assetAsReference.IDInInfoSource);
        Assert.Null(assetAsReference.InfoSource);
        Assert.Single(assetAsReference.ShortName, asset.ShortName[0]);
        Assert.Null(assetAsReference.FullName);
    }

    [Fact]
    public void ToReferenceNameableNullShortNameTest()
    {
        Asset asset = new Asset
        {
            UUID = Guid.NewGuid(),
            IDInInfoSource = new IDType { Value = "IDInInfoSource" },
            InfoSource = new InfoSource
            {
                UUID = Guid.NewGuid(),
                ShortName = new TextType[] { "InfoSource short name" }
            },
            // ShortName = new TextType[] { "Asset Name 1", "Asset Name 2" },
            FullName = new TextType[] { "Full Name 1", "Full Name 2" },
        };

        var assetAsReference = asset.ToReference(minimal: true);

        Assert.Equal(asset.UUID, assetAsReference.UUID);
        Assert.Null(asset.ShortName);
    }

    [Fact]
    public void ToReferenceNameableEmptyShortNameTest()
    {
        Asset asset = new Asset
        {
            UUID = Guid.NewGuid(),
            IDInInfoSource = new IDType { Value = "IDInInfoSource" },
            InfoSource = new InfoSource
            {
                UUID = Guid.NewGuid(),
                ShortName = new TextType[] { "InfoSource short name" }
            },
            ShortName = new TextType[] { },
            FullName = new TextType[] { "Full Name 1", "Full Name 2" },
        };

        var assetAsReference = asset.ToReference(minimal: true);

        Assert.Equal(asset.UUID, assetAsReference.UUID);
        Assert.Empty(asset.ShortName);
    }

    [Fact]
    public void ToReferenceRecursiveInfoSourceTest()
    {
        InfoSource infoSource = new InfoSource
        {
            UUID = Guid.NewGuid(),
            IDInInfoSource = new IDType { Value = "IDInInfoSource" },
            ShortName = new TextType[] { "Asset Name 1", "Asset Name 2" },
            FullName = new TextType[] { "Full Name 1", "Full Name 2" },
        };
        infoSource.InfoSource = infoSource;

        var infoSourceAsReference = infoSource.ToReference(minimal: true);

        Assert.Equal(infoSource.UUID, infoSourceAsReference.UUID);
        Assert.Null(infoSourceAsReference.IDInInfoSource);
        Assert.Null(infoSourceAsReference.InfoSource);
        Assert.Single(infoSourceAsReference.ShortName, infoSource.ShortName[0]);
        Assert.Null(infoSourceAsReference.FullName);
        Assert.NotEqual(infoSource, infoSourceAsReference);
    }
}