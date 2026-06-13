using DETechOne.SmartWMS.Contracts.Dtos.Movement;
using DETechOne.SmartWMS.Contracts.Requests.Movement;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Movement;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Application.Movement;

public sealed class MovementService : IMovementService
{
    private const string DefaultSystemUser = "system";
    private readonly IMovementRepository _movementRepository;

    public MovementService(IMovementRepository movementRepository)
    {
        _movementRepository = movementRepository;
    }

    public async Task<Result<MovementDocumentDto>> CreateAsync(CreateMovementRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        string validationError = ValidateCreateRequest(request);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return Result<MovementDocumentDto>.Fail("MOVEMENT_VALIDATION", validationError);
        }

        if (!TryParseMovementType(request.MovementType, out MovementType movementType))
        {
            return Result<MovementDocumentDto>.Fail("MOVEMENT_VALIDATION", "MovementType is invalid.");
        }

        string actor = NormalizeActor(userName);
        string movementNumber = await _movementRepository.GetNextMovementNumberAsync(movementType, cancellationToken).ConfigureAwait(false);
        var document = new MovementDocument(movementNumber, movementType, request.ReferenceType, request.ReferenceNumber, actor);
        document.MarkCreated(actor);

        foreach (CreateMovementLineRequest lineRequest in request.Lines.OrderBy(line => line.LineNumber))
        {
            document.AddLine(new MovementLine(
                lineRequest.LineNumber,
                lineRequest.ItemCode,
                lineRequest.FromWarehouseCode,
                lineRequest.FromLocationCode,
                lineRequest.ToWarehouseCode,
                lineRequest.ToLocationCode,
                lineRequest.Quantity,
                lineRequest.LotNumber,
                lineRequest.UomCode));
        }

        await _movementRepository.AddAsync(document, cancellationToken).ConfigureAwait(false);
        return Result<MovementDocumentDto>.Ok(MovementMapper.ToDto(document), "Movement document created successfully.");
    }

    public async Task<Result<MovementDocumentDto>> ConfirmAsync(ConfirmMovementRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.MovementId == Guid.Empty)
        {
            return Result<MovementDocumentDto>.Fail("MOVEMENT_VALIDATION", "MovementId is required.");
        }

        MovementDocument? document = await _movementRepository.GetByIdAsync(request.MovementId, cancellationToken).ConfigureAwait(false);
        if (document is null)
        {
            return Result<MovementDocumentDto>.Fail("MOVEMENT_NOT_FOUND", "Movement document was not found.");
        }

        try
        {
            document.Confirm(NormalizeActor(userName));
        }
        catch (InvalidOperationException exception)
        {
            return Result<MovementDocumentDto>.Fail("MOVEMENT_INVALID_STATE", exception.Message);
        }

        await _movementRepository.UpdateAsync(document, cancellationToken).ConfigureAwait(false);
        return Result<MovementDocumentDto>.Ok(MovementMapper.ToDto(document), "Movement document confirmed successfully.");
    }

    public async Task<Result<MovementDocumentDto>> CancelAsync(CancelMovementRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.MovementId == Guid.Empty)
        {
            return Result<MovementDocumentDto>.Fail("MOVEMENT_VALIDATION", "MovementId is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return Result<MovementDocumentDto>.Fail("MOVEMENT_VALIDATION", "Reason is required.");
        }

        MovementDocument? document = await _movementRepository.GetByIdAsync(request.MovementId, cancellationToken).ConfigureAwait(false);
        if (document is null)
        {
            return Result<MovementDocumentDto>.Fail("MOVEMENT_NOT_FOUND", "Movement document was not found.");
        }

        try
        {
            document.Cancel(NormalizeActor(userName), request.Reason);
        }
        catch (InvalidOperationException exception)
        {
            return Result<MovementDocumentDto>.Fail("MOVEMENT_INVALID_STATE", exception.Message);
        }

        await _movementRepository.UpdateAsync(document, cancellationToken).ConfigureAwait(false);
        return Result<MovementDocumentDto>.Ok(MovementMapper.ToDto(document), "Movement document cancelled successfully.");
    }

    public async Task<Result<MovementDocumentDto>> GetByIdAsync(Guid movementId, CancellationToken cancellationToken = default)
    {
        if (movementId == Guid.Empty)
        {
            return Result<MovementDocumentDto>.Fail("MOVEMENT_VALIDATION", "MovementId is required.");
        }

        MovementDocument? document = await _movementRepository.GetByIdAsync(movementId, cancellationToken).ConfigureAwait(false);
        return document is null
            ? Result<MovementDocumentDto>.Fail("MOVEMENT_NOT_FOUND", "Movement document was not found.")
            : Result<MovementDocumentDto>.Ok(MovementMapper.ToDto(document));
    }

    public async Task<Result<IReadOnlyList<MovementDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MovementDocument> documents = await _movementRepository.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        IReadOnlyList<MovementDocumentDto> dto = documents.Select(MovementMapper.ToDto).ToArray();
        return Result<IReadOnlyList<MovementDocumentDto>>.Ok(dto);
    }

    private static string ValidateCreateRequest(CreateMovementRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MovementType)) return "MovementType is required.";
        if (string.IsNullOrWhiteSpace(request.ReferenceType)) return "ReferenceType is required.";
        if (request.Lines.Count == 0) return "At least one movement line is required.";

        var lineNumbers = new HashSet<int>();
        foreach (CreateMovementLineRequest line in request.Lines)
        {
            if (line.LineNumber <= 0) return "LineNumber must be greater than zero.";
            if (!lineNumbers.Add(line.LineNumber)) return $"LineNumber {line.LineNumber} is duplicated.";
            if (string.IsNullOrWhiteSpace(line.ItemCode)) return $"ItemCode is required in line {line.LineNumber}.";
            if (string.IsNullOrWhiteSpace(line.FromWarehouseCode)) return $"FromWarehouseCode is required in line {line.LineNumber}.";
            if (string.IsNullOrWhiteSpace(line.FromLocationCode)) return $"FromLocationCode is required in line {line.LineNumber}.";
            if (string.IsNullOrWhiteSpace(line.ToWarehouseCode)) return $"ToWarehouseCode is required in line {line.LineNumber}.";
            if (string.IsNullOrWhiteSpace(line.ToLocationCode)) return $"ToLocationCode is required in line {line.LineNumber}.";
            if (line.Quantity <= 0) return $"Quantity must be greater than zero in line {line.LineNumber}.";
            if (line.FromWarehouseCode.Equals(line.ToWarehouseCode, StringComparison.OrdinalIgnoreCase)
                && line.FromLocationCode.Equals(line.ToLocationCode, StringComparison.OrdinalIgnoreCase))
            {
                return $"Origin and destination cannot be the same in line {line.LineNumber}.";
            }
        }

        return string.Empty;
    }

    private static bool TryParseMovementType(string value, out MovementType movementType)
    {
        if (Enum.TryParse(value, ignoreCase: true, out movementType))
        {
            return Enum.IsDefined(movementType);
        }

        movementType = default;
        return false;
    }

    private static string NormalizeActor(string? userName) => string.IsNullOrWhiteSpace(userName) ? DefaultSystemUser : userName.Trim();
}
