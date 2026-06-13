namespace DETechOne.SmartWMS.Contracts.Dtos.SAP;

public sealed class SapSalesOrderDto
{
    public int DocEntry { get; init; }
    public int DocNum { get; init; }
    public string CardCode { get; init; } = string.Empty;
    public string CardName { get; init; } = string.Empty;
    public string DocStatus { get; init; } = string.Empty;
    public DateTime? DocDate { get; init; }
    public IReadOnlyCollection<SapSalesOrderLineDto> Lines { get; init; } = Array.Empty<SapSalesOrderLineDto>();
}
