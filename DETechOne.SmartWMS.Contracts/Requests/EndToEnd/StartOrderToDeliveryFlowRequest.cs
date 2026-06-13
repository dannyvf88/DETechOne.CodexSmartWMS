namespace DETechOne.SmartWMS.Contracts.Requests.EndToEnd;

public sealed class StartOrderToDeliveryFlowRequest
{
    public int SalesOrderDocEntry { get; init; }
    public string? WarehouseCode { get; init; }
    public string? CorrelationId { get; init; }
}
