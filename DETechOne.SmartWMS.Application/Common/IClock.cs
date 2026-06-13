namespace DETechOne.SmartWMS.Application.Common;

public interface IClock
{
    DateTime UtcNow { get; }
}
