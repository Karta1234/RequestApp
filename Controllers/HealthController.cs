using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/health")]
[AllowAnonymous]
[Tags("Health")]
public class HealthController: ControllerBase
{
  private readonly AppDbContext _db;
  public HealthController(AppDbContext db) => _db = db;

  /// <summary>Liveness + проверка соединения с БД.</summary>
  [HttpGet]
  [EndpointSummary("Health check")]
  [EndpointDescription("Возвращает 200 если приложение живо и соединение с БД активно, иначе 503.")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
  public async Task<IActionResult> Get(CancellationToken ct)
  {
    var canConnect = await _db.Database.CanConnectAsync(ct);
    return canConnect ? Ok(new {status = "Healthy", db = "up"}) : StatusCode(503, new {status= "Unhealty", db = "down"});
  }

}
