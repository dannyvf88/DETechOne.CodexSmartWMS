using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Device;

public sealed class ScannerSession : BaseEntity
{
    private readonly List<ScannerEvent> _events = new();

    private ScannerSession()
    {
        DeviceCode = string.Empty;
        Operation = string.Empty;
        OperatorUserName = string.Empty;
    }

    public ScannerSession(
        string deviceCode,
        string operation,
        string? referenceDocument,
        string operatorUserName)
    {
        DeviceCode = string.IsNullOrWhiteSpace(deviceCode) ? throw new ArgumentException("Device code is required.", nameof(deviceCode)) : deviceCode.Trim();
        Operation = string.IsNullOrWhiteSpace(operation) ? throw new ArgumentException("Operation is required.", nameof(operation)) : operation.Trim();
        ReferenceDocument = string.IsNullOrWhiteSpace(referenceDocument) ? null : referenceDocument.Trim();
        OperatorUserName = string.IsNullOrWhiteSpace(operatorUserName) ? throw new ArgumentException("Operator user name is required.", nameof(operatorUserName)) : operatorUserName.Trim();
        Status = ScannerSessionStatus.Open;
        StartedAtUtc = DateTime.UtcNow;
        CreatedBy = OperatorUserName;
    }

    public string DeviceCode { get; private set; }
    public string Operation { get; private set; }
    public string? ReferenceDocument { get; private set; }
    public string OperatorUserName { get; private set; }
    public ScannerSessionStatus Status { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public IReadOnlyCollection<ScannerEvent> Events => _events.AsReadOnly();

    public ScannerEvent AddEvent(ScannerEventType eventType, string value, string? symbology, string? source)
    {
        if (Status != ScannerSessionStatus.Open)
        {
            throw new InvalidOperationException("Scanner session is not open.");
        }

        var scannerEvent = new ScannerEvent(Id, eventType, value, symbology, source, OperatorUserName);
        _events.Add(scannerEvent);
        MarkUpdated(OperatorUserName);
        return scannerEvent;
    }

    public void Complete(string userName)
    {
        if (Status != ScannerSessionStatus.Open)
        {
            throw new InvalidOperationException("Only open scanner sessions can be completed.");
        }

        Status = ScannerSessionStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        MarkUpdated(userName);
    }

    public void Cancel(string reason, string userName)
    {
        if (Status != ScannerSessionStatus.Open)
        {
            throw new InvalidOperationException("Only open scanner sessions can be cancelled.");
        }

        Status = ScannerSessionStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
        CancellationReason = string.IsNullOrWhiteSpace(reason) ? "Cancelled by operator." : reason.Trim();
        MarkUpdated(userName);
    }
}
