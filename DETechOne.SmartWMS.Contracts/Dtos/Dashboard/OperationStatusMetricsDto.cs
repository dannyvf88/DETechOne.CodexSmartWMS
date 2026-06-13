namespace DETechOne.SmartWMS.Contracts.Dtos.Dashboard;

public sealed class OperationStatusMetricsDto
{
    public int Open { get; init; }
    public int InProgress { get; init; }
    public int Completed { get; init; }
    public int Cancelled { get; init; }
    public int DeliveryCreated { get; init; }
    public decimal RequiredQuantity { get; init; }
    public decimal ProcessedQuantity { get; init; }
    public decimal PendingQuantity { get; init; }
}
