using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/request")]
[Authorize]
public class RequestController: ControllerBase
{
  private readonly IRequestService _service;

  public RequestController(IRequestService requestService)
  {
    _service = requestService;
  }
  [HttpPost]
  public async Task<ActionResult<RequestDto>> CreateRequest(CreateRequestDto dto, CancellationToken ct)
  {
    var result = await _service.CreateAsync(dto, ct);
    return CreatedAtAction(nameof(GetById), new {id = result.Id}, result);
  }

  [HttpGet]
  [Authorize(Roles = "Accountant")] // не забыть
  public async Task<ActionResult<IReadOnlyList<RequestDto>>> GetRequests(CancellationToken ct){
    return Ok(await _service.GetAllAsync(ct));
  }

  [HttpGet("my")]
  public async Task<ActionResult<IReadOnlyList<RequestDto>>> GetRequestsByUser(CancellationToken ct){
    return Ok(await _service.GetByUserAsync(ct));
  }

  [HttpGet("{id:int}")]
  public async  Task<ActionResult<RequestDto>> GetById(int id, CancellationToken ct)
  {
    return Ok(await _service.GetByIdAsync(id, ct));
  }
}