using Microsoft.EntityFrameworkCore;

/// <summary>
/// EF Core-реализация <see cref="IRequestService"/>.
/// Использует <see cref="AppDbContext"/> для доступа к данным и <see cref="ICurrentUser"/>
/// для получения id/роли текущего запроса.
/// </summary>
public class RequestService : IRequestService
{
  private readonly AppDbContext _db;
  private readonly ICurrentUser _currentUser;
  public RequestService(AppDbContext db, ICurrentUser currentUser)
  {
    _db = db;
    _currentUser = currentUser;
  }

  /// <inheritdoc/>
  public async Task<RequestDto> CreateAsync(CreateRequestDto dto, CancellationToken ct)
  {
    var type = await _db.RequestTypes.FirstOrDefaultAsync(t => t.Id == dto.TypeId, ct);
    if (type is null)
    {
      throw new NotFoundException($"Тип заявки {dto.TypeId} не найден");
    }

    if (type.RequiresCustomTemplate && string.IsNullOrWhiteSpace(dto.CustomTemplate))
      throw new ValidationException("Требуется описание справки");

    if (!type.RequiresCustomTemplate && !string.IsNullOrWhiteSpace(dto.CustomTemplate))
      throw new ValidationException("Описание справки не требуется");

    var duplicate = await _db.Requests.AnyAsync(r =>
    r.EmployeeId == _currentUser.UserId &&
    r.TypeId == dto.TypeId &&
    r.IsActive, ct);

    if (duplicate)
    {
      throw new ConflictException($"Активная заявка типа {dto.TypeId} уже существует");
    }
    var request = new Request(_currentUser.UserId, dto.TypeId, dto.Reason, dto.Quantity, dto.CustomTemplate);
    _db.Requests.Add(request);
    await _db.SaveChangesAsync(ct);
    return RequestDto.MapToDto(request, type, RequestStatuses.New);
  }

  /// <inheritdoc/>
  public async Task<IReadOnlyList<RequestDto>> GetAllAsync(CancellationToken ct)
  {
    return await _db.Requests
      .OrderByDescending(r => r.CreatedAt)
      .Select(request =>
        new RequestDto(request.Id, request.TypeId, request.Type.Name,
        request.StatusId, request.Status.Name,
        request.Reason, request.Quantity, request.CustomTemplate,
        request.EmployeeId, request.IsActive))
      .ToListAsync(ct);
  }

  /// <inheritdoc/>
  public async Task<RequestDto> GetByIdAsync(int id, CancellationToken ct)
  {
    var userId = _currentUser.UserId;
    var isAccountant = _currentUser.Role == Role.Accountant;
    var result = await _db.Requests
    .Where(r => r.Id == id && (isAccountant || r.EmployeeId == userId))
    .Select(request =>
      new RequestDto(request.Id, request.TypeId, request.Type.Name,
      request.StatusId, request.Status.Name,
      request.Reason, request.Quantity, request.CustomTemplate,
      request.EmployeeId, request.IsActive))
    .FirstOrDefaultAsync(ct);
    if (result is null) throw new NotFoundException($"Заявка {id} не найдена");
    return result;
  }

  /// <inheritdoc/>
  public async Task<IReadOnlyList<RequestDto>> GetByUserAsync(CancellationToken ct)
  {
    var userId = _currentUser.UserId;
    return await _db.Requests
      .Where(r => r.EmployeeId == userId)
      .Select(request =>
        new RequestDto(request.Id, request.TypeId, request.Type.Name,
        request.StatusId, request.Status.Name,
        request.Reason, request.Quantity, request.CustomTemplate,
        request.EmployeeId, request.IsActive))
      .ToListAsync(ct);
  }

  /// <inheritdoc/>
  public async Task<RequestDto> ChangeStatusAsync(int requestId, int newStatusId, CancellationToken ct)
  {
    var newStatus = await _db.RequestStatuses
        .FirstOrDefaultAsync(s => s.Id == newStatusId, ct);
    if (newStatus is null)
        throw new NotFoundException($"Статус {newStatusId} не найден");
    var request = await _db.Requests
      .Include(r => r.Type)
      .FirstOrDefaultAsync(r => r.Id == requestId, ct);

    if (request is null)
      throw new NotFoundException($"Заявка {requestId} не найдена");

    if (!request.IsActive)
          throw new ConflictException("Нельзя менять статус завершённой заявки");

    var oldStatusId = request.StatusId;

    request.ChangeStatus(newStatus);

    _db.RequestStatusHistories.Add(new RequestStatusHistory {
        RequestId = request.Id,
        FromStatusId = oldStatusId,
        ToStatusId = newStatusId,
        ChangedBy = _currentUser.UserId,
        ChangedAt = DateTime.UtcNow
    });

    await _db.SaveChangesAsync(ct);

    return RequestDto.MapToDto(request, request.Type, newStatus);
  }
}