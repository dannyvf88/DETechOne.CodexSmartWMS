namespace DETechOne.SmartWMS.Application.EndToEnd;

public static class EndToEndFlowStatus
{
    public const string Created = "Created";
    public const string PickingCreated = "PickingCreated";
    public const string PickingCompleted = "PickingCompleted";
    public const string PackingCreated = "PackingCreated";
    public const string PackingCompleted = "PackingCompleted";
    public const string ShippingCreated = "ShippingCreated";
    public const string ShippingConfirmed = "ShippingConfirmed";
    public const string DeliveryCreated = "DeliveryCreated";
    public const string Completed = "Completed";
    public const string WaitingForOperator = "WaitingForOperator";
    public const string Failed = "Failed";
}
