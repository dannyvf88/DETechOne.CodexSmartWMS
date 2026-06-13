namespace DETechOne.SmartWMS.Web.Models.Common;

public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public T? Data { get; set; }
    public T? Value { get; set; }

    public T? GetPayload() => Data is not null ? Data : Value;
}
