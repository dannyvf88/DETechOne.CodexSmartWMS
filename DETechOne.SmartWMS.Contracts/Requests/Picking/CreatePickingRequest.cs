namespace DETechOne.SmartWMS.Contracts.Requests.Picking;

public sealed class CreatePickingRequest
{
    public string SourceDocumentType { get; set; } = string.Empty;
    public string SourceDocumentNumber { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public IReadOnlyList<CreatePickingLineRequest> Lines { get; set; } = Array.Empty<CreatePickingLineRequest>();
}
