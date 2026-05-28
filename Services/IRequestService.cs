/// <summary>
/// Доменные операции над заявками: создание, чтение, смена статуса.
/// Все методы предполагают аутентифицированного пользователя — Id и роль
/// берутся из <see cref="ICurrentUser"/>.
/// </summary>
public interface IRequestService
{
  /// <summary>
  /// Создать заявку от имени текущего пользователя.
  /// EmployeeId берётся из <see cref="ICurrentUser.UserId"/>, статус выставляется в New
  /// конструктором <see cref="Request"/>.
  /// </summary>
  /// <param name="dto">Данные новой заявки (тип, причина, количество, опциональный шаблон).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>DTO созданной заявки с заполненным Id, StatusName и TypeName.</returns>
  /// <exception cref="NotFoundException">Указанный <c>TypeId</c> не существует.</exception>
  /// <exception cref="ValidationException">
  /// <c>CustomTemplate</c> не соответствует флагу <c>RequiresCustomTemplate</c> на типе:
  /// либо отсутствует там где требуется, либо передан там где не нужен.
  /// </exception>
  /// <exception cref="ConflictException">
  /// У текущего пользователя уже есть активная заявка того же типа.
  /// Защищено уникальным частичным индексом на стороне БД.
  /// </exception>
  Task<RequestDto> CreateAsync(CreateRequestDto dto, CancellationToken ct);

  /// <summary>
  /// Список всех заявок, отсортированных по <c>CreatedAt</c> убыванию.
  /// Предполагает, что вызывающий — Accountant (проверяется на уровне контроллера).
  /// </summary>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Все заявки в виде DTO. Пустой список если ничего нет.</returns>
  Task<IReadOnlyList<RequestDto>> GetAllAsync(CancellationToken ct);

  /// <summary>
  /// Заявки текущего пользователя — фильтр по <see cref="ICurrentUser.UserId"/>.
  /// Используется обычными сотрудниками для просмотра своих заявок.
  /// </summary>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Заявки, у которых EmployeeId совпадает с current user. Пустой список если нет.</returns>
  Task<IReadOnlyList<RequestDto>> GetByUserAsync(CancellationToken ct);

  /// <summary>
  /// Получить заявку по id с учётом row-level авторизации:
  /// Employee видит только свои заявки, Accountant — любые.
  /// Если заявка чужая для Employee — метод бросает 404 (а не 403),
  /// чтобы не раскрывать существование объекта.
  /// </summary>
  /// <param name="id">Идентификатор заявки.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>DTO заявки.</returns>
  /// <exception cref="NotFoundException">
  /// Заявка с таким id отсутствует или принадлежит другому пользователю
  /// (для не-Accountant ролей).
  /// </exception>
  Task<RequestDto> GetByIdAsync(int id, CancellationToken ct);

  /// <summary>
  /// Сменить статус заявки и зафиксировать переход в <c>request_status_histories</c>.
  /// Обновление заявки и запись истории выполняются в одной транзакции
  /// (один <c>SaveChangesAsync</c>) — атомарно.
  /// </summary>
  /// <param name="id">Идентификатор заявки.</param>
  /// <param name="newStatusId">Целевой статус. Должен существовать в <c>request_statuses</c>.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>DTO заявки с обновлённым статусом, <c>IsActive</c> и <c>UpdatedAt</c>.</returns>
  /// <exception cref="NotFoundException">
  /// Заявка или указанный статус не существуют.
  /// </exception>
  /// <exception cref="ConflictException">
  /// Заявка уже в terminal-статусе (<c>IsActive == false</c>) — менять нельзя.
  /// </exception>
  Task<RequestDto> ChangeStatusAsync(int id, int newStatusId, CancellationToken ct);
}
