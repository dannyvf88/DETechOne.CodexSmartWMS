namespace DETechOne.SmartWMS.Domain.Common;

public sealed class Result
{
    private Result(bool success, string? errorCode, string? message)
    {
        Success = success;
        ErrorCode = errorCode;
        Message = message;
    }

    public bool Success { get; }
    public string? ErrorCode { get; }
    public string? Message { get; }

    public static Result Ok(string? message = null) => new(true, null, message);
    public static Result Fail(string errorCode, string message) => new(false, errorCode, message);
}

public sealed class Result<T>
{
    private Result(bool success, T? value, string? errorCode, string? message)
    {
        Success = success;
        Value = value;
        ErrorCode = errorCode;
        Message = message;
    }

    public bool Success { get; }
    public T? Value { get; }
    public string? ErrorCode { get; }
    public string? Message { get; }

    public static Result<T> Ok(T value, string? message = null) => new(true, value, null, message);
    public static Result<T> Fail(string errorCode, string message) => new(false, default, errorCode, message);
}
