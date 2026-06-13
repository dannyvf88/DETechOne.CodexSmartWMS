using DETechOne.SmartWMS.Domain.Entities.Movement;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Application.Movement;

public interface IMovementRepository
{
    Task<MovementDocument?> GetByIdAsync(Guid movementId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MovementDocument>> GetOpenAsync(CancellationToken cancellationToken = default);
    Task AddAsync(MovementDocument movementDocument, CancellationToken cancellationToken = default);
    Task UpdateAsync(MovementDocument movementDocument, CancellationToken cancellationToken = default);
    Task<string> GetNextMovementNumberAsync(MovementType movementType, CancellationToken cancellationToken = default);
}
