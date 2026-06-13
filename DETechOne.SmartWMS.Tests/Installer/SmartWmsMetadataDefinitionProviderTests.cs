using DETechOne.SmartWMS.Domain.Enums;
using DETechOne.SmartWMS.Installer.Definitions;

namespace DETechOne.SmartWMS.Tests.Installer;

public sealed class SmartWmsMetadataDefinitionProviderTests
{
    [Fact]
    public void GetDefinitions_ShouldReturnCoreSmartWmsMetadata()
    {
        var provider = new SmartWmsMetadataDefinitionProvider();

        var definitions = provider.GetDefinitions();

        Assert.NotEmpty(definitions);
        Assert.Contains(definitions, item => item.Code == "@DTO_SW_OJ_HPICKING" && item.ObjectType == MetadataObjectType.UserTable);
        Assert.Contains(definitions, item => item.Code == "@DTO_SW_OJ_HPACKING" && item.ObjectType == MetadataObjectType.UserTable);
        Assert.Contains(definitions, item => item.Code == "DTO_SW_PICKING" && item.ObjectType == MetadataObjectType.UserObject);
    }
}
