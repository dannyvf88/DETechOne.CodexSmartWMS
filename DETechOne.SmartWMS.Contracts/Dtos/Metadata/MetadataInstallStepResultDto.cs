using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Contracts.Dtos.Metadata;

public sealed record MetadataInstallStepResultDto(
    string Code,
    string Name,
    MetadataObjectType ObjectType,
    MetadataInstallStatus Status,
    bool Success,
    string Message);
