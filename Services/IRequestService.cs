public interface IRequestService
{
  Task<RequestDto> CreateAsync(CreateRequestDto dto, CancellationToken ct);
  Task<IReadOnlyList<RequestDto>> GetAllAsync(CancellationToken ct);
  Task<IReadOnlyList<RequestDto>> GetByUserAsync(CancellationToken ct);
  Task<RequestDto> GetByIdAsync(int id, CancellationToken ct);
  Task<RequestDto> ChangeStatusAsync(int id, int newStatusId, CancellationToken ct);
}