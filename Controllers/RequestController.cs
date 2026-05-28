using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/request")]
[Authorize]
[Tags("Requests")]
public class RequestController: ControllerBase
{
  private readonly IRequestService _service;

  public RequestController(IRequestService requestService)
  {
    _service = requestService;
  }

  [HttpPost]
  [EndpointSummary("Создать заявку")]
  [EndpointDescription("Создаёт новую заявку от имени текущего пользователя. EmployeeId берётся из X-User-Id, статус всегда New.")]
  [ProducesResponseType<RequestDto>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<RequestDto>> CreateRequest(CreateRequestDto dto, CancellationToken ct)
  {
    var result = await _service.CreateAsync(dto, ct);
    return CreatedAtAction(nameof(GetById), new {id = result.Id}, result);
  }

  [HttpGet]
  [Authorize(Roles = "Accountant")]
  [EndpointSummary("Список всех заявок (только для Accountant)")]
  [ProducesResponseType<IReadOnlyList<RequestDto>>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<IReadOnlyList<RequestDto>>> GetRequests(CancellationToken ct){
    return Ok(await _service.GetAllAsync(ct));
  }

  [HttpGet("my")]
  [EndpointSummary("Заявки текущего пользователя")]
  [EndpointDescription("Возвращает заявки, у которых EmployeeId совпадает с X-User-Id текущего запроса.")]
  [ProducesResponseType<IReadOnlyList<RequestDto>>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult<IReadOnlyList<RequestDto>>> GetRequestsByUser(CancellationToken ct){
    return Ok(await _service.GetByUserAsync(ct));
  }

  [HttpGet("{id:int}")]
  [EndpointSummary("Заявка по id")]
  [EndpointDescription("Employee видит только свои заявки (чужие → 404). Accountant видит любые.")]
  [ProducesResponseType<RequestDto>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async  Task<ActionResult<RequestDto>> GetById(int id, CancellationToken ct)
  {
    return Ok(await _service.GetByIdAsync(id, ct));
  }

  [HttpPatch("{id:int}/status")]
  [Authorize(Roles = "Accountant")]
  [EndpointSummary("Сменить статус заявки (только для Accountant)")]
  [EndpointDescription("Меняет статус заявки и создаёт запись в истории переходов в одной транзакции. Запрещено для заявок в terminal-статусе.")]
  [ProducesResponseType<RequestDto>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
   public async Task<ActionResult<RequestDto>> ChangeStatus(
      int id, [FromQuery] int newStatusId, CancellationToken ct)
      => Ok(await _service.ChangeStatusAsync(id, newStatusId, ct));
}
