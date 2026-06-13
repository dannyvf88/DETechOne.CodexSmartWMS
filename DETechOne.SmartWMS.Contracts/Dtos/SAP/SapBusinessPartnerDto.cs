namespace DETechOne.SmartWMS.Contracts.Dtos.SAP;

public sealed class SapBusinessPartnerDto
{
    public string CardCode { get; init; } = string.Empty;
    public string CardName { get; init; } = string.Empty;
    public string CardType { get; init; } = string.Empty;
    public bool Active { get; init; }
}
