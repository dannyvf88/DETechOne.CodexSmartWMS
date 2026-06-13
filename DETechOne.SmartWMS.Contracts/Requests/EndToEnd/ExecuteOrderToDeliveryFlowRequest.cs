namespace DETechOne.SmartWMS.Contracts.Requests.EndToEnd;

public sealed class ExecuteOrderToDeliveryFlowRequest
{
    public Guid FlowId { get; init; }
    public bool AutoCompletePicking { get; init; }
    public bool AutoCompletePacking { get; init; }
    public bool AutoConfirmShipping { get; init; }
    public bool CreateSapDelivery { get; init; }
    public string? PackageCode { get; init; }
}
