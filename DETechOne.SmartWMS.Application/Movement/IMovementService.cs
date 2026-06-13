using DETechOne.SmartWMS.Contracts.Dtos.Movement;
using DETechOne.SmartWMS.Contracts.Requests.Movement;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Movement;

public interface IMovementService
{
    Task<Result<MovementDocumentDto>> CreateAsync(CreateMovementRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<MovementDocumentDto>> ConfirmAsync(ConfirmMovementRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<MovementDocumentDto>> CancelAsync(CancelMovementRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<MovementDocumentDto>> GetByIdAsync(Guid movementId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<MovementDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default);
}
